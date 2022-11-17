namespace Sample.Server;

public class PingGrain : Grain, IPingGrain
{
    public Task<Pong> Ping(Ping request) =>
        Task.FromResult(new Pong(request.Message));
}