using BloomFilterDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestClient.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

string baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                 ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");

builder.Services.AddHttpClient<BloomApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

using var host = builder.Build();

var apiClient = host.Services.GetRequiredService<BloomApiClient>();

Console.WriteLine("=== Bloom Filter API Test Client ===");
Console.WriteLine($"Base URL: {baseUrl}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Add item");
    Console.WriteLine("2. Check item");
    Console.WriteLine("3. View metrics");
    Console.WriteLine("4. Run sample test batch");
    Console.WriteLine("0. Exit");
    Console.Write("Select: ");

    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1":
                Console.Write("Enter item to add: ");
                var addItem = Console.ReadLine() ?? string.Empty;
                await apiClient.AddAsync(addItem);
                break;

            case "2":
                Console.Write("Enter item to check: ");
                var checkItem = Console.ReadLine() ?? string.Empty;
                await apiClient.CheckAsync(checkItem);
                break;

            case "3":
                await apiClient.GetMetricsAsync();
                break;

            case "4":
                var testItems = new[]
                {
                    "user:1001",
                    "user:1002",
                    "unknown:aaa",
                    "unknown:bbb",
                    "order:A001",
                    "order:A999",
                    "email:test@example.com"
                };

                foreach (var item in testItems)
                {
                    await apiClient.CheckAsync(item);
                }

                await apiClient.GetMetricsAsync();
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}