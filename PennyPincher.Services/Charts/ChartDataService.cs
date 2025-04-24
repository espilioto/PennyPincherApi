using ErrorOr;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Charts;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Utils;
using System.Globalization;

namespace PennyPincher.Services.Charts;

public class ChartDataService : IChartDataService
{
    private readonly IStatementsService _statementsService;
    private readonly ILogger<StatementsService> _logger;
    private readonly IUtils _utils;

    public ChartDataService(IStatementsService statementsService, ILogger<StatementsService> logger, IUtils utils)
    {
        _statementsService = statementsService;
        _logger = logger;
        _utils = utils;
    }

    public async Task<ErrorOr<BreakdownDetailsForMonthResponse>> GetBreakdownDataForMonth(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        List<Error> errors = [];

        if (month < 1 || month > 12)
            errors.Add(Error.Validation(description: "wtf is that month bruh"));

        if (errors.Count > 0)
            return errors;

        try
        {
            var requestDate = new DateTime(year, month, 1);

            var excludedCategoryIds = new List<int>();
            if (ignoreInitsAndTransfers) //TODO oof
                excludedCategoryIds.Add(1);

            if (ignoreLoans) //TODO more oof
                excludedCategoryIds.Add(29);

            var statements = await _statementsService.GetAllAsync(
                new StatementFilterRequest(null, null, null, excludedCategoryIds, requestDate, requestDate.AddMonths(1).AddDays(-1), null, null, null),
                new StatementSortingRequest("date", "asc")
            );

            if (statements.IsError)
                return statements.Errors;

            var donutData = statements.Value
                .Where(x => x.Amount < 0)
                .GroupBy(x => x.Category.Name)
                .Select(x => new GenericKeyValueResponse(x.Key, x.Sum(d => d.Amount)))
                .ToList();

            var income = statements.Value.Where(x => x.Amount > 0).ToList();
            var totalIncome = income.Sum(x => x.Amount);
            var expenses = statements.Value.Where(x => x.Amount < 0).ToList();
            var totalExpenses = expenses.Sum(x => x.Amount);
            var balance = income.Sum(x => x.Amount) + expenses.Sum(x => x.Amount);

            return new BreakdownDetailsForMonthResponse(requestDate.ToString("MMMM yyyy"), donutData, income, expenses, totalIncome, totalExpenses, balance);

        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<List<MonthlyBreakdownResponse>>> GetMonthlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        try
        {
            var result = new List<MonthlyBreakdownResponse>();

            var excludedCategoryIds = new List<int>();
            if (ignoreInitsAndTransfers) //TODO oof
                excludedCategoryIds.Add(1);

            if (ignoreLoans) //TODO more oof
                excludedCategoryIds.Add(29);

            var statements = await _statementsService.GetAllAsync(
                new StatementFilterRequest(null, null, null, excludedCategoryIds, null, null, null, null, null),
                new StatementSortingRequest("date", "desc")
            );

            if (statements.IsError)
                return statements.Errors;

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

    public async Task<ErrorOr<List<GenericKeyValueResponse>>> GetOverviewBalanceChartData()
    {
        try
        {
            decimal balanceSum = 0;
            var result = new List<GenericKeyValueResponse>();

            var statements = await _statementsService.GetAllAsync(null, new StatementSortingRequest("date", "asc"));

            if (statements.IsError)
                return statements.Errors;

            var groupedStatements = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("MM/yy", CultureInfo.InvariantCulture)}" });

            foreach (var item in groupedStatements)
            {
                balanceSum += item.Sum(x => x.Amount);
                result.Add(new GenericKeyValueResponse(item.Key.date, balanceSum));
            }

            return result.Count > 0 ? result : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<CategoryAnalyticsResponse>> GetCategoryAnalyticsChartData(int categoryId)
    {
        try
        {
            var yearAverages = new List<GenericKeyValueResponse>();
            var chartData = new List<GenericKeyValueResponse>();

            var statements = await _statementsService.GetAllAsync(
                new StatementFilterRequest(null, [categoryId], null, null, null, null, null, null, null),
                new StatementSortingRequest("date", "asc")
            );

            if (statements.IsError)
                return statements.Errors;

            var minDate = statements.Value.Min(x => x.Date);
            var currentDate = DateTime.UtcNow;

            //yearly averages
            var statementsGroupedByYear = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("yyyy", CultureInfo.InvariantCulture)}" });

            foreach (var yearGroup in statementsGroupedByYear)
            {
                yearAverages.Add(new GenericKeyValueResponse(yearGroup.Key.date, yearGroup.Sum(x => Math.Abs(x.Amount) / 12)));
            }

            //chart data
            var statementsGroupedByMonth = statements.Value
                .GroupBy(x => new { date = $"{x.Date.ToString("MM/yy", CultureInfo.InvariantCulture)}" });

            foreach (var monthGroup in statementsGroupedByMonth)
            {
                chartData.Add(new GenericKeyValueResponse(monthGroup.Key.date, monthGroup.Sum(x => Math.Abs(x.Amount))));
            }

            return new CategoryAnalyticsResponse(yearAverages, chartData);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<SavingsChartResponse>> GetSavingsRateChartData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        try
        {
            var incomeResult = new List<GenericKeyValueResponse>();
            var expensesResult = new List<GenericKeyValueResponse>();
            var savingsResult = new List<GenericKeyValueResponse>();
            var yearlyAmounts = new List<SavingsChartYearlyAmountsResponse>();

            var excludedCategoryIds = new List<int>();
            if (ignoreInitsAndTransfers) //TODO oof
                excludedCategoryIds.Add(1);

            if (ignoreLoans) //TODO more oof
                excludedCategoryIds.Add(29);

            var statements = await _statementsService.GetAllAsync(
                new StatementFilterRequest(null, null, null, excludedCategoryIds, null, null, null, null, null),
                new StatementSortingRequest("date", "asc")
            );

            if (statements.IsError)
                return statements.Errors;

            //monthly calculations
            var statementsGroupedByMonth = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("MM/yy", CultureInfo.InvariantCulture)}" });

            var minDate = statements.Value.Min(x => x.Date);
            var currentDate = DateTime.UtcNow;

            var monthList = _utils.GetMonthList(minDate, currentDate);
            foreach (var month in monthList.Value)
            {
                var date = month.ToString("MM/yy", CultureInfo.InvariantCulture);

                var incomeAmount = statementsGroupedByMonth.First(x => x.Key.date == date)?.Where(x => x.Amount > 0).Sum(x => Math.Abs(x.Amount));
                var expensesAmount = statementsGroupedByMonth.First(x => x.Key.date == date)?.Where(x => x.Amount < 0).Sum(x => Math.Abs(x.Amount));
                var savingsAmount = incomeAmount < expensesAmount ? 0 : incomeAmount - expensesAmount;

                incomeResult.Add(new GenericKeyValueResponse(date, incomeAmount ?? 0));
                expensesResult.Add(new GenericKeyValueResponse(date, expensesAmount ?? 0));
                savingsResult.Add(new GenericKeyValueResponse(date, savingsAmount.HasValue ? savingsAmount.Value : 0));
            }

            //yearly calculations
            var statementsGroupedByYear = statements.Value.GroupBy(x => new { date = $"{x.Date.ToString("yyyy", CultureInfo.InvariantCulture)}" });
            var yearList = _utils.GetYearList(minDate, currentDate);

            foreach (var year in yearList.Value)
            {
                var date = year.Year.ToString();

                var incomeAmountForYear = statementsGroupedByYear.First(x => x.Key.date == date)?.Where(x => x.Amount > 0).Sum(x => Math.Abs(x.Amount));
                var expensesAmountForYear = statementsGroupedByYear.First(x => x.Key.date == date)?.Where(x => x.Amount < 0).Sum(x => Math.Abs(x.Amount));
                var savingsAmountForYear = incomeAmountForYear < expensesAmountForYear ? 0 : incomeAmountForYear - expensesAmountForYear;
                var savingsPercentForYear = Math.Round(savingsAmountForYear > 0 ? incomeAmountForYear.Value / Math.Abs(incomeAmountForYear.Value + expensesAmountForYear.Value) * 100 : 0, 0);

                yearlyAmounts.Add(new SavingsChartYearlyAmountsResponse(
                        date,
                        incomeAmountForYear.Value,
                        expensesAmountForYear.Value,
                        savingsAmountForYear.Value,
                        savingsPercentForYear
                    )
                );
            }

            var result = new SavingsChartResponse(yearlyAmounts, incomeResult, expensesResult, savingsResult);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
