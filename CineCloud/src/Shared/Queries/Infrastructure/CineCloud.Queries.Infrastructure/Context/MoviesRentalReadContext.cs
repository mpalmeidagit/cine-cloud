using CineCloud.Queries.Domain.Models;
using CineCloud.Queries.Infrastructure.Settings;
using MongoDB.Driver;

namespace CineCloud.Queries.Infrastructure.Context;


public class MoviesRentalReadContext : IMoviesRentalReadContext
{
    public MoviesRentalReadContext(IMongoClient client, MongoDbSettings settings)
    {
        var database = client.GetDatabase(settings.DatabaseName);
        Dvds = database.GetCollection<Dvd>(settings.DvdsCollection);
        Directors = database.GetCollection<Director>(settings.DirectorsCollection);
    }
    public IMongoCollection<Dvd> Dvds { get; }

    public IMongoCollection<Director> Directors { get; }
    
}