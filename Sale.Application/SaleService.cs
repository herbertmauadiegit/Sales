using Microsoft.EntityFrameworkCore;
using Sales.Domain;
using Sales.Infra;

namespace Sales.Application
{
    public class SaleService(IInMemoryQueueService _queue, SaleDbContext _dbContext) : ISaleService
    {
        #region Private

        private string GenerateRandomSaleNumber()
        {

            var number = GenerateRandomNumber();
            while (_dbContext.Sales.Any(x => x.SaleNumber == int.Parse(number)))
            {
                number = GenerateRandomNumber();
            }

            return number;
        }

        private string GenerateRandomNumber()
        {
            Random random = new Random();
            string result = "";

            for (int i = 0; i < 5; i++)
            {
                result += random.Next(0, 10).ToString();
            }

            return result;
        }

        private void ApplyDiscounts(List<SaleItem> items)
        {
            foreach (var item in items)
            {
                if (item.Quantity > 20)
                {
                    throw new InvalidOperationException($"Cannot sell more than 20 identical items. (${item.Product})");
                }

                if (item.Quantity < 4)
                {
                    item.Discount = 0;
                }
                else if (item.Quantity >= 4 && item.Quantity <= 9)
                {
                    item.Discount = 0.10m;
                }
                else if (item.Quantity >= 10 && item.Quantity <= 20)
                {
                    item.Discount = 0.20m;
                }
            }
        }

        #endregion

        #region Public

        public async Task<Sale> CreateSale(SaleRequest saleReq)
        {

            var sale = new Sale()
            {
                Branch = saleReq.Branch,
                Customer = saleReq.Customer,
                IsCancelled = false,
                SaleDate = DateTime.Now,
                SaleNumber = int.Parse(GenerateRandomSaleNumber())
            };

            List<SaleItem> items = saleReq.Items.Select(x => new SaleItem()
            {
                Product = x.Product,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
            }).ToList();

            ApplyDiscounts(items);
            sale.TotalSaleAmount = items.Sum(x => x.TotalAmount);
            await _dbContext.Sales.AddAsync(sale);
            items.ForEach(x => x.SaleId = sale.Id);
            await _dbContext.SaleItems.AddRangeAsync(items);
            await _dbContext.SaveChangesAsync();
            _queue.Enqueue(new QueuePayload() { EventType = EventType.Add, Sale = sale });
            return sale;
        }

        public async Task<Sale> GetSale(int saleNumber)
        {
            var sale = await _dbContext.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
            if (sale == null)
                throw new Exception("Sale not found for sale number:" + saleNumber);
            _queue.Enqueue(new QueuePayload() { EventType = EventType.Get, Sale = sale });
            return sale;
        }   

        public async Task<Sale> UpdateSale(int saleNumber, UpdateSaleRequest saleReq)
        {
            var sale = await _dbContext.Sales.FirstAsync(x=>x.SaleNumber == saleNumber);
            if (sale == null)
                throw new Exception("Sale not found for sale number:" + saleNumber);
            sale.Branch = saleReq.Branch;
            sale.Customer = saleReq.Customer;
            

            List<SaleItem> items = saleReq.Items.Select(x => new SaleItem()
            {
                Product = x.Product,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
            }).ToList();

            ApplyDiscounts(items);
            sale.TotalSaleAmount = items.Sum(x => x.TotalAmount);
            _dbContext.Sales.Update(sale);
            items.ForEach(x => x.SaleId = sale.Id);
            _dbContext.SaleItems.RemoveRange(_dbContext.SaleItems.Where(x => x.Sale.SaleNumber == saleNumber).AsEnumerable());
            await _dbContext.SaleItems.AddRangeAsync(items);
            await _dbContext.SaveChangesAsync();
            _queue.Enqueue(new QueuePayload() { EventType = EventType.Update, Sale = sale });
            return sale;
        }

        public async Task CancelSale(int saleNumber)
        {

            var sale = await _dbContext.Sales.FirstOrDefaultAsync(x => x.SaleNumber == saleNumber);
            if (sale == null)
                throw new Exception("Sale not found for sale number:" + saleNumber);
            sale.IsCancelled = true;

            await _dbContext.SaveChangesAsync();
            _queue.Enqueue(new QueuePayload() { EventType = EventType.Cancelation, Sale = sale });
        }

        public async Task<QueuePayload> DeQueue()
        {

           return _queue.Dequeue();
        }

        #endregion
    }
}
