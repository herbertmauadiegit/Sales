namespace Sales.Domain
{
    public class Sale
    {
        public int Id { get; set; }
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public string Branch { get; set; }
        public List<SaleItem> Items { get; set; }
        public bool IsCancelled { get; set; }
    }
}
