using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using System.ComponentModel.DataAnnotations;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using System.Text.RegularExpressions;

namespace RenameInvoice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.pdf");
            foreach (var file in files)
            {
                if (GetInfo(file, out var info))
                {
                    var name = Path.Combine(file.Directory.FullName, $"{info.Money}{info.Company}{info.Date}{file.Extension}");
                    Console.WriteLine(name);
                }
            }
        }

        static bool GetInfo(FileInfo file, out InvoiceInfo info)
        {
            using (PdfDocument document = PdfDocument.Open(file.FullName))
            {
                var letters = document.GetPage(1).Letters; // no preprocessing

                // 1. Extract words
                var wordExtractor = NearestNeighbourWordExtractor.Instance;

                var words = wordExtractor.GetWords(letters);

                // 2. Segment page
                var pageSegmenter = DocstrumBoundingBoxes.Instance;

                var textBlocks = pageSegmenter.GetBlocks(words);

                // 3. Postprocessing
                var readingOrder = RenderingReadingOrderDetector.Instance;
                var orderedTextBlocks = readingOrder.Get(textBlocks);

                info = new InvoiceInfo();
                foreach (var block in orderedTextBlocks)
                {
                    Regex regex = new Regex("开票日期：(?<v>\\d{4}年\\d{2}月\\d{2})");
                    var match = regex.Match(block.Text);
                    if (match.Success)
                    {
                        info.Date = match.Groups["v"].Value;
                        continue;
                    }

                    regex = new Regex("发票号码：(?<v>\\d+)");
                    match = regex.Match(block.Text);
                    if (match.Success)
                    {
                        info.Code = match.Groups["v"].Value;
                        continue;
                    }

                    regex = new Regex("￥(?<v>\\d+\\.\\d{2})");
                    match = regex.Match(block.Text);
                    if (match.Success)
                    {
                        info.Money = match.Groups["v"].Value;
                        continue;
                    }

                    regex = new Regex("购\\s*名称：\\s*(?<v>[^\\n]+)");
                    match = regex.Match(block.Text);
                    if (match.Success)
                    {
                        info.Company = match.Groups["v"].Value;
                        continue;
                    }
                };

                return info.IsCompleted;
            }

        }
    }
}
