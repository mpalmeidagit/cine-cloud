using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.UpdateDvd;

public record UpdateDvdCommand(Guid Id,
                                   string Title,
                                   int Genre,
                                   DateTime Published,
                                   Guid DirectorId,
                                   int Copies) : ICommand, IRequest<UpdateDvdResponse>;