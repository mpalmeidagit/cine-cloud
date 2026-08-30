using CineCloud.Application.Contracts;
using CineCloud.Domain.Entities;
using CineCloud.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CineCloud.Infrastructure.Repositories;

public class DirectorWriteRepository(CineCloudWriteContext context) : IDirectorsWriteRepository
{
    public async Task<bool> Create(Director entity)
    {
        await context.Directors.AddAsync(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(Guid Id)
    {
        var rowsAffected = await context.Directors
                                .Where(d => d.Id == Id)
                                .ExecuteDeleteAsync();
        return rowsAffected > 0;
    }

    public async Task<Director> Get(Guid Id) =>
        await context.Directors.FindAsync(Id);

    public async Task<Director> GetDirectorWithMovies(Guid Id) =>
        await context.Directors.AsNoTracking()
                                .Include(d => d.Dvds)
                                .Where(d => d.Id == Id)
                                .FirstOrDefaultAsync();

    public async Task<bool> Update(Director entity)
    {
        context.Directors.Update(entity);
        return await context.SaveChangesAsync() > 0;
    }
}