using Sales.Domain;

namespace Sales.Application
{
    public class InMemoryQueueService : IInMemoryQueueService
    {
        private Queue<QueuePayload> _queue = new Queue<QueuePayload>();

        public void Enqueue(QueuePayload message)
        {
            _queue.Enqueue(message);
            Console.WriteLine($"Message Enqueued: {message.Sale.SaleNumber} - Action: {message.EventType}");
        }

        public QueuePayload Dequeue()
        {
            if (_queue.Count > 0)
            {
                var message = _queue.Dequeue();
                Console.WriteLine($"Message Dequeued: {message.Sale.SaleNumber} - Action: {message.EventType}");
                return message;
            }
            return null;
        }
    }
}
