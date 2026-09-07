using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Domain.Models;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;

public class CreateDirectorCommandHandler : IRequestHandler<CreateDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository;

    public CreateDirectorCommandHandler(IDirectorsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CreateDirectorCommand request, CancellationToken cancellationToken)
    {
        var director = await _repository.Get(request.Id);
        if (director is not null)
            return false;

        director = new Director { Id = request.Id, FullName = request.FullName, CreatedAt = request.CreatedAt, UpdatedAt = request.UpdatedAt };

        var result = await _repository.Create(director);
        if (result is null)
            return false;

        return true;
    }
}