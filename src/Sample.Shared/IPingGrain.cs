namespace Sample;

[GenerateSerializer, Alias("ping")]
public record Ping(string Message);

[GenerateSerializer, Alias("pong")]
public record Pong(string Message);

public interface IPingGrain : IGrainWithStringKey
{
    Task<Pong> Ping(Ping request);
}