using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZakupkiParser.Entites;

public class Tender
{
    [BsonElement("regNumber")]
    public string RegNumber { get; set; }

    [BsonElement("purchaseName")]
    public string PurchaseName { get; set; }

    [BsonElement("purchaseMethod")]
    public string PurchaseMethod { get; set; }

    [BsonElement("platformName")]
    public string PlatformName { get; set; }

    [BsonElement("platformUrl")]
    public string PlatformUrl { get; set; }

    [BsonElement("organizationName")]
    public string OrganizationName { get; set; }

    [BsonElement("postalAddress")]
    public string PostalAddress { get; set; }

    [BsonElement("location")]
    public string Location { get; set; }

    [BsonElement("contactPerson")]
    public string ContactPerson { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("phone")]
    public string Phone { get; set; }

    [BsonElement("fax")]
    public string Fax { get; set; }

    [BsonElement("additionalInfo")]
    public string AdditionalInfo { get; set; }

    [BsonElement("applicationDeadline")]
    public string ApplicationDeadline { get; set; }

    [BsonElement("resultsDate")]
    public string ResultsDate { get; set; }

    [BsonElement("contractPrice")]
    public string ContractPrice { get; set; }

    [BsonElement("startDate")]
    public string StartDate { get; set; }

    [BsonElement("endDate")]
    public string EndDate { get; set; }

    [BsonElement("budgetFunds")]
    public string BudgetFunds { get; set; }

    [BsonElement("ownFunds")]
    public string OwnFunds { get; set; }

    [BsonElement("downloadedAt")]
    public string DownloadedAt { get; set; } = DateTime.UtcNow.ToString("s");
    
    [BsonElement("items")]
    public List<TenderItem> Items { get; set; } = new();
}