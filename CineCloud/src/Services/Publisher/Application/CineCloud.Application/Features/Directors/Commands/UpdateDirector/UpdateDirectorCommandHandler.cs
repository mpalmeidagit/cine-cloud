using CineCloud.Application.Contracts;
using MediatR;

namespace CineCloud.Application.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandler : IRequestHandler<UpdateDirectorCommand, UpdateDirectorResponse>
{
    private readonly IDirectorsWriteRepository _repository;

    public UpdateDirectorCommandHandler(IDirectorsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateDirectorResponse> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
    {
        var director = await _repository.Get(request.Id);
        if (director is null)
            return default;

        director.UpdateName(request.Name);
        director.UpdateSurname(request.Surname);

        var result = await _repository.Update(director);
        if (!result)
            return default;

        return new UpdateDirectorResponse(
            director.Id.ToString(), 
            director.FullName(),
            director.UpdatedAt
        );
    }
}