using System.Net;
using CShells;
using Microsoft.AspNetCore.Authentication;
using ModelContextProtocol.Client;
using Orbyss.Foundation.Mcp.AspNetCore;
using Orbyss.Localization;
using Orbyss.Localization.Formats;
using Orbyss.Localization.Mcp.AspNetCore;

var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.Services.AddAuthentication("probe").AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("probe", _ => { });
builder.Services.AddAuthorization();

var settings = new ShellSettings(
    new ShellId("localization-management"),
    [
        "Orbyss.Localization",
        "Orbyss.Localization.Formats",
        "Orbyss.Localization.Application",
        "Orbyss.Localization.Storage.InMemory",
        "Orbyss.Foundation.Mcp.AspNetCore",
        "Orbyss.Localization.Mcp.AspNetCore"
    ]);
var defaults = new OrbyssLocalizationFeature();
var formats = new OrbyssLocalizationFormatsFeature();
var application = new OrbyssLocalizationApplicationFeature();
var storage = new OrbyssLocalizationInMemoryStorageFeature();
var mcp = new FoundationMcpFeature(settings);
var tools = new OrbyssLocalizationMcpFeature(settings);
defaults.ConfigureServices(builder.Services);
formats.ConfigureServices(builder.Services);
application.ConfigureServices(builder.Services);
storage.ConfigureServices(builder.Services);
mcp.ConfigureServices(builder.Services);
tools.ConfigureServices(builder.Services);

await using var app = builder.Build();
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
    var call = await clientMcp.CallToolAsync(
        "localization.catalogs.list",
        new Dictionary<string, object?> { ["search"] = null, ["first"] = 0, ["maximum"] = 10 });
    Require(call.IsError != true, "authenticated localization MCP call failed");
}
finally
{
    await app.StopAsync();
}

Console.WriteLine("Orbyss Localization shared MCP management probe passed.");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
