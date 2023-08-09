using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System;
using shutdownApi.Services;
using System.Runtime.CompilerServices;

namespace shutdownApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShutdownController : ControllerBase
{

    private readonly IConfiguration _config;
    private readonly IShutdownService _shutdownService;
    private readonly ILogger<ShutdownController> _logger;
    private readonly string? _apiKey;

    public ShutdownController(IConfiguration config, ILogger<ShutdownController> logger, IShutdownService shutdownService)
    {
        _config = config;
        _logger = logger;
        _shutdownService = shutdownService;
        _apiKey = config["ApiKey"];
    }

    [HttpGet(Name = "GetShutdown")]
    public IActionResult GetShutdown([FromHeader] string? xApiKey, [FromQuery] string? type)
    {
        _logger.LogInformation($"GetHalt Called type={type}");
        if (_apiKey is null)
        {
            _logger.LogInformation("No API Key configured, do nothing");
            return NoContent();
        }
        if (xApiKey != _apiKey)
        {
            _logger.LogInformation("Wrong API Key used");
            return BadRequest();
        }
        // Delay the Halt so client app get response
        new Thread(() =>
        {
            Thread.Sleep(2000);
            if (type == "halt")
                _shutdownService.Halt();
            else // Default is PowerOff
                _shutdownService.PowerOff();
        }).Start();

        return Accepted();

    }

}
