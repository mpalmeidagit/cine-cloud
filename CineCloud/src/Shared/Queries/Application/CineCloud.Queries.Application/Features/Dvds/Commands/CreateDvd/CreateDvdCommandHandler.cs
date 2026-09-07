using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Domain.Models;
using MediatR;

namespace CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;


public class CreateDvdCommandHandler : IRequestHandler<CreateDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository;

    public CreateDvdCommandHandler(IDvdsQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CreateDvdCommand request, CancellationToken cancellationToken)
    {
        var dvd = await _repository.Get(request.Id);
        if (dvd is not null)
            return false;

        dvd = new Dvd
        {
            Id = request.Id,
            Title = request.Title,
            Genre = request.Genre,
            Published = request.Published,
            Available = request.Available,
            Copies = request.Copies,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            DirectorId = request.DirectorId
        };

        var result = await _repository.Create(dvd);
        if (result is null)
            return false;


        return true;
    }

}