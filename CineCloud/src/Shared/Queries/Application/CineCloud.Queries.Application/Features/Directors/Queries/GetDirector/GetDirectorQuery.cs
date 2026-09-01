using BuildingBlocks.Core.Mediator;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;

public record GetDirectorQuery(string FullName) : IQuery, IRequest<GetDirectorResponse>;