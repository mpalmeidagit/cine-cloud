using BuildingBlocks.Core.Mediator;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;

public record GetDvdResponse(
        string Id,
        string Title,
        string Genre,
        DateTime Published,
        int Copies,
        string DirectorId,
        DateTime CreatedAt,
        DateTime UpdatedAt
        ) : IResponse;