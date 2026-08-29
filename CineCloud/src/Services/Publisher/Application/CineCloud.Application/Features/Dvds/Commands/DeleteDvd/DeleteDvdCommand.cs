using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.DeleteDvd;

public record DeleteDvdCommand(Guid Id) : ICommand, IRequest<DeleteDvdResponse>;