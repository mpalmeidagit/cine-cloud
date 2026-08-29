using MediatR;
using BuildingBlocks.Core.Mediator;


namespace CineCloud.Application.Features.Dvds.Commands.CreateDvd;

public record CreateDvdCommand(
        string Title,
        int Genre,
        DateTime Published,
        int Copies,
        Guid DirectorId) : ICommand, IRequest<CreateDvdResponse>;