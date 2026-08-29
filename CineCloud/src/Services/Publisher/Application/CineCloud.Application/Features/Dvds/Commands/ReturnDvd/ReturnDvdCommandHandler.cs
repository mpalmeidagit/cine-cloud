using CineCloud.Application.Contracts;
using MediatR;

namespace CineCloud.Application.Features.Dvds.Commands.ReturnDvd;

public class ReturnDvdCommandHandler : IRequestHandler<ReturnDvdCommand, ReturnDvdResponse>
{
    private readonly IDvdsWriteRepository _repository;

    public ReturnDvdCommandHandler(IDvdsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReturnDvdResponse> Handle(ReturnDvdCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return default;

        var dvd = await _repository.Get(request.Id);
        if (dvd is null)
            return default;

        dvd.ReturnCopy();

        var result = await _repository.Update(dvd);
        if (!result)
            return default;

        return new ReturnDvdResponse(dvd.Id.ToString(), dvd.UpdatedAt);
    }
}