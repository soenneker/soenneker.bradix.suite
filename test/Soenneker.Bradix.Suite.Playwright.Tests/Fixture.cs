using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Serilog;
using Soenneker.Fixtures.Unit;
using Soenneker.Playwrights.Installation.Abstract;
using Soenneker.Playwrights.Installation.Registrars;
using Soenneker.Utils.Test;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Soenneker.Playwrights.Session;
using Soenneker.Utils.Network.Abstract;
using Soenneker.Utils.Network.Registrars;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

public sealed class Fixture : UnitFixture
{
    private readonly HttpClient _httpClient = new();
    private readonly StringBuilder _demoOutput = new();
    private readonly object _sync = new();
    private readonly string _demoProjectPath;

    private Process? _demoProcess;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public string BaseUrl { get; set;  }

    public Fixture()
    {

        _demoProjectPath = ResolveDemoProjectPath();
    }

    public override async ValueTask InitializeAsync()
    {
        SetupIoC(Services);

        await base.InitializeAsync();

        if (ServiceProvider is null)
            throw new InvalidOperationException("Service provider was not initialized.");

        var networkUtil = ServiceProvider.GetRequiredService<INetworkUtil>();

        BaseUrl = $"http://127.0.0.1:{networkUtil.GetFreePort()}/";

        var playwrightInstallationUtil = ServiceProvider.GetRequiredService<IPlaywrightInstallationUtil>();

        await playwrightInstallationUtil.EnsureInstalled();

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await LaunchBrowser(ServiceProvider.GetRequiredService<IConfiguration>());

        await StartDemo();
    }

    private static void SetupIoC(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: false);
        });

        services.AddPlaywrightInstallationUtilAsSingleton();
        services.AddNetworkUtilAsSingleton();

        IConfiguration config = TestUtil.BuildConfig();
        services.AddSingleton(config);
    }

    private async Task<IBrowser> LaunchBrowser(IConfiguration configuration)
    {
        if (_playwright is null)
            throw new InvalidOperationException("Playwright has not been initialized.");

        string browser = configuration["Playwright:Browser"]?.Trim().ToLowerInvariant() ?? "chromium";

        var options = new BrowserTypeLaunchOptions
        {
            Headless = true
        };

        return browser switch
        {
            "chromium" => await _playwright.Chromium.LaunchAsync(options),
            "firefox" => await _playwright.Firefox.LaunchAsync(options),
            "webkit" => await _playwright.Webkit.LaunchAsync(options),
            _ => throw new InvalidOperationException($"Unsupported Playwright browser '{browser}'.")
        };
    }

    public async Task<BrowserSession> CreateSession()
    {
        if (_browser is null)
            throw new InvalidOperationException("Browser has not been initialized.");

        IBrowserContext context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl
        });

        IPage page = await context.NewPageAsync();

        return new BrowserSession(context, page);
    }

    public override async ValueTask DisposeAsync()
    {
        Exception? disposalException = null;

        try
        {
            if (_browser is not null)
                await _browser.DisposeAsync();
        }
        catch (Exception ex)
        {
            disposalException = ex;
        }

        try
        {
            _playwright?.Dispose();
        }
        catch (Exception ex) when (disposalException is null)
        {
            disposalException = ex;
        }

        try
        {
            if (_demoProcess is not null)
            {
                if (!_demoProcess.HasExited)
                    _demoProcess.Kill(entireProcessTree: true);

                _demoProcess.Dispose();
            }
        }
        catch (Exception ex) when (disposalException is null)
        {
            disposalException = ex;
        }

        if (disposalException is not null)
            throw disposalException;
    }

    private async Task StartDemo()
    {
        string trimmedBaseUrl = BaseUrl.TrimEnd('/');

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{_demoProjectPath}\" --urls {trimmedBaseUrl}",
            WorkingDirectory = Path.GetDirectoryName(_demoProjectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _demoProcess = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        _demoProcess.OutputDataReceived += (_, args) => AppendDemoOutput(args.Data);
        _demoProcess.ErrorDataReceived += (_, args) => AppendDemoOutput(args.Data);

        if (!_demoProcess.Start())
            throw new InvalidOperationException("Failed to start Bradix demo process.");

        _demoProcess.BeginOutputReadLine();
        _demoProcess.BeginErrorReadLine();

        await WaitForDemoReady();
    }

    private async Task WaitForDemoReady()
    {
        for (var attempt = 0; attempt < 120; attempt++)
        {
            if (_demoProcess is not null && _demoProcess.HasExited)
            {
                throw new InvalidOperationException(
                    $"Bradix demo exited before becoming ready. Exit code: {_demoProcess.ExitCode}{Environment.NewLine}{GetCapturedOutput()}");
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

                if (response.IsSuccessStatusCode)
                    return;
            }
            catch
            {
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException($"Timed out waiting for Bradix demo at {BaseUrl}.{Environment.NewLine}{GetCapturedOutput()}");
    }

    private void AppendDemoOutput(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return;

        lock (_sync)
        {
            _demoOutput.AppendLine(data);
        }
    }

    private string GetCapturedOutput()
    {
        lock (_sync)
        {
            return _demoOutput.ToString();
        }
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