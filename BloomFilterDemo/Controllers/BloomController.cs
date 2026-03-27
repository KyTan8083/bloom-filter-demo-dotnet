using BloomFilterDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloomFilterDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloomController : ControllerBase
    {
        private readonly BloomService _bloomService;
        private readonly BloomMetrics _metrics;

        public BloomController(BloomService bloomService, BloomMetrics metrics)
        {
            _bloomService = bloomService;
            _metrics = metrics;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddItemRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Item))
            {
                return BadRequest(new { error = "Item is required." });
            }

            await _bloomService.AddAsync(request.Item, cancellationToken);
            return Ok(new { message = "Inserted.", item = request.Item });
        }

        [HttpGet("check")]
        public async Task<IActionResult> Check([FromQuery] string item, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return BadRequest(new { error = "Item is required." });
            }

            var result = await _bloomService.CheckAsync(item, cancellationToken);
            return Ok(result);
        }

        [HttpGet("metrics")]
        public IActionResult Metrics()
        {
            return Ok(_metrics.Snapshot());
        }
    }

    public sealed class AddItemRequest
    {
        public string Item { get; set; } = string.Empty;
    }

}
