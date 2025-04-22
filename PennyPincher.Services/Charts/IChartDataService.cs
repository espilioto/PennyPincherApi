using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<BreakdownDetailsForMonthResponse>> GetBreakdownDataForMonth(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<GenericChartResponse>>> GetOverviewBalanceChartData();
    public Task<ErrorOr<List<GenericChartResponse>>> GetCategoryAnalyticsChartData(int categoryId);
    public Task<ErrorOr<SavingsChartResponse>> GetSavingsRateChartData(bool ignoreInitsAndTransfers, bool ignoreLoans);
}
