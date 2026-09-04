namespace BuildingBlocks.Core.EventBus.Events;

public record DvdUpdatedEvent(string Id,
        string Title,
        string Genre,
        DateTime Published,
        int Copies,
        string DirectorId,
        DateTime UpdatedAt);