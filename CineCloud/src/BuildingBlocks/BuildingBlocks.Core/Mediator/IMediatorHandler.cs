namespace BuildingBlocks.Core.Mediator;

public interface IMediatorHandler
{
    Task<IResponse> SendCommand<TC>(TC command, CancellationToken token = default) where TC : ICommand;
    Task<bool> SendCommandAndReturnBool<TC>(TC command, CancellationToken token = default) where TC : ICommand;
    Task<IResponse> SendQuery<TQ>(TQ query, CancellationToken token = default) where TQ : IQuery;
}