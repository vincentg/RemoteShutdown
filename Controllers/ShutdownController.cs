using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System;

namespace shutdownApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShutdownController : ControllerBase
{

    private readonly IConfiguration _config;
    private readonly ILogger<ShutdownController> _logger;
    private readonly string? _apiKey;

    public ShutdownController(IConfiguration config,ILogger<ShutdownController> logger)
    {
	_config = config;
        _logger = logger;
	_apiKey = config["ApiKey"];
    }

    [HttpGet(Name = "GetShutdown")]
    public IActionResult Get([FromHeader] string? xApiKey)
    {
	_logger.LogInformation("GetShutdown Called");
	if (_apiKey is null) {
            _logger.LogInformation("No API Key configured, do nothing");
            return NoContent();
	}
	if (xApiKey != _apiKey) {
            _logger.LogInformation("Wrong API Key used");
	    return BadRequest();
	}
        // Delay the Halt so client app get response
	new Thread(() => {
	    Thread.Sleep(2000);
            using var process = Process.Start(
            new ProcessStartInfo
            {
                FileName = "/bin/sudo",
                ArgumentList = { "halt" }
            });
	}).Start();

	return Accepted();
    }
}
