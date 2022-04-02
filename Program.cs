using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
WebApplication app = builder.Build();
app.UseCors(builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins("http://localhost:3000"));
HttpClient client = new();


app.MapGet(
  "/intra-day-trade-history-summary",
  async (
    [FromQuery(Name = "startDate")] string startDate,
    [FromQuery(Name = "endDate")] string endDate
  ) =>
{
    HttpResponseMessage response = await client.GetAsync($"https://seffaflik.epias.com.tr/transparency/service/market/intra-day-trade-history?startDate={startDate}&endDate={endDate}");
    if (response.IsSuccessStatusCode)
    {
        string data = await response.Content.ReadAsStringAsync();
        Content? content = JsonSerializer.Deserialize<Content>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (content?.Body?.IntraDayTradeHistoryList.Count > 0)
        {
            
            List<TradeHistory> FilteredTradeHistoryList = content.Body.IntraDayTradeHistoryList.FindAll(tradeHistory => tradeHistory.Conract.StartsWith("PH"));
            List<TradeHistoryStatistics> tradeHistoryStatisticsList = new();

            foreach (var tradeHistory in FilteredTradeHistoryList)
            {
                if (!tradeHistoryStatisticsList.Any(stats => stats.Conract == tradeHistory.Conract))
                {
                    tradeHistoryStatisticsList.Add(new TradeHistoryStatistics(tradeHistory.Conract));
                } else
                {
                    tradeHistoryStatisticsList.Find(stats => stats.Conract == tradeHistory.Conract)!.AddTransaction(tradeHistory);
                }
            }

            return Results.Ok(tradeHistoryStatisticsList);
        }
    }
    return Results.NotFound();
});

app.Run();

public class Content
{
    public string? ResultCode { get; set; }
    public string? ResultDescription { get; set; }
    public Body? Body { get; set; }
    public object? Statistics { get; set; }
}

public class Body
{
    public List<TradeHistory> IntraDayTradeHistoryList { get; set; } = new List<TradeHistory>();
}

public class TradeHistory
{
    public int? Id { get; set; }
    public string? Date { get; set; }
    public string Conract { get; set; } = String.Empty;
    public double Price { get; set; }
    public int Quantity { get; set; }
}

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