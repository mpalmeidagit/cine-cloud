using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.DeleteDvd;

public record DeleteDvdCommand(string Id, DateTime DeletedAt) : ICommand, IRequest<bool>;