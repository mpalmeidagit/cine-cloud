using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.ReturnDvd;

public record ReturnDvdCommand(Guid Id) : ICommand, IRequest<ReturnDvdResponse>;