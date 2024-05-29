namespace EventInfo
{
    public class EventInfoHistory
    {
        private static EventInfoHistory instance;
        private static readonly object lockObject = new object();
        private List<EventDTO> events;

        private EventInfoHistory()
        {
            events = new List<EventDTO>();
        }

        public static EventInfoHistory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new EventInfoHistory();
                        }
                    }
                }
                return instance;
            }
        }

        public void AddEvent(EventDTO eventDTO)
        {
            events.Add(eventDTO);
        }

        public List<EventDTO> GetAllEvents()
        {
            return events;
        }

        public EventDTO GetLastEvent()
        {
            if (events.Count > 0)
            {
                return events[events.Count - 1];
            }
            return null;
        }

        public List<EventDTO> GetLastNEvents(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("n mora biti pozitivno.");
            }
            int startIndex = Math.Max(0, events.Count - n);
            int count = Math.Min(n, events.Count);
            return events.GetRange(startIndex, count);
        }
        public EventDTO GetLastEventWithId(int id)
        {
            return events.LastOrDefault(e => e.Device == id);
        }
    }
}
