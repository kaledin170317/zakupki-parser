using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ZakupkiParser;


using ClosedXML.Excel;
class Program
{
    
    public static List<string> LoadTenderIdsFromExcel(string filePath)
    {
        var tenderIds = new List<string>();
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1); // первая вкладка
        var rows = worksheet.RangeUsed().RowsUsed();

        foreach (var row in rows)
        {
            var cellValue = row.Cell(1).GetValue<string>().Trim();
            if (!string.IsNullOrEmpty(cellValue))
                tenderIds.Add(cellValue);
        }

        return tenderIds;
    }
    
    static async Task Main()
    {
        var tenderIds = LoadTenderIdsFromExcel("C:\\Users\\kaled\\RiderProjects\\ZakupkiParser\\ZakupkiParser\\tenders.xlsx");

        var parser = new ParserHTML(new HttpClient());
        await parser.DownloadAndSaveHtml(tenderIds);

        var files = await parser.LoadFileContentsAsync();

        var parseTender = new ParseTender();
        var mongoService = new MongoService();

        foreach (var pair in files)
        {
            Console.WriteLine($"Обработка {pair.Key}...");
            try
            {
                var tender = parseTender.ParseHtml(pair.Value).Result;
                await mongoService.SaveTenderAsync(tender);
                Console.WriteLine($"Сохранено: {tender.RegNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке {pair.Key}: {ex.Message}");
            }
        }

        Console.WriteLine("Готово");
    }
}