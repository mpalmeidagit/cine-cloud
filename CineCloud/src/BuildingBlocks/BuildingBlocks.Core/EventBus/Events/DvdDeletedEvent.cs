namespace BuildingBlocks.Core.EventBus.Events;

public record DvdDeletedEvent(string Id, DateTime DeletedAt);