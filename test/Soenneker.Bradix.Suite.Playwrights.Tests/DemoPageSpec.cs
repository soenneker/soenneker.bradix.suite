using System;
using Microsoft.Playwright;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

internal sealed record DemoPageSpec(string Route, string Title, string Heading, string Description, Func<IPage, ILocator> ReadyLocator);
