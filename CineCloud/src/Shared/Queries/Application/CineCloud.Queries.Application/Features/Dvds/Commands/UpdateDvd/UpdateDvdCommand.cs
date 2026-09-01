using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;

public record UpdateDvdCommand(
        string Id,
        string Title,
        string Genre,
        DateTime Published,
        int Copies,
        string DirectorId,
        DateTime UpdatedAt
        ) : ICommand, IRequest<bool>;