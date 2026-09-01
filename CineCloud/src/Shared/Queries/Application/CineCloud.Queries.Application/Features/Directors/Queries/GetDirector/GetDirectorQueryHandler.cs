using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;

public class GetDirectorQueryHandler : IRequestHandler<GetDirectorQuery, GetDirectorResponse>
{
    private readonly IDirectorsQueryRepository _repository;

    public GetDirectorQueryHandler(IDirectorsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDirectorResponse> Handle(GetDirectorQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FullName))
            return default;

        var director = await _repository.GetByName(request.FullName);
        if (director is null)
            return default;

        return new GetDirectorResponse(director.Id, director.FullName);
    }
}