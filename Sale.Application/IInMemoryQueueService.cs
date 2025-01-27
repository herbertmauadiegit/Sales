using Sales.Domain;

namespace Sales.Application
{
    public interface IInMemoryQueueService
    {
        void Enqueue(QueuePayload message);

        QueuePayload Dequeue();
    }
}
