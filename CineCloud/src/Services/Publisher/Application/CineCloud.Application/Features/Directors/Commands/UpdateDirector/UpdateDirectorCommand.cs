using MediatR;
using BuildingBlocks.Core.Mediator;

namespace CineCloud.Application.Features.Directors.Commands.UpdateDirector;

public record UpdateDirectorCommand(Guid Id, string Name, string Surname) : ICommand, IRequest<UpdateDirectorResponse>;