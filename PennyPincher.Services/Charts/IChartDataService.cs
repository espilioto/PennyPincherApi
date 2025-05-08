using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownDataAsync(bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<BreakdownDetailsResponse>> GetBreakdownDataForMonthAsync(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<GenericKeyValueResponse>>> GetOverviewBalanceChartDataAsync();
    public Task<ErrorOr<CategoryAnalyticsResponse>> GetCategoryAnalyticsChartDataAsync(int categoryId);
    public Task<ErrorOr<SavingsChartResponse>> GetSavingsRateChartDataAsync(bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<YearlyBreakdownResponse>>> GetYearlyBreakdownDataAsync(bool ignoreInitsAndTransfers, bool ignoreLoans);


}
