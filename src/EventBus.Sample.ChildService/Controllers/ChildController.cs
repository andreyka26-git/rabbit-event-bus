using Common;
using EventBus.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.Sample.ChildService.Controllers
{
    [Route("child")]
    [ApiController]
    public class ChildController : Controller
    {
        private readonly IEventBus _eventBus;

        public ChildController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // POST api/values
        [HttpPost("publish-message")]
        public void Post([FromBody] ChildModel model)
        {
            _eventBus.Publish(model, nameof(ChildModel));
        }
    }
}
