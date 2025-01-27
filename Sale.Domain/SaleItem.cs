using System.Text.Json.Serialization;

namespace Sales.Domain
{
    public class SaleItem
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount => Quantity * UnitPrice * (1 - Discount);
        [JsonIgnore]
        public int? SaleId { get; set; }
        [JsonIgnore]
        public Sale? Sale { get; set; }
    }
}
