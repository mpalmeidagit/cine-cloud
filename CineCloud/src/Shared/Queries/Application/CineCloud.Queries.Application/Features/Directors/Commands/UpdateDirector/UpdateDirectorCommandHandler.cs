using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandler : IRequestHandler<UpdateDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository;
    private readonly UpdateDirectorCommandValidator _validator;

    public UpdateDirectorCommandHandler(IDirectorsQueryRepository repository, UpdateDirectorCommandValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return false;

        var director = await _repository.Get(request.Id);
        if (director is null)
            return false;

        director.FullName = request.FullName;
        director.UpdatedAt = request.UpdatedAt;

        return await _repository.Update(director);
    }
}