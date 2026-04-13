using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Fixtures.Unit;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public sealed class Fixture : UnitFixture
{
    private readonly string _demoProjectPath;
    private FixtureHost? _fixtureHost;
    private FixtureOrchestrator? _orchestrator;

    public string BaseUrl =>
        _orchestrator?.BaseUrl ?? throw new InvalidOperationException("Fixture has not been initialized.");

    public Fixture()
    {
        _demoProjectPath = ResolveDemoProjectPath();
    }

    public override async ValueTask InitializeAsync()
    {
        _fixtureHost = new FixtureHost();
        await _fixtureHost.Start();

        await base.InitializeAsync();

        _orchestrator = _fixtureHost.Services.GetRequiredService<FixtureOrchestrator>();
        await _orchestrator.Initialize(_demoProjectPath, CancellationToken.None);
    }

    public ValueTask<BrowserSession> CreateSession()
    {
        if (_orchestrator is null)
            throw new InvalidOperationException("Fixture has not been initialized.");

        return _orchestrator.CreateSession();
    }

    public override async ValueTask DisposeAsync()
    {
        Exception? exception = null;

        try
        {
            if (_orchestrator is not null)
                await _orchestrator.DisposeAsync();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        try
        {
            if (_fixtureHost is not null)
                await _fixtureHost.DisposeAsync();
        }
        catch (Exception ex) when (exception is null)
        {
            exception = ex;
        }

        if (exception is not null)
            throw exception;
    }

    private static string ResolveDemoProjectPath()
    {
        string suiteRoot = FindSuiteRoot();
        string demoProjectPath = Path.Combine(suiteRoot, "test", "Soenneker.Bradix.Suite.Demo", "Soenneker.Bradix.Suite.Demo.csproj");

        if (!File.Exists(demoProjectPath))
            throw new FileNotFoundException("Could not locate the Bradix demo project.", demoProjectPath);

        return demoProjectPath;
    }

    private static string FindSuiteRoot()
    {
        string[] startingPoints =
        [
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        ];

        foreach (string startingPoint in startingPoints)
        {
            DirectoryInfo? current = new(startingPoint);

            while (current is not null)
            {
                string candidate = Path.Combine(current.FullName, "Soenneker.Bradix.Suite.slnx");

                if (File.Exists(candidate))
                    return current.FullName;

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the Bradix suite root.");
    }
}