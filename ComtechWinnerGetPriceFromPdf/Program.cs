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
            DirectoryInfo di = new DirectoryInfo(@"C:\GET\GET-Academy Solutions\Comtech Winner pris beregner");
            FileInfo[] files = di.GetFiles("Tilbud-1.pdf", SearchOption.AllDirectories);
            var totalPrice = 0;
            foreach (var file in files)
            {
                Console.WriteLine(file.DirectoryName + "\\" + file.Name);
                PdfReader pdfReader = new PdfReader(file.DirectoryName + "\\" + file.Name);
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, pdfReader.NumberOfPages, strategy);
                totalPrice += GetPriceFromText(currentText);
            }
            Console.WriteLine(totalPrice);
        }

        private static int GetPriceFromText(string currentText)
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
                    || word == "NOK") wordsCorrect++;
                else wordsCorrect = 0;
            }

            Console.WriteLine(price);
            if (price == "") price = "0";
            var roundedPrice = price.Split(',');
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
