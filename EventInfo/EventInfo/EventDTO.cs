namespace EventInfo
{
    public class EventDTO
    {
        public String _Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int Device { get; set; }
        public float Battery { get; set; }
        public float Humidity { get; set; }
        public float Temperature { get; set; }

        public String Message { get; set; }
    }
}
