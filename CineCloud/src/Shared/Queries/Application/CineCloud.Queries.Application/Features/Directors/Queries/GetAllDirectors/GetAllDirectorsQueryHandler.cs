using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetAllDirectors;

public class GetAllDirectorsQueryHandler : IRequestHandler<GetAllDirectorsQuery, GetAllDirectorsResponse>
{
    private const int DEFAULT_PAGE_SIZE = 10;
    private const int MAX_PAGE_SIZE = 100;

    private readonly IDirectorsQueryRepository _repository;

    public GetAllDirectorsQueryHandler(IDirectorsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllDirectorsResponse> Handle(GetAllDirectorsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > MAX_PAGE_SIZE ? DEFAULT_PAGE_SIZE : request.PageSize;

        var (directors, totalCount) = await _repository.GetAll(page, pageSize);

        var items = directors
            .Select(d => new GetDirectorResponse(d.Id, d.FullName))
            .ToList();

        return new GetAllDirectorsResponse(items, page, pageSize, totalCount);
    }
}
