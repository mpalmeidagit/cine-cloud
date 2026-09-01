using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;

public record CreateDirectorCommand(
        string Id,
        string FullName,
        DateTime CreatedAt,
        DateTime UpdatedAt) : ICommand, IRequest<bool>;