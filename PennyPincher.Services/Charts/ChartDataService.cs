using ErrorOr;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Charts;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;
using System.Globalization;

namespace PennyPincher.Services.Charts;

public class ChartDataService : IChartDataService
{
    private readonly IStatementsService _statementsService;
    private readonly ILogger<StatementsService> _logger;

    public ChartDataService(IStatementsService statementsService, ILogger<StatementsService> logger)
    {
        _statementsService = statementsService;
        _logger = logger;
    }

    public Task<ErrorOr<BreakdownDetailsForMonth>> GetBreakdownDataForMonth(DateTime date)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData()
    {
        try
        {
            var result = new List<MonthlyBreakdownResponse>();

            var statements = await _statementsService.GetAllAsync(null, new StatementSortingRequest("date", "desc"));
            var groupedStatements = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture)}", month = x.Date.Month, year = x.Date.Year });

            foreach (var item in groupedStatements)
            {
                var income = item.Where(x => x.Amount > 0).Sum(x => x.Amount);
                var expenses = item.Where(x => x.Amount < 0).Sum(x => x.Amount);

                result.Add(new MonthlyBreakdownResponse(
                    item.Key.date,
                    item.Key.month,
                    item.Key.year,
                    income,
                    expenses,
                    income + expenses)
                );
            }

            return result.Count > 0 ? result : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<List<OverviewBalanceChartResponse>>> GetOverviewBalanceChartData()
    {
        try
        {
            decimal balanceSum = 0;
            var result = new List<OverviewBalanceChartResponse>();

            var statements = await _statementsService.GetAllAsync(null, new StatementSortingRequest("date", "asc"));
            var groupedStatements = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("MM/yy", CultureInfo.InvariantCulture)}" });

            foreach (var item in groupedStatements)
            {
                balanceSum += item.Sum(x => x.Amount);
                result.Add(new OverviewBalanceChartResponse(item.Key.date, balanceSum));
            }

            return result.Count > 0 ? result : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
