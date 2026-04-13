using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public sealed class FixtureHost : IAsyncDisposable
{
    private readonly IHost _host;

    public FixtureHost()
    {
        _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((_, services) =>
                    {
                        Startup.ConfigureServices(services);
                    })
                    .Build();
    }

    public IServiceProvider Services => _host.Services;

    public async ValueTask Start()
    {
        await _host.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}