namespace Sales.Domain
{
    public class SaleItemRequest
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
