namespace Sales.Domain
{
    public class QueuePayload
    {
        public EventType EventType { get; set; }
        public Sale Sale { get; set; }
    }

    public enum EventType
    {
        Add = 0,
        Update = 1,
        Cancelation = 2,
        Get = 3
    }
}
