using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetAllDvds;

public record GetAllDvdsQuery(int Page, int PageSize) : IQuery, IRequest<GetAllDvdsResponse>;
