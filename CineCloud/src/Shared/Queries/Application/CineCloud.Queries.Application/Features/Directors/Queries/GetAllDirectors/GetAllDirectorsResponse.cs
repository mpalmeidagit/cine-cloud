using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetAllDirectors;

public record GetAllDirectorsResponse(
        IReadOnlyCollection<GetDirectorResponse> Directors,
        int Page,
        int PageSize,
        long TotalCount) : IResponse;
