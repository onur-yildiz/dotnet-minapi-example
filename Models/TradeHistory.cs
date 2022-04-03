namespace Models
{
    public class TradeHistory
    {
        public int? Id { get; set; }
        public string? Date { get; set; }
        public string Conract { get; set; } = String.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}