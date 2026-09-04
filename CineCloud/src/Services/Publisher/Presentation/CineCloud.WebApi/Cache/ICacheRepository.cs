using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;

namespace CineCloud.WebApi.Cache;

public interface ICacheRepository
{
    Task<GetDvdResponse> Get(string title);
    Task Update(GetDvdResponse response);
}