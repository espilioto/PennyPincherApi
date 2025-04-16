using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<BreakdownDetailsForMonthResponse>> GetBreakdownDataForMonth(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<OverviewBalanceChartResponse>>> GetOverviewBalanceChartData();
    public Task<ErrorOr<List<CategoryAnalyticsChartResponse>>> GetCategoryAnalyticsChartData(int categoryId);
}
