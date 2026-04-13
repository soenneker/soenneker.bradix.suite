using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Playwrights.Installation.Abstract;
using Soenneker.Playwrights.Session;
using Soenneker.Utils.Delay;
using Soenneker.Utils.Dotnet.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;
using Soenneker.Utils.Network.Abstract;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public sealed class FixtureOrchestrator : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IDotnetUtil _dotnetUtil;
    private readonly INetworkUtil _networkUtil;
    private readonly IHttpClientCache _httpClientCache;
    private readonly IPlaywrightInstallationUtil _playwrightInstallationUtil;
    private readonly FixtureRuntime _runtime;
    private readonly ILogger<FixtureOrchestrator> _logger;
    
    private readonly StringBuilder _demoOutput = new();
    private readonly object _sync = new();

    public FixtureOrchestrator(IConfiguration configuration, IDotnetUtil dotnetUtil, INetworkUtil networkUtil, IHttpClientCache httpClientCache,
        IPlaywrightInstallationUtil playwrightInstallationUtil, FixtureRuntime runtime, ILogger<FixtureOrchestrator> logger)
    {
        _configuration = configuration;
        _dotnetUtil = dotnetUtil;
        _networkUtil = networkUtil;
        _httpClientCache = httpClientCache;
        _playwrightInstallationUtil = playwrightInstallationUtil;
        _runtime = runtime;
        _logger = logger;
    }

    public string BaseUrl => _runtime.BaseUrl;

    public async Task Initialize(string demoProjectPath, CancellationToken cancellationToken)
    {
        _runtime.BaseUrl = $"http://127.0.0.1:{_networkUtil.GetFreePort()}/";

        await _playwrightInstallationUtil.EnsureInstalled(cancellationToken);

        _runtime.Playwright = await Playwright.CreateAsync().NoSync();
        _runtime.Browser = await LaunchBrowser().NoSync();

        await StartDemo(demoProjectPath, cancellationToken);
    }

    public async ValueTask<BrowserSession> CreateSession()
    {
        if (_runtime.Browser is null)
            throw new InvalidOperationException("Browser has not been initialized.");

        IBrowserContext context = await _runtime.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = _runtime.BaseUrl
        }).NoSync();

        IPage page = await context.NewPageAsync().NoSync();

        return new BrowserSession(context, page);
    }

    private async ValueTask<IBrowser> LaunchBrowser()
    {
        if (_runtime.Playwright is null)
            throw new InvalidOperationException("Playwright has not been initialized.");

        string browser = _configuration["Playwright:Browser"]
                         ?.Trim()
                         .ToLowerInvariantFast() ?? "chromium";

        var options = new BrowserTypeLaunchOptions
        {
            Headless = true
        };

        return browser switch
        {
            "chromium" => await _runtime.Playwright.Chromium.LaunchAsync(options),
            "firefox" => await _runtime.Playwright.Firefox.LaunchAsync(options),
            "webkit" => await _runtime.Playwright.Webkit.LaunchAsync(options),
            _ => throw new InvalidOperationException($"Unsupported Playwright browser '{browser}'.")
        };
    }

    private async Task StartDemo(string demoProjectPath, CancellationToken cancellationToken)
    {
        string trimmedBaseUrl = _runtime.BaseUrl.TrimEnd('/');

        _logger.LogInformation("Starting Bradix demo from {ProjectPath} on {BaseUrl}", demoProjectPath, _runtime.BaseUrl);
        Console.WriteLine($"[fixture] Starting Bradix demo on {_runtime.BaseUrl}");

        _runtime.DemoProcess = await _dotnetUtil.Start(
            demoProjectPath,
            urls: trimmedBaseUrl,
            outputCallback: line => CaptureDemoOutput(line, isError: false),
            errorCallback: line => CaptureDemoOutput(line, isError: true),
            cancellationToken: cancellationToken).NoSync();

        if (_runtime.DemoProcess is null)
            throw new InvalidOperationException($"Failed to start Bradix demo project '{demoProjectPath}'.");

        await WaitForDemoReady(cancellationToken).NoSync();
    }

    private async ValueTask WaitForDemoReady(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            _logger.LogInformation("Waiting for Bradix demo readiness, attempt {Attempt}/10 at {BaseUrl}", attempt + 1, _runtime.BaseUrl);
            Console.WriteLine($"[fixture] Waiting for Bradix demo readiness ({attempt + 1}/10) at {_runtime.BaseUrl}");

            if (_runtime.DemoProcess is not null && _runtime.DemoProcess.HasExited)
            {
                throw new InvalidOperationException(
                    $"Bradix demo exited before becoming ready. Exit code: {_runtime.DemoProcess.ExitCode}{Environment.NewLine}{GetCapturedOutput()}");
            }

            try
            {
                HttpClient client = await _httpClientCache.Get(nameof(FixtureOrchestrator), cancellationToken: cancellationToken).NoSync();

                using HttpResponseMessage response = await client.GetAsync(_runtime.BaseUrl, cancellationToken).NoSync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Bradix demo is ready at {BaseUrl}", _runtime.BaseUrl);
                    Console.WriteLine($"[fixture] Bradix demo is ready at {_runtime.BaseUrl}");
                    return;
                }
            }
            catch
            {
            }

            await DelayUtil.Delay(1000, _logger, cancellationToken).NoSync();
        }

        throw new TimeoutException($"Timed out waiting for Bradix demo at {_runtime.BaseUrl}.{Environment.NewLine}{GetCapturedOutput()}");
    }

    private string GetCapturedOutput()
    {
        lock (_sync)
        {
            return _demoOutput.ToString();
        }
    }

    private void CaptureDemoOutput(string line, bool isError)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        string formatted = isError ? $"[demo:err] {line}" : $"[demo] {line}";

        lock (_sync)
        {
            _demoOutput.AppendLine(formatted);
        }

        if (isError)
            _logger.LogWarning("{Output}", formatted);
        else
            _logger.LogInformation("{Output}", formatted);

        Console.WriteLine(formatted);
    }

    public async ValueTask DisposeAsync()
    {
        Exception? exception = null;

        try
        {
            if (_runtime.Browser is not null)
                await _runtime.Browser.DisposeAsync().NoSync();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        try
        {
            _runtime.Playwright?.Dispose();
        }
        catch (Exception ex) when (exception is null)
        {
            exception = ex;
        }

        try
        {
            if (_runtime.DemoProcess is not null)
            {
                try
                {
                    if (!_runtime.DemoProcess.HasExited)
                        _runtime.DemoProcess.Kill(entireProcessTree: true);
                }
                catch (InvalidOperationException)
                {
                }

                _runtime.DemoProcess.Dispose();
                _runtime.DemoProcess = null;
            }
        }
        catch (Exception ex) when (exception is null)
        {
            exception = ex;
        }

        await _httpClientCache.Remove(nameof(FixtureOrchestrator)).NoSync();

        if (exception is not null)
            throw exception;
    }
}