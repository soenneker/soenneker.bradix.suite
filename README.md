[![](https://img.shields.io/nuget/v/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.bradix.suite)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Bradix.Suite
### Radix-style UI primitives, built for Blazor.

## Installation

```bash
dotnet add package Soenneker.Bradix.Suite
```

## Setup

Register services in `Program.cs`:

```csharp
builder.Services.AddBradixComponentAsScoped();
```

Inject the higher-level utility where you need it:

```csharp
@inject IBradixComponent Suite
```

## Usage

Initialize the package once before first use:

```csharp
await Suite.Initialize();
```
