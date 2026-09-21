# Bradix demo

The documentation shell uses the published Quark Suite package. Examples continue to render the Bradix project in this repository.

Run with `dotnet run --project test/Soenneker.Bradix.Suite.Demo` from the repository root. The launch profile uses `http://localhost:7039`. Stop the server after testing.

## Examples

Wrap each preview in `DemoExample`, specifying its Razor file and zero-based example index. The build copies the page sources into `wwwroot/_sources`; `DemoSourceService` extracts the matching preview and the page's supporting `@code` block. Quark's `DemoSection` supplies the expandable code editor and clipboard action. Do not maintain a second copy of the example markup.

The Tailwind generators run during the demo build and use the manifest from the pinned Quark package. Node/npm must be available for CSS generation, as in the Quark Suite demo.

## Cloudflare AI Search

- Account: `08469810296dac20f6efdab76fc98637`
- Namespace / instance: `default` / `soenneker-bradix-suite`
- Source: `https://bradix.soenneker.com`, rendered sitemap crawl, every six hours.
- Content selector: `*` → `main`, excluding navigation from search content.
- Metadata schema: `title` and `description`, both text. `MainLayout` emits the matching page-specific meta tags.
- Public endpoint: configured in `wwwroot/appsettings.json`; it is a public URL, not a credential.
- Only the search endpoint is enabled; chat completions and MCP are disabled.
- Rate limit: 120 requests per 60 seconds, fixed window.
- Authorized hosts: `bradix.soenneker.com`, `http://localhost:7039`, `http://localhost:7041` (browser verification).

After deploying the demo, run Sync in the Cloudflare instance or allow the scheduled crawl to pick up the new page metadata and examples. The old deployed pages do not provide title/description metadata, so existing search results may have missing titles until that crawl completes. Endpoint setting changes can take a few minutes to propagate.

Cloudflare's [snippet documentation](https://developers.cloudflare.com/ai-search/configuration/retrieval/public-endpoint/embed-search-snippets/) describes the full local-origin format. CORS restrictions are browser controls, not authentication.
