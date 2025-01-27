using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Sales.Application;
using Sales.Domain;
using Sales.Infra;

namespace Sales.Tests
{
    [TestFixture]
    public class SaleServiceTests
    {
        private SaleDbContext _dbContext;
        private ISaleService _saleService;
        private IInMemoryQueueService _queueService;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();
            var options = new DbContextOptionsBuilder<SaleDbContext>()
                .UseInMemoryDatabase("SaleDbTest")
                .Options;

            _dbContext = new SaleDbContext(options);
            _queueService = new InMemoryQueueService();
            _saleService = new SaleService(_queueService, _dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Should_Create_Sale()
        {
         
            var sale = new SaleRequest
            {
                Customer = "CustomerTest1",
                Branch = "BranchTest1",
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest { Product = "ProductTest1", Quantity = 15, UnitPrice = 105 },
                    new SaleItemRequest { Product = "ProductTest2", Quantity = 5, UnitPrice = 110 }
                }
            };

            var createdSale = await _saleService.CreateSale(sale);

            ClassicAssert.NotNull(createdSale);
            ClassicAssert.AreEqual(2, createdSale.Items.Count);
            ClassicAssert.AreEqual(15, createdSale.Items[0].Quantity);
        }

        [Test]
        public async Task Should_Apply_10Percent_Discount_For_Quantity_5()
        {
            var sale = new SaleRequest
            {
                Customer = "Customer2",
                Branch = "Branch2",
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest { Product = "Product1", Quantity = 5, UnitPrice = 100 }
                }
            };

            var createdSale = await _saleService.CreateSale(sale);

         
            ClassicAssert.AreEqual(0.10m, createdSale.Items[0].Discount); 
            ClassicAssert.AreEqual(450, createdSale.Items[0].TotalAmount);
        }

        [Test]
        public async Task Should_Throw_Exception_For_Quantity_Above_20Async()
        {
            var sale = new SaleRequest
            {
                Customer = "Customer3",
                Branch = "Branch3",
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest { Product = "Product1", Quantity = 21, UnitPrice = 100 }
                }
            };

            ClassicAssert.ThrowsAsync<InvalidOperationException>(()=> _saleService.CreateSale(sale));
        }

        [Test]
        public async Task Should_Apply_20Percent_Discount_For_Quantity_15()
        {
            var sale = new SaleRequest
            {
                Customer = "Customer4",
                Branch = "Branch4",
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest { Product = "Product1", Quantity = 15, UnitPrice = 100 }
                }
            };

            var createdSale = await _saleService.CreateSale(sale);

            ClassicAssert.AreEqual(0.20m, createdSale.Items[0].Discount);
            ClassicAssert.AreEqual(1200, createdSale.Items[0].TotalAmount);
        }
    }
}
