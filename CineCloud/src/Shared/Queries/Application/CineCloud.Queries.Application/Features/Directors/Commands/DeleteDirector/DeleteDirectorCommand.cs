using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Commands.DeleteDirector;

public record DeleteDirectorCommand(string Id) : ICommand, IRequest<bool>;