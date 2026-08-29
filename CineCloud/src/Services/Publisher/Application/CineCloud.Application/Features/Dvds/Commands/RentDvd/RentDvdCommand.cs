using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.RentDvd;

public record RentDvdCommand(Guid Id) : ICommand, IRequest<RentDvdResponse>;