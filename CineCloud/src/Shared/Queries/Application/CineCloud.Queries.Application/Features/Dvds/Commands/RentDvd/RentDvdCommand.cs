using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.RentDvd;

public record RentDvdCommand(string Id, DateTime UpdatedAt) : ICommand, IRequest<bool>;