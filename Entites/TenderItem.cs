namespace ZakupkiParser.Entites;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

public class TenderItem
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("code")]
    public string Code { get; set; }

    [BsonElement("type")]
    public string Type { get; set; }

    [BsonElement("unit")]
    public string Unit { get; set; }

    [BsonElement("unitPrice")]
    public string UnitPrice { get; set; }

    [BsonElement("quantity")]
    public string Quantity { get; set; }

    [BsonElement("totalPrice")]
    public string TotalPrice { get; set; }

    [BsonElement("characteristics")]
    public List<TenderItemCharacteristic> Characteristics { get; set; } = new();
}