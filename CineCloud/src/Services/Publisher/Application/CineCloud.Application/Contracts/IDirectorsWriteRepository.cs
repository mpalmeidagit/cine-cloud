using CineCloud.Domain.Entities;

namespace CineCloud.Application.Contracts;

public interface IDirectorsWriteRepository : IWriteRepository<Director>
{
    Task<Director> GetDirectorWithMovies(Guid Id);
}