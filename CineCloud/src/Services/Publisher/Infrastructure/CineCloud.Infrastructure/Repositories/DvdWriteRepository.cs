using CineCloud.Application.Contracts;
using CineCloud.Domain.Entities;
using CineCloud.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CineCloud.Infrastructure.Repositories;

public class DvdWriteRepository(CineCloudWriteContext context) : IDvdsWriteRepository
{
    public async Task<bool> Create(Dvd entity)
    {
        await context.Dvds.AddAsync(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(Guid Id)
    {
        var rowsAffected = await context.Dvds.Where(d => d.Id == Id).ExecuteDeleteAsync();

        return rowsAffected > 0;
    }

    public async Task<Dvd> Get(Guid Id) =>
        await context.Dvds.FindAsync(Id);

    public async Task<bool> Update(Dvd entity)
    {
        context.Dvds.Update(entity);
        return await context.SaveChangesAsync() > 0;
    }
}