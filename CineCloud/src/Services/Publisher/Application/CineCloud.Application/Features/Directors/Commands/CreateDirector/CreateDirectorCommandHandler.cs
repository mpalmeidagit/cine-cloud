using CineCloud.Application.Contracts;
using CineCloud.Domain.Entities;
using MediatR;

namespace CineCloud.Application.Features.Directors.Commands.CreateDirector;

public class CreateDirectorCommandHandler : IRequestHandler<CreateDirectorCommand, CreateDirectorResponse>
{
    private readonly IDirectorsWriteRepository _repository;

    public CreateDirectorCommandHandler(IDirectorsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateDirectorResponse> Handle(CreateDirectorCommand request, CancellationToken cancellationToken)
    {
        var director = new Director(
            request.Name,
            request.Surname
        );

        var result = await _repository.Create(director);
        if (!result)
            return default;

        return new CreateDirectorResponse(
            director.Id.ToString(),
            director.FullName(),
            director.CreatedAt,
            director.UpdatedAt
        );
    }
}