namespace ZakupkiParser.Entites;

using MongoDB.Bson.Serialization.Attributes;

public class TenderItemCharacteristic
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("value")]
    public string Value { get; set; }

    [BsonElement("unit")]
    public string Unit { get; set; }

    [BsonElement("instruction")]
    public string Instruction { get; set; }
}
