using CineCloud.Queries.Domain.Models;

namespace CineCloud.Queries.Application.Contracts;

public interface IDirectorsQueryRepository : IQueryRepository<Director>
{
    Task<Director> GetByName(string name);
}