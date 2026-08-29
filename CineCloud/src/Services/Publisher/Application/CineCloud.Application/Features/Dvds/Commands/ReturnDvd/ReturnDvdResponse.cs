using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Dvds.Commands.ReturnDvd;

public record ReturnDvdResponse(string Id, DateTime UpdatedAt) : IResponse;