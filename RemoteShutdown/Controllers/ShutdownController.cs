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

    [HttpGet(Name = "GetHalt")]
    public IActionResult GetHalt([FromHeader] string? xApiKey)
    {
        _logger.LogInformation("GetHalt Called");
        void haltAction() { _shutdownService.Halt(); }

        return ExecuteCommand(xApiKey, haltAction);
    }

    [HttpGet(Name = "GetPowerOff")]
    public IActionResult GetPowerOff([FromHeader] string? xApiKey)
    {
        _logger.LogInformation("GetPowerOff Called");
        void powerOffAction() { _shutdownService.PowerOff(); }

        return ExecuteCommand(xApiKey, powerOffAction);
    }

    private IActionResult ExecuteCommand(string? xApiKey, Action shutdownAction)
    {
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
            shutdownAction();
        }).Start();
           
        return Accepted();
    }

}
