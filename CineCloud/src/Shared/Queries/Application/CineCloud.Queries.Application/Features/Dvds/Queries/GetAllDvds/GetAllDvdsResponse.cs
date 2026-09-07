using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetAllDvds;

public record GetAllDvdsResponse(
        IReadOnlyCollection<GetDvdResponse> Dvds,
        int Page,
        int PageSize,
        long TotalCount) : IResponse;
