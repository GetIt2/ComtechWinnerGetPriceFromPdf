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
            while (true)
            {
                Console.Write("Insert folder location: ");
                var fileLoc = Console.ReadLine();
                var files = GetAllFilesFromFolder(fileLoc);
                var totalPrice = 0;
                foreach (var file in files)
                {
                    var currentText = GetTextFromFile(file);
                    totalPrice += GetPriceFromText(currentText, false);
                }
                Console.WriteLine("Total: " + totalPrice.ToString("##,###"));
            }
        }

        private static string GetTextFromFile(FileInfo file)
        {
            Console.WriteLine(file.DirectoryName + "\\" + file.Name);
            PdfReader pdfReader = new PdfReader(file.DirectoryName + "\\" + file.Name);
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            var currentText = "";
            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                currentText += PdfTextExtractor.GetTextFromPage(pdfReader, i, strategy);
            }
            return currentText;
        }

        private static FileInfo[] GetAllFilesFromFolder(string fileLoc)
        {
            DirectoryInfo di = new DirectoryInfo(fileLoc);
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
                // Console.WriteLine($"/{word}\\");
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
            var priceInInt = int.Parse(roundedPrice[0]);
            Console.WriteLine("Kr: " + priceInInt.ToString("##,###"));
            return priceInInt;
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
