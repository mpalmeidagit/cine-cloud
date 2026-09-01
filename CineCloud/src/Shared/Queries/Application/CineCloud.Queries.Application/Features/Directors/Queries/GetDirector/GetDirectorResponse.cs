using BuildingBlocks.Core.Mediator;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;

public record GetDirectorResponse(string Id, string FullName) : IResponse;