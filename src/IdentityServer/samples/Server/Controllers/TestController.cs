using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdOps.IdentityServer.Samples.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly IEventService _eventService;

        public TestController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Test()
        {
            await _eventService.RaiseAsync(new TestEvent());

            return Ok();
        }
    }

    public class TestEvent : Event
    {
        public TestEvent()
            : base("Test", "MyName", EventTypes.Information, 9999, "Just testing")
        {
            Foo = "Bar";
            Things = new string[] { "A", "B" };
        }

        public string Foo { get; set; }

        public string[] Things { get; set; }

        public bool Fa { get; set; }

        public bool Tr { get; set; } = true;

        public int In { get; set; } = 42;

        public double Dou { get; set; } = 42.7;
    }
}

