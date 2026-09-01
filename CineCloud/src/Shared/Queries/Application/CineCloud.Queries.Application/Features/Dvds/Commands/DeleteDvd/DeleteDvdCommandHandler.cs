using CineCloud.Queries.Application.Contracts;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.DeleteDvd;

public class DeleteDvdCommandHandler : IRequestHandler<DeleteDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository;

    public DeleteDvdCommandHandler(IDvdsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteDvdCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id) || request.DeletedAt > DateTime.Now)
            return false;

        var dvd = await _repository.Get(request.Id);
        if (dvd is null)
            return false;

        dvd.Available = false;
        dvd.DeletedAt = request.DeletedAt;
        dvd.Copies = 0;

        return await _repository.Update(dvd);
    }
}