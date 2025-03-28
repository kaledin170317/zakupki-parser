using System.Collections.Concurrent;
using HtmlAgilityPack;
using ZakupkiParser.Entites;

namespace ZakupkiParser;

public class ParseTender
{
    public ParseTender()
    {
    }

    private Tender SetKeyToTender(Tender tender, ConcurrentDictionary<string, string> columns)
{
    columns.TryGetValue("Номер извещения", out var regNumber);
    tender.RegNumber = regNumber;

    
    columns.TryGetValue("Наименование объекта закупки", out var purchaseName);
    tender.PurchaseName = purchaseName;

    columns.TryGetValue("Способ определения поставщика (подрядчика, исполнителя)", out var purchaseMethod);
    tender.PurchaseMethod = purchaseMethod;

    columns.TryGetValue("Наименование электронной площадки в информационно-телекоммуникационной сети «Интернет»", out var platformName);
    tender.PlatformName = platformName;

    columns.TryGetValue("Адрес электронной площадки в информационно-телекоммуникационной сети «Интернет»", out var platformUrl);
    tender.PlatformUrl = platformUrl;

    columns.TryGetValue("Размещение осуществляет", out var orgName);
    tender.OrganizationName = orgName;

    columns.TryGetValue("Почтовый адрес", out var postalAddress);
    tender.PostalAddress = postalAddress;

    columns.TryGetValue("Место нахождения", out var location);
    tender.Location = location;

    columns.TryGetValue("Ответственное должностное лицо", out var contactPerson);
    tender.ContactPerson = contactPerson;

    columns.TryGetValue("Адрес электронной почты", out var email);
    tender.Email = email;

    columns.TryGetValue("Номер контактного телефона", out var phone);
    tender.Phone = phone;

    columns.TryGetValue("Факс", out var fax);
    tender.Fax = fax;

    columns.TryGetValue("Дополнительная информация", out var additionalInfo);
    tender.AdditionalInfo = additionalInfo;

    columns.TryGetValue("Дата и время окончания подачи заявок", out var applicationDeadline);
    tender.ApplicationDeadline = applicationDeadline;

    columns.TryGetValue("Дата подведения итогов определения поставщика (подрядчика, исполнителя)", out var resultsDate);
    tender.ResultsDate = resultsDate;

    columns.TryGetValue("Начальная (максимальная) цена контракта", out var contractPrice);
    tender.ContractPrice = contractPrice;

    columns.TryGetValue("Дата начала исполнения контракта", out var startDate);
    tender.StartDate = startDate;

    columns.TryGetValue("Срок исполнения контракта", out var endDate);
    tender.EndDate = endDate;

    columns.TryGetValue("Закупка за счет бюджетных средств", out var budgetFunds);
    tender.BudgetFunds = budgetFunds;

    columns.TryGetValue("Закупка за счет собственных средств организации", out var ownFunds);
    tender.OwnFunds = ownFunds;

    tender.DownloadedAt = DateTime.UtcNow.ToString("s");

    return tender;
}


    private Task ParseRow(Tender tender, HtmlNode row, ConcurrentDictionary<string, string> columns)
    {
        return Task.Run((() =>
        {
            var keyNode = row.SelectSingleNode(".//p[@class='parameter']");
            var valueNode = row.SelectSingleNode(".//p[@class='parameterValue']");
            if (keyNode != null && valueNode != null)
            {
                var key = keyNode.InnerText.Trim();
                var value = valueNode.InnerText.Trim();
                columns.TryAdd(key, value);
            }
        }));
    }

    public async Task<Tender> ParseHtml(string html)
    {
        var tender = new Tender();
        var tasks = new List<Task>();
        var columns = new ConcurrentDictionary<string, string>();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rows = doc.DocumentNode.SelectNodes("//tr");

        foreach (var row in rows)
        {
            tasks.Add(ParseRow(tender, row, columns));
        }
        
        await Task.WhenAll(tasks);
        var items = ParseTenderItems(doc);
        tender.Items = items;
        
        return SetKeyToTender(tender, columns);
    }
    
    public List<TenderItem> ParseTenderItems(HtmlDocument doc)
{
    var items = new List<TenderItem>();
    var tables = doc.DocumentNode.SelectNodes("//table[contains(@class, 'table') and contains(@class, 'font14')]");
    if (tables == null) return items;

    TenderItem currentItem = null;

    foreach (var table in tables)
    {
        var header = table.SelectSingleNode(".//tr/th[1]")?.InnerText.Trim();

        if (header?.Contains("Наименование товара") == true)
        {
            // Таблица с товаром
            var row = table.SelectSingleNode(".//tr[2]");
            var cells = row?.SelectNodes("td");
            if (cells == null || cells.Count < 7) continue;

            currentItem = new TenderItem
            {
                Name = cells[0]?.InnerText.Trim().Replace("\n", " "),
                Code = cells[1]?.InnerText.Trim(),
                Type = cells[2]?.InnerText.Trim(),
                Unit = cells[3]?.InnerText.Trim(),
                UnitPrice = cells[4]?.InnerText.Trim(),
                Quantity = cells[5]?.InnerText.Trim(),
                TotalPrice = cells[6]?.InnerText.Trim()
            };

            items.Add(currentItem);
        }
        else if (header?.Contains("Характеристики товара") == true && currentItem != null)
        {
            var rows = table.SelectNodes(".//tr[position() > 2]");
            if (rows == null) continue;

            string lastName = null;
            string lastInstruction = null;
            string lastUnit = null;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null) continue;

                string name = null;
                string value = null;
                string unit = null;
                string instruction = null;

                if (cells.Count == 4)
                {
                    name = cells[0].InnerText.Trim();
                    value = cells[1].InnerText.Trim();
                    unit = cells[2].InnerText.Trim();
                    instruction = cells[3].InnerText.Trim();

                    lastName = name;
                    lastUnit = unit;
                    lastInstruction = instruction;

                    currentItem.Characteristics.Add(new TenderItemCharacteristic
                    {
                        Name = name,
                        Value = value,
                        Unit = unit,
                        Instruction = instruction
                    });
                }
                else if (cells.Count == 1 && lastName != null)
                {
                    value = cells[0].InnerText.Trim();

                    currentItem.Characteristics.Add(new TenderItemCharacteristic
                    {
                        Name = lastName,
                        Value = value,
                        Unit = lastUnit,
                        Instruction = lastInstruction
                    });
                }
                else if (cells.Count == 2 && lastName != null)
                {
                    value = cells[0].InnerText.Trim();
                    instruction = cells[1].InnerText.Trim();

                    currentItem.Characteristics.Add(new TenderItemCharacteristic
                    {
                        Name = lastName,
                        Value = value,
                        Unit = lastUnit,
                        Instruction = instruction
                    });
                }
            }
        }
    }

    return items;
}

}