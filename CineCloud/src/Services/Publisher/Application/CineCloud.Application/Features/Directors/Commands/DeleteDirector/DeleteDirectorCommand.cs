using BuildingBlocks.Core.Mediator;
using MediatR;


namespace CineCloud.Application.Features.Directors.Commands.DeleteDirector;

public record DeleteDirectorCommand(Guid Id) : ICommand, IRequest<bool>;