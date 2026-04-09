using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownDataAsync(string userId, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<BreakdownDetailsResponse>> GetBreakdownDataForMonthAsync(string userId, int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<GenericKeyValueResponse>>> GetOverviewBalanceChartDataAsync(string userId);
    public Task<ErrorOr<CategoryAnalyticsResponse>> GetCategoryAnalyticsChartDataAsync(string userId, int categoryId);
    public Task<ErrorOr<SavingsChartResponse>> GetSavingsRateChartDataAsync(string userId, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<List<YearlyBreakdownResponse>>> GetYearlyBreakdownDataAsync(string userId, bool ignoreInitsAndTransfers, bool ignoreLoans);
    public Task<ErrorOr<BreakdownDetailsResponse>> GetBreakdownDataForYearAsync(string userId, int year, bool ignoreInitsAndTransfers, bool ignoreLoans);


}
