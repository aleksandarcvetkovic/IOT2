namespace EventInfo
{
    public class AnalyticsStore
    {
        private static Dictionary<int, AnalyticsDTO> analytics;
        private static readonly object lockObject = new object();
        private static AnalyticsStore instance;

        private AnalyticsStore()
        {
            analytics = new Dictionary<int, AnalyticsDTO>();
        }

        public static AnalyticsStore Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new AnalyticsStore();
                        }
                    }
                }
                return instance;
            }
        }
        // Add analytics to the store
        public void AddAnalytics(AnalyticsDTO analyticsDTO)
        {
            //if key exists, update the value
            if (analytics.ContainsKey(analyticsDTO.Device))
            {
                analytics[analyticsDTO.Device] = analyticsDTO;
                return;
            }
            analytics.Add(analyticsDTO.Device, analyticsDTO);
        }

        // Get all analytics
        public List<AnalyticsDTO> GetAllAnalytics()
        {
            return analytics.Values.ToList();
        }

        // Get analytics by id
        public AnalyticsDTO GetAnalyticsById(int id)
        {
            if (analytics.ContainsKey(id))
            {
                return analytics[id];
            }
            return null;
        }
    }
}
