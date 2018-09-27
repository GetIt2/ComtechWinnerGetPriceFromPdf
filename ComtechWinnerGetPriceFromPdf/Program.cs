using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ComtechWinnerGetPriceFromPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = GetAllFilesFromFolder();
            var totalPrice = 0;
            foreach (var file in files)
            {
                var currentText = GetTextFromFile(file);
                totalPrice += GetPriceFromText(currentText, false);
            }
            Console.WriteLine(totalPrice);
        }

        private static string GetTextFromFile(FileInfo file)
        {
            Console.WriteLine(file.DirectoryName + "\\" + file.Name);
            PdfReader pdfReader = new PdfReader(file.DirectoryName + "\\" + file.Name);
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, pdfReader.NumberOfPages, strategy);
            return currentText;
        }

        private static FileInfo[] GetAllFilesFromFolder()
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\GET\GET-Academy Solutions\Comtech Winner pris beregner");
            FileInfo[] files = di.GetFiles("Tilbud-1.pdf", SearchOption.AllDirectories);
            return files;
        }

        private static int GetPriceFromText(string currentText, bool priceNotFoundOnFirstAttempt)
        {
            var firstSplit = currentText.Split(' ');
            var wordsCorrect = 0;
            var secondPartOfPrice = false;
            var price = "";
            var words = SplitOnNewLine(firstSplit);
            foreach (var word in words)
            {
                if (wordsCorrect == 4 || secondPartOfPrice)
                {
                    secondPartOfPrice = !secondPartOfPrice;
                    price += word;
                }
                if (word == "Totalt"
                    || word == "ink"
                    || word == "MVA"
                    || word == "NOK"
                    || (word == "eks"
                        && priceNotFoundOnFirstAttempt)) wordsCorrect++;
                else wordsCorrect = 0;
            }

            if (price == "" && priceNotFoundOnFirstAttempt) price = "0";
            if (price == "") return GetPriceFromText(currentText, true);
            var roundedPrice = price.Split(',');
            Console.WriteLine(roundedPrice[0]);
            return int.Parse(roundedPrice[0]);
        }

        private static string[] SplitOnNewLine(string[] firstSplit)
        {
            var words = new List<string>();
            foreach (var s in firstSplit)
            {
                string[] lines = s.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
                foreach (var line in lines)
                {
                    words.Add(line);
                }
            }

            return words.ToArray();
        }
    }
}
