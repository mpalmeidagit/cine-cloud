using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandler : IRequestHandler<UpdateDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository;

    public UpdateDvdCommandHandler(IDvdsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateDvdCommand request, CancellationToken cancellationToken)
    {
        var dvd = await _repository.Get(request.Id);
        if (dvd is null)
            return false;

        dvd.Title = request.Title;
        dvd.Genre = request.Genre;
        dvd.Published = request.Published;
        dvd.Copies = request.Copies;
        dvd.DirectorId = request.DirectorId;
        dvd.UpdatedAt = request.UpdatedAt;

        return await _repository.Update(dvd);
    }
}