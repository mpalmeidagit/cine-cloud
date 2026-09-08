using CineCloud.Application.Contracts;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandler : IRequestHandler<UpdateDvdCommand, UpdateDvdResponse>
{
    private readonly IDvdsWriteRepository _repository;

    public UpdateDvdCommandHandler(IDvdsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateDvdResponse> Handle(UpdateDvdCommand request, CancellationToken cancellationToken)
    {
        var dvd = await _repository.Get(request.Id);
        if (dvd is null)
            return default;

        dvd.UpdateTitle(request.Title);
        dvd.UpdateCopies(request.Copies);
        dvd.UpdatePublishedDate(request.Published);
        dvd.UpdateGenre(request.Genre);
        dvd.UpdateDirector(request.DirectorId);

        var result = await _repository.Update(dvd);
        if (!result)
            return default;

        return new UpdateDvdResponse(dvd.Id.ToString(),
            dvd.Title,
            dvd.Genre.ToString(),
            dvd.Published,
            dvd.Copies,
            dvd.DirectorId.ToString(),
            dvd.UpdatedAt);

    }
}