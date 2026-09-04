using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Domain.Models;
using CineCloud.Queries.Infrastructure.Context;
using MongoDB.Driver;

namespace CineCloud.Queries.Infrastructure.Repositories;

public class DirectorsQueryRepository : IDirectorsQueryRepository
{
    private readonly IMoviesRentalReadContext _context;

    public DirectorsQueryRepository(IMoviesRentalReadContext context)
    {
        _context = context;
    }

    public async Task<Director> Create(Director entity)
    {
        await _context.Directors.InsertOneAsync(entity);
        return entity;
    }

    public async Task<bool> Delete(string id)
    {
        var result = await _context.Directors.DeleteOneAsync(d => d.Id == id);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<Director> Get(string id) =>
        await _context.Directors.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<Director> GetByName(string name) =>
        await _context.Directors.Find(p => p.FullName == name).FirstOrDefaultAsync();

    public async Task<bool> Update(Director entity)
    {
        var result = await _context.Directors.ReplaceOneAsync(d => d.Id == entity.Id, entity);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

}