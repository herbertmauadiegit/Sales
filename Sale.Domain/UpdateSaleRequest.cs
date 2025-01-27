namespace Sales.Domain
{
    public class UpdateSaleRequest
    {
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<SaleItemRequest> Items { get; set; }
        public bool IsCancelled { get; set; }
    }
}
