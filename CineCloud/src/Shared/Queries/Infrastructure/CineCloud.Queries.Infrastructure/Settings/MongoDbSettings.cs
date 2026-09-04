namespace CineCloud.Queries.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DirectorsCollection { get; set; } = string.Empty;
    public string DvdsCollection { get; set; } = string.Empty;
}