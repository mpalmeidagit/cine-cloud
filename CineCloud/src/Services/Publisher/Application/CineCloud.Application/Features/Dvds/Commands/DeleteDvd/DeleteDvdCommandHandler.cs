using CineCloud.Application.Contracts;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.DeleteDvd;

public class DeleteDvdCommandHandler : IRequestHandler<DeleteDvdCommand, DeleteDvdResponse>
{
    private readonly IDvdsWriteRepository _repository;

    public DeleteDvdCommandHandler(IDvdsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteDvdResponse> Handle(DeleteDvdCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return default;

        var dvd = await _repository.Get(request.Id);
        if (dvd is null)
            return default;

        dvd.DeleteDvd();

        var result = await _repository.Update(dvd);
        if (!result)
            return default;

        return new DeleteDvdResponse(dvd.Id.ToString(), (DateTime)dvd.DeletedAt);

    }
}