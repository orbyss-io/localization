using System.Net;
using CShells;
using Microsoft.AspNetCore.Authentication;
using ModelContextProtocol.Client;
using Orbyss.Foundation.Mcp.AspNetCore;
using Orbyss.Localization;
using Orbyss.Localization.Formats;
using Orbyss.Localization.Management.Mcp.AspNetCore;
using Orbyss.Localization.Runtime.Mcp.AspNetCore;

var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.Services.AddAuthentication("probe").AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("probe", _ => { });
builder.Services.AddAuthorization();

var settings = new ShellSettings(
    new ShellId("localization-management"),
    [
        "Orbyss.Localization.Formats",
        "Orbyss.Localization.Management",
        "Orbyss.Localization.Runtime",
        "Orbyss.Localization.Storage.InMemory",
        "Orbyss.Foundation.Mcp.AspNetCore",
        "Orbyss.Localization.Management.Mcp.AspNetCore",
        "Orbyss.Localization.Runtime.Mcp.AspNetCore"
    ]);
var formats = new OrbyssLocalizationFormatsFeature();
var management = new OrbyssLocalizationManagementFeature();
var runtime = new OrbyssLocalizationRuntimeFeature(settings);
var storage = new OrbyssLocalizationInMemoryStorageFeature();
var mcp = new FoundationMcpFeature(settings);
var managementTools = new OrbyssLocalizationManagementMcpFeature(settings);
var runtimeTools = new OrbyssLocalizationRuntimeMcpFeature();
formats.ConfigureServices(builder.Services);
management.ConfigureServices(builder.Services);
runtime.ConfigureServices(builder.Services);
storage.ConfigureServices(builder.Services);
mcp.ConfigureServices(builder.Services);
managementTools.ConfigureServices(builder.Services);
runtimeTools.ConfigureServices(builder.Services);

await using var app = builder.Build();
Require(app.Services.GetRequiredService<ILocalizationRuntime>() is DefaultLocalizationRuntime, "runtime feature did not register the default release-backed runtime");
app.Urls.Add("http://127.0.0.1:0");
app.UseAuthentication();
app.UseAuthorization();
mcp.MapEndpoints(app, app.Environment);
await app.StartAsync();
try
{
    using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
    using var anonymous = await client.PostAsync(
        "/orbyss-foundation/mcp",
        new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
    Require(anonymous.StatusCode == HttpStatusCode.Unauthorized, "shared MCP allowed anonymous access");

    var transport = new HttpClientTransport(new HttpClientTransportOptions
    {
        Name = "Orbyss Localization management probe",
        Endpoint = new Uri(new Uri(app.Urls.Single()), "/orbyss-foundation/mcp"),
        TransportMode = HttpTransportMode.StreamableHttp,
        EnableStandaloneGetStream = false,
        AdditionalHeaders = new Dictionary<string, string> { ["X-Test-Auth"] = "true" }
    });
    await using var clientMcp = await McpClient.CreateAsync(transport);
    var catalog = await clientMcp.ListToolsAsync();
    Require(catalog.Count == 16, $"expected 16 localization tools, found {catalog.Count}");
    Require(catalog.Any(tool => tool.Name == "localization.catalogs.get"), "catalog get tool is missing");
    Require(catalog.Count(tool => tool.Name.StartsWith("localization.runtime.", StringComparison.Ordinal)) == 2, "runtime tools were not independently contributed");
    var call = await clientMcp.CallToolAsync(
        "localization.catalogs.list",
        new Dictionary<string, object?> { ["search"] = null, ["first"] = 0, ["maximum"] = 10 });
    Require(call.IsError != true, "authenticated localization MCP call failed");
}
finally
{
    await app.StopAsync();
}

Console.WriteLine("Orbyss Localization split management/runtime MCP probe passed.");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
