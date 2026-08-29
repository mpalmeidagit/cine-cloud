using MediatR;
using BuildingBlocks.Core.Mediator;


namespace CineCloud.Application.Features.Directors.Commands.CreateDirector;

public record CreateDirectorCommand(string Name, string Surname) : ICommand, IRequest<CreateDirectorResponse>;
