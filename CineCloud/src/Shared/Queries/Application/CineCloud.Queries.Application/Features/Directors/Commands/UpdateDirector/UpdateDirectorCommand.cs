using BuildingBlocks.Core.Mediator;
using MediatR;


namespace CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;

public record UpdateDirectorCommand(
        string Id,
        string FullName,
        DateTime UpdatedAt) : ICommand, IRequest<bool>;