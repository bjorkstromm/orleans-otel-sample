using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryMetrics(metrics =>
{
    metrics
        .AddPrometheusExporter()
        .AddMeter("Microsoft.Orleans");
});

builder.Services.AddOpenTelemetryTracing(tracing =>
{
    tracing.SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService(serviceName: "sample-service"));

    tracing.AddAspNetCoreInstrumentation();
    tracing.AddSource("Microsoft.Orleans.Runtime");
    tracing.AddSource("Microsoft.Orleans.Application");
    tracing.AddSource("Sample");
    tracing.AddZipkinExporter();
});

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "sample-service";
        });

    siloBuilder.AddActivityPropagation();
});
var app = builder.Build();

app.MapGet("/{id}",
    async (
        [FromRoute] string id,
        [FromQuery] string? message,
        [FromServices] IGrainFactory grainFactory) =>
            await grainFactory
                .GetGrain<IPingGrain>(id)
                .Ping(new Ping(message ?? string.Empty)));

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
