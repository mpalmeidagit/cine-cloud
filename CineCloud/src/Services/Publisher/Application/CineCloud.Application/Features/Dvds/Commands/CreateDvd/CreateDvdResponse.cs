using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Dvds.Commands.CreateDvd;

public record CreateDvdResponse(
        string Id,
        string Title,
        string Genre,
        DateTime Published,
        bool Available,
        int Copies,
        string DirectorId,
        DateTime CreatedAt,
        DateTime UpdatedAt) : IResponse;