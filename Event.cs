namespace rabbithole
{
    public class Event
    {
        public GUID id { get; set; }

        public DateTime timestamp { get; set; }

        public string event_name { get; set; }
        
        [Column(TypeName="jsonb")]
        public string content { get; set; }
    }
}