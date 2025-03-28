using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class MongoService
{
    private readonly IMongoCollection<Tender> _tenders;

    public MongoService(string connectionString = "mongodb://localhost:55555", string dbName = "Zakupki", string collectionName = "Tenders")
    {
        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            _tenders = database.GetCollection<Tender>(collectionName);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка подключения к MongoDB: " + ex.Message, ex);
        }
    }

    public async Task SaveTenderAsync(Tender tender)
    {
      
        Console.WriteLine($"Сохраняем закупку: {tender.RegNumber} — {tender.PurchaseName}");
        if (tender == null)
            throw new ArgumentNullException(nameof(tender), "Tender не может быть null");

        if (string.IsNullOrWhiteSpace(tender.RegNumber))
            throw new ArgumentException("RegNumber не может быть пустым", nameof(tender.RegNumber));

        try
        {
           var result = _tenders.ReplaceOne(
               filter: Builders<Tender>.Filter.Eq(t => t.RegNumber, tender.RegNumber),
               replacement: tender,
                options: new ReplaceOptions { IsUpsert = true }
               );
            Console.WriteLine(result);
            if (!result.IsAcknowledged)
                throw new Exception($"MongoDB не подтвердила сохранение закупки {tender.RegNumber}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при сохранении закупки {tender.RegNumber} в MongoDB: {ex.Message}", ex);
        }
    }
}
