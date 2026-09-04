using CineCloud.Queries.Domain.Models;
using MongoDB.Driver;

namespace CineCloud.Queries.Infrastructure.Context;

public interface IMoviesRentalReadContext
{
    IMongoCollection<Dvd> Dvds { get; }
    IMongoCollection<Director> Directors { get; }
}