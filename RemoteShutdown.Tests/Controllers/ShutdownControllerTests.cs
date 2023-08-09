using Xunit;
using shutdownApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using shutdownApi.Services;

namespace shutdownApi.Controllers.Tests
{
    public class ShutdownControllerTests
    {

        class DummyShutdownImpl : IShutdownService
        {
            public int haltCount { get; set; }
            public int powerOffCount { get; set; }
            public DummyShutdownImpl() { haltCount = 0; powerOffCount = 0; }
            public void Halt() { haltCount++; }
            public void PowerOff() { powerOffCount++; }
        }


        static readonly ILogger<ShutdownController> logger = new NullLogger<ShutdownController>();

        [Fact()]
        public void ShutdownControllerWrongKeyTests()
        {
            var inMemorySettings = new Dictionary<string, string?> {
               {"ApiKey", "SuperSecretAPIKEYMock"},};

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var shutdownController = new ShutdownController(configuration, logger,
                new DummyShutdownImpl());
            // Test wrong key
            var result = shutdownController.GetShutdown("TEST", null).GetType();
            Assert.Equal(result, new BadRequestResult().GetType());
        }

        [Fact()]
        public void ShutdownControllerNoKeyTests()
        {
            // Test no Key 
            IConfiguration empty_configuration = new ConfigurationBuilder().Build();
            var shutdownController = new ShutdownController(empty_configuration, logger,
                new DummyShutdownImpl());

            var result = shutdownController.GetShutdown("TEST", null).GetType();
            Assert.Equal(result, new NoContentResult().GetType());

        }

        [Fact()]
        public void GetHaltTest()
        {
            var inMemorySettings = new Dictionary<string, string?> {
               {"ApiKey", "SuperSecretAPIKEYMock"},};
            IConfiguration configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();
            DummyShutdownImpl shutdownService = new DummyShutdownImpl();
            var shutdownController = new ShutdownController(configuration, logger, shutdownService);
            var result = shutdownController.GetShutdown("SuperSecretAPIKEYMock","halt").GetType();
            Assert.Equal(result, new AcceptedResult().GetType());
            // Execution is delayed
            Assert.Equal(0, shutdownService.powerOffCount);
            Assert.Equal(0, shutdownService.haltCount);
            Thread.Sleep(3000);
            Assert.Equal(0, shutdownService.powerOffCount);
            Assert.Equal(1, shutdownService.haltCount);

        }

        [Fact()]
        public void GetPowerOffTest()
        {
            var inMemorySettings = new Dictionary<string, string?> {
               {"ApiKey", "SuperSecretAPIKEYMock"},};
            IConfiguration configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();
            DummyShutdownImpl shutdownService = new DummyShutdownImpl();
            var shutdownController = new ShutdownController(configuration, logger, shutdownService);
            var result = shutdownController.GetShutdown("SuperSecretAPIKEYMock","whatever").GetType();
            Assert.Equal(result, new AcceptedResult().GetType());
            Assert.Equal(0, shutdownService.powerOffCount);
            Assert.Equal(0, shutdownService.haltCount);
            Thread.Sleep(3000);
            Assert.Equal(1, shutdownService.powerOffCount);
            Assert.Equal(0, shutdownService.haltCount);
        }
    }
}