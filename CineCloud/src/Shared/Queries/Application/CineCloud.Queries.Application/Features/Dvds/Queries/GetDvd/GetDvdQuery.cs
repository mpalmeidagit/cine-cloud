using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;

public record GetDvdQuery(string Title) : IQuery, IRequest<GetDvdResponse>;