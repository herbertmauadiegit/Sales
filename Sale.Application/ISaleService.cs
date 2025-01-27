using Sales.Domain;

namespace Sales.Application
{
    public interface ISaleService
    {
        Task<Sale> CreateSale(SaleRequest saleReq);
        Task<Sale> GetSale(int saleNumber);
        Task<Sale> UpdateSale(int saleNumber, UpdateSaleRequest saleReq);
        Task CancelSale(int saleNumber);
        Task<QueuePayload> DeQueue();
    }
}
