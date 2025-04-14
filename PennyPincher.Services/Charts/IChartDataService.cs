using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<BreakdownDetailsForMonth>> GetBreakdownDataForMonth(DateTime date);
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData();
    public Task<ErrorOr<List<OverviewBalanceChartResponse>>> GetOverviewBalanceChartData();
}
