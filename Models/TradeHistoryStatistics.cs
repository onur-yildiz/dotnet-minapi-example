namespace Models
{
    public class TradeHistoryStatistics
    {
        public string Conract { get; set; } = string.Empty;
        public double TotalTransactionFee { get; set; }
        public double TotalTransactionAmount { get; set; }
        public double WeightedAveragePrice { get; set; }

        public TradeHistoryStatistics(string conract, double totalTransactionFee = 0, double totalTransactionAmount = 0, double weightedAveragePrice = 0)
        {
            this.Conract = conract;
            this.TotalTransactionFee = totalTransactionFee;
            this.TotalTransactionAmount = totalTransactionAmount;
            this.WeightedAveragePrice = weightedAveragePrice;
        }

        public void AddTransaction(TradeHistory tradeHistory)
        {
            TotalTransactionFee += tradeHistory.Price * tradeHistory.Quantity / 10;
            TotalTransactionAmount += tradeHistory.Quantity / 10;
            WeightedAveragePrice = TotalTransactionFee / TotalTransactionAmount;
        }
    }

}
