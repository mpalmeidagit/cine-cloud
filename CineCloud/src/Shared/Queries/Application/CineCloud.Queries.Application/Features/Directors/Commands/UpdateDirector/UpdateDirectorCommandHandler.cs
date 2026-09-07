using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandler : IRequestHandler<UpdateDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository;

    public UpdateDirectorCommandHandler(IDirectorsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
    {
        var director = await _repository.Get(request.Id);
        if (director is null)
            return false;

        director.FullName = request.FullName;
        director.UpdatedAt = request.UpdatedAt;

        return await _repository.Update(director);
    }
}