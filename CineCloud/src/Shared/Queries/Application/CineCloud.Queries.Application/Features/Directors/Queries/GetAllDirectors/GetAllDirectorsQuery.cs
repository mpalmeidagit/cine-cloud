using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetAllDirectors;

public record GetAllDirectorsQuery(int Page, int PageSize) : IQuery, IRequest<GetAllDirectorsResponse>;
