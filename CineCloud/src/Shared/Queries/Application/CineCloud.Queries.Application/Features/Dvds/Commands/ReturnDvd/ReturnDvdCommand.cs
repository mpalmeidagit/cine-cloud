using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.ReturnDvd;

public record ReturnDvdCommand(string Id, DateTime UpdatedAt) : ICommand, IRequest<bool>;