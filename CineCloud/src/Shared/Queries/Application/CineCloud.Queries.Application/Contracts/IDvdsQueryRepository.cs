using CineCloud.Queries.Domain.Models;

namespace CineCloud.Queries.Application.Contracts;

public interface IDvdsQueryRepository : IQueryRepository<Dvd>
{
    Task<Dvd> GetByTitle(string title);
}