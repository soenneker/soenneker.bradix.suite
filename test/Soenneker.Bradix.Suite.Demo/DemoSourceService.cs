using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Demo;

// Read the same Razor files used to render previews so examples cannot drift from their source.
public sealed class DemoSourceService(HttpClient http)
{
    private readonly Dictionary<string, Task<string>> _sources = new(StringComparer.Ordinal);

    public async Task<string> GetExample(string file, int index)
    {
        if (!_sources.TryGetValue(file, out Task<string>? load))
            _sources[file] = load = http.GetStringAsync($"_sources/{file}.txt");

        string source;
        try { source = (await load).Replace("\r\n", "\n", StringComparison.Ordinal); }
        catch { _sources.Remove(file); throw; }

        MatchCollection examples = Regex.Matches(source, @"<DemoExample\b[^>]*>(.*?)</DemoExample>", RegexOptions.Singleline);
        if (index < 0 || index >= examples.Count)
            throw new InvalidOperationException($"Example {index} is missing from {file}.");

        string[] lines = examples[index].Groups[1].Value.Trim('\n', '\r').Split('\n');
        int indent = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Min(line => line.Length - line.TrimStart().Length);
        string markup = string.Join('\n', lines.Select(line => line.Length >= indent ? line[indent..] : line)).Trim();
        string imports = string.Join('\n', source.Split('\n').Where(line => line.StartsWith("@using ", StringComparison.Ordinal)));
        int codeStart = source.IndexOf("\n@code", StringComparison.Ordinal);
        string code = codeStart >= 0 ? source[codeStart..].TrimEnd() : "";
        return $"@using Soenneker.Bradix\n{imports}\n{markup}\n{code}".Trim();
    }
}
