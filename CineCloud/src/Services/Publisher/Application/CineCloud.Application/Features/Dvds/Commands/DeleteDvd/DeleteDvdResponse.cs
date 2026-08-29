using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Dvds.Commands.DeleteDvd;

public record DeleteDvdResponse(string Id, DateTime DeletedAt) : IResponse;
