using Microsoft.AspNetCore.Mvc;

namespace EventInfo
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventInfoController : ControllerBase
    {
        private EventInfoHistory eventInfoHistory;

        public EventInfoController()
        {
            eventInfoHistory = EventInfoHistory.Instance;
        }

        [HttpGet("GetAllEvents")]
        public ActionResult<List<EventDTO>> GetAllEvents()
        {
            return eventInfoHistory.GetAllEvents();
        }

        [HttpGet("GetLastEvent")]
        public ActionResult<EventDTO> GetLastEvent()
        {
            return eventInfoHistory.GetLastEvent();
        }

        [HttpGet("GetLastNEvents/{n}")]
        public ActionResult<List<EventDTO>> GetLastNEvents(int n)
        {
            return eventInfoHistory.GetLastNEvents(n);
        }
        [HttpGet("GetLastEventWithId/{id}")]
        public ActionResult<EventDTO> GetLastEventWithId(int id)
        {
            return eventInfoHistory.GetLastEventWithId(id);
        }
    }
}
