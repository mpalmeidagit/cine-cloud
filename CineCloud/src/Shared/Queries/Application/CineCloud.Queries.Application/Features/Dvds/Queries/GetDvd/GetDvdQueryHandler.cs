using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;

public class GetDvdQueryHandler : IRequestHandler<GetDvdQuery, GetDvdResponse>
{
    private readonly IDvdsQueryRepository _repository;

    public GetDvdQueryHandler(IDvdsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDvdResponse> Handle(GetDvdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Title))
            return default;

        var dvd = await _repository.GetByTitle(request.Title);
        if (dvd is null)
            return default;

        return new GetDvdResponse(dvd.Id, dvd.Title, dvd.Genre, dvd.Published, dvd.Copies, dvd.DirectorId, dvd.CreatedAt, dvd.UpdatedAt);
    }
}