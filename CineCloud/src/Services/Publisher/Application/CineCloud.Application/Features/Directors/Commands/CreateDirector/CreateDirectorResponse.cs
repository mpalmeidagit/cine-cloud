using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Directors.Commands.CreateDirector;

public record CreateDirectorResponse(string Id,
        string FullName,
        DateTime CreatedAt,
        DateTime UpdatedAt) : IResponse;