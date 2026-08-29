using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Dvds.Commands.RentDvd;

public record RentDvdResponse(string Id, DateTime UpdatedAt) : IResponse;