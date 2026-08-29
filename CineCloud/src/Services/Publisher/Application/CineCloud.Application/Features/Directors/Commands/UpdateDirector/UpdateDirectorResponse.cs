using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Directors.Commands.UpdateDirector;

public record UpdateDirectorResponse(string Id,
       string FullName,
       DateTime UpdatedAt) : IResponse;