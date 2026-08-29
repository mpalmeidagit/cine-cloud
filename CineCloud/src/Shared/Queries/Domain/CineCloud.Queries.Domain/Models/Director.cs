using MongoDB.Bson.Serialization.Attributes;

namespace CineCloud.Queries.Domain.Models;

/// <summary>
/// Domínio de leitura não terá muitas regras de negócio pode ser anêmico, mas é importante manter a consistência com o banco de escrita.
/// </summary>
public class Director
{
    [BsonId]
    public string Id { get; set; }

    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}