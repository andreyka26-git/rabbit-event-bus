using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Infrastructure.Abstractions;
using EventBus.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.Sample.MainService.Controllers
{
    [Route("main")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public MainController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // POST api/values
        [HttpPost("publish message")]
        public void Post([FromBody] MainServiceModel model)
        {
            _eventBus.Publish(model);
        }
    }
}
