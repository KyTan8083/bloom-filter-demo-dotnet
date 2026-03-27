using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
//using BloomFilterDemo.Models;
using TestClient.Models;

namespace TestClient.Services
{
    public sealed class BloomApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BloomApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddAsync(string item, CancellationToken cancellationToken = default)
        {
            var request = new AddItemRequest
            {
                Item = item
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/bloom/add",
                request,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(
                    $"Add API failed. Status={(int)response.StatusCode}, Body={body}");
            }

            Console.WriteLine($"[ADD OK] {item}");
            Console.WriteLine(body);
            Console.WriteLine();
        }

        public async Task<LookupResult?> CheckAsync(string item, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(
                $"api/bloom/check?item={Uri.EscapeDataString(item)}",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(
                    $"Check API failed. Status={(int)response.StatusCode}, Body={body}");
            }

            var result = JsonSerializer.Deserialize<LookupResult>(body, _jsonOptions);

            Console.WriteLine($"[CHECK] {item}");
            Console.WriteLine(body);
            Console.WriteLine();

            return result;
        }

        public async Task<BloomMetricsSnapshot?> GetMetricsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(
                "api/bloom/metrics",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(
                    $"Metrics API failed. Status={(int)response.StatusCode}, Body={body}");
            }

            var result = JsonSerializer.Deserialize<BloomMetricsSnapshot>(body, _jsonOptions);

            Console.WriteLine("[METRICS]");
            Console.WriteLine(body);
            Console.WriteLine();

            return result;
        }

    }
}
