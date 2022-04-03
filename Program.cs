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
    try
    {
        var response = await FetchFromEpiasAsync(startDate, endDate);
        if (!response.IsSuccessStatusCode) return Results.StatusCode((int)response.StatusCode);

        var tradeHistoryList = await ExtractTradeHistoryListAsync(response);
        if (tradeHistoryList.Count == 0) return Results.NotFound();

        var statisticsSummary = GetStatisticsSummary(tradeHistoryList);
        return Results.Ok(statisticsSummary);
    } catch (Exception ex)
    {
        app.Logger.LogError(ex.ToString());
        return Results.Problem(statusCode: 500);
    }
});

app.Run();

static async Task<HttpResponseMessage> FetchFromEpiasAsync (string startDate, string endDate)
{
    var client = new HttpClient();
    return await client.GetAsync($"https://seffaflik.epias.com.tr/transparency/service/market/intra-day-trade-history?startDate={startDate}&endDate={endDate}");
}

static async Task<List<TradeHistory>> ExtractTradeHistoryListAsync(HttpResponseMessage response)
{
    var data = await response.Content.ReadAsStringAsync();
    var content = JsonSerializer.Deserialize<ResponseContent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return content?.Body?.IntraDayTradeHistoryList ?? new List<TradeHistory>();
}

static HashSet<TradeHistoryStatistics> GetStatisticsSummary(List<TradeHistory> tradeHistoryList)
{
    var filteredTradeHistoryList = tradeHistoryList.Where(tradeHistory => tradeHistory.Conract.StartsWith("PH"));
    var tradeHistoryStatistics = new HashSet<TradeHistoryStatistics>();
    
    foreach (var tradeHistory in filteredTradeHistoryList)
    {
        var statistics = new TradeHistoryStatistics(tradeHistory.Conract);
        if (tradeHistoryStatistics.TryGetValue(statistics, out var value))
        {
            value.AddTransaction(tradeHistory);
            continue;
        }
        tradeHistoryStatistics.Add(statistics);
    }

    return tradeHistoryStatistics;
}
