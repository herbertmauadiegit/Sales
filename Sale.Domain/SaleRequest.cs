namespace Sales.Domain
{
    public class SaleRequest
    {
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<SaleItemRequest> Items { get; set; }
    }
}
