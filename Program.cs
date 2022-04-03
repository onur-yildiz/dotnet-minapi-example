using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
WebApplication app = builder.Build();
app.UseCors(builder =>
    builder.SetIsOriginAllowed(
        origin =>
        {
            var host = new Uri(origin).Host;
            return host == "localhost" || host == "spcase-app.vercel.app";
        }
    )
);

app.MapGet(
  "/intra-day-trade-history-summary",
  async (
    [FromQuery(Name = "startDate")] string startDate,
    [FromQuery(Name = "endDate")] string endDate
  ) =>
{
    var client = new HttpClient();
    var response = await client.GetAsync($"https://seffaflik.epias.com.tr/transparency/service/market/intra-day-trade-history?startDate={startDate}&endDate={endDate}");

    if (!response.IsSuccessStatusCode) return Results.StatusCode((int)response.StatusCode);

    var data = await response.Content.ReadAsStringAsync();
    var content = JsonSerializer.Deserialize<Content>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (content?.Body?.IntraDayTradeHistoryList.Count == 0) return Results.NotFound();

    var filteredTradeHistoryList = content!.Body!.IntraDayTradeHistoryList.Where(tradeHistory => tradeHistory.Conract.StartsWith("PH"));
    var tradeHistoryStatisticsList = new List<TradeHistoryStatistics>();

    foreach (var tradeHistory in filteredTradeHistoryList)
    {
        if (!tradeHistoryStatisticsList.Any(stats => stats.Conract == tradeHistory.Conract))
        {
            tradeHistoryStatisticsList.Add(new TradeHistoryStatistics(tradeHistory.Conract));
        }
        else
        {
            tradeHistoryStatisticsList.Find(stats => stats.Conract == tradeHistory.Conract)!.AddTransaction(tradeHistory);
        }
    }

    return Results.Ok(tradeHistoryStatisticsList);
});

app.Run();





