using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetAllDvds;

public class GetAllDvdsQueryHandler : IRequestHandler<GetAllDvdsQuery, GetAllDvdsResponse>
{
    private const int DEFAULT_PAGE_SIZE = 10;
    private const int MAX_PAGE_SIZE = 100;

    private readonly IDvdsQueryRepository _repository;

    public GetAllDvdsQueryHandler(IDvdsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllDvdsResponse> Handle(GetAllDvdsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > MAX_PAGE_SIZE ? DEFAULT_PAGE_SIZE : request.PageSize;

        var (dvds, totalCount) = await _repository.GetAll(page, pageSize);

        var items = dvds
            .Select(d => new GetDvdResponse(d.Id, d.Title, d.Genre, d.Published, d.Copies, d.DirectorId, d.CreatedAt, d.UpdatedAt))
            .ToList();

        return new GetAllDvdsResponse(items, page, pageSize, totalCount);
    }
}
