using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<BreakdownDetailsForMonthResponse>> GetBreakdownDataForMonth(int month, int year);
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData();
    public Task<ErrorOr<List<OverviewBalanceChartResponse>>> GetOverviewBalanceChartData();
}
