namespace BuildingBlocks.Core.EventBus.Events;

public record DvdCreatedEvent(string Id,
        string Title,
        string Genre,
        DateTime Published,
        bool Available,
        int Copies,
        string DirectorId,
        DateTime CreatedAt,
        DateTime UpdatedAt);