using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Domain.Models;
using CineCloud.Queries.Infrastructure.Context;
using MongoDB.Driver;

namespace CineCloud.Queries.Infrastructure.Repositories;

public class DvdsQueryRepository : IDvdsQueryRepository
{
    private readonly IMoviesRentalReadContext _context;

    public DvdsQueryRepository(IMoviesRentalReadContext context)
    {
        _context = context;
    }

    public async Task<Dvd> Create(Dvd entity)
    {
        await _context.Dvds.InsertOneAsync(entity); 

        return entity;
    }

    public async Task<bool> Delete(string id)
    {
        var result = await _context.Dvds.DeleteOneAsync(p => p.Id == id);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<Dvd> Get(string id) =>
        await _context.Dvds.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<Dvd> GetByTitle(string title) =>
        await _context
        .Dvds
        .Find(p => p.Title == title && p.Available)
        .FirstOrDefaultAsync();

    public async Task<bool> Update(Dvd entity)
    {
        var result = await _context.Dvds.ReplaceOneAsync(d => d.Id == entity.Id, entity);  

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}