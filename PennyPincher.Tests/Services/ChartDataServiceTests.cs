using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Charts;
using PennyPincher.Services.Charts;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Utils;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class ChartDataServiceTests
{
    private readonly ILogger<StatementsService> _statementsLogger = LoggerFactory.Create(b => { }).CreateLogger<StatementsService>();

    private (ChartDataService chartService, StatementsService statementsService) CreateServices(string? dbName = null)
    {
        var context = TestDbContextFactory.Create(dbName);
        var mapper = TestDbContextFactory.CreateMapper();
        var statementsService = new StatementsService(context, mapper, _statementsLogger);
        var utils = new Utils(_statementsLogger);
        var chartService = new ChartDataService(statementsService, _statementsLogger, utils);
        return (chartService, statementsService);
    }

    private async Task<Data.PennyPincherApiDbContext> SeedStandardData(string dbName)
    {
        var context = TestDbContextFactory.Create(dbName);
        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "Checking", "#0000FF");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Transfers");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Food");
        await TestDbContextFactory.SeedCategoryAsync(context, 3, "user1", "Salary");
        await TestDbContextFactory.SeedCategoryAsync(context, 29, "user1", "Loans");
        return context;
    }

    // ========== GetOverviewBalanceChartDataAsync ==========

    [Fact]
    public async Task OverviewBalance_CumulativeBalanceIsCorrect()
    {
        var db = "overview_cumulative";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 1000m, new DateTime(2024, 1, 15));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -200m, new DateTime(2024, 2, 15));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 500m, new DateTime(2024, 3, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetOverviewBalanceChartDataAsync("user1");

        Assert.False(result.IsError);
        var data = result.Value;
        Assert.Equal(3, data.Count);
        Assert.Equal(1000m, data[0].Value);  // Jan: +1000
        Assert.Equal(800m, data[1].Value);   // Feb: 1000-200
        Assert.Equal(1300m, data[2].Value);  // Mar: 800+500
    }

    [Fact]
    public async Task OverviewBalance_KeysAreMonthYearFormat()
    {
        var db = "overview_keys";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, 100m, new DateTime(2024, 3, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetOverviewBalanceChartDataAsync("user1");

        Assert.False(result.IsError);
        // Flutter parses this as "MM/yy" in the line chart
        Assert.Equal("03/24", result.Value[0].Key);
    }

    [Fact]
    public async Task OverviewBalance_GroupsMultipleStatementsInSameMonth()
    {
        var db = "overview_grouping";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 500m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 300m, new DateTime(2024, 1, 20));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetOverviewBalanceChartDataAsync("user1");

        Assert.False(result.IsError);
        Assert.Single(result.Value);
        Assert.Equal(800m, result.Value[0].Value);
    }

    [Fact]
    public async Task OverviewBalance_ReturnsNotFound_WhenNoData()
    {
        var db = "overview_empty";
        await SeedStandardData(db);

        var (svc, _) = CreateServices(db);
        var result = await svc.GetOverviewBalanceChartDataAsync("user1");

        Assert.True(result.IsError);
    }

    // ========== GetMonthlyBreakdownDataAsync ==========

    [Fact]
    public async Task MonthlyBreakdown_SplitsIncomeAndExpenses()
    {
        var db = "monthly_split";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 2000m, new DateTime(2024, 3, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -500m, new DateTime(2024, 3, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetMonthlyBreakdownDataAsync("user1", false, false);

        Assert.False(result.IsError);
        var month = result.Value[0];
        Assert.Equal(2000m, month.Income);
        Assert.Equal(-500m, month.Expenses);
        Assert.Equal(1500m, month.Balance);
    }

    [Fact]
    public async Task MonthlyBreakdown_OutputHasFieldsFlutterExpects()
    {
        var db = "monthly_fields";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 100m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetMonthlyBreakdownDataAsync("user1", false, false);

        Assert.False(result.IsError);
        var item = result.Value[0];
        // Flutter reads: monthYear, month, year, income, expenses, balance
        Assert.Equal("June 2024", item.MonthYear);
        Assert.Equal(6, item.Month);
        Assert.Equal(2024, item.Year);
    }

    [Fact]
    public async Task MonthlyBreakdown_ExcludesCategoryId1_WhenIgnoreInitsAndTransfers()
    {
        var db = "monthly_exclude_transfers";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 5000m, new DateTime(2024, 1, 1), "Transfer");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 2000m, new DateTime(2024, 1, 15), "Salary");

        var (svc, _) = CreateServices(db);
        var result = await svc.GetMonthlyBreakdownDataAsync("user1", true, false);

        Assert.False(result.IsError);
        Assert.Equal(2000m, result.Value[0].Income); // Transfer excluded
    }

    // ========== GetYearlyBreakdownDataAsync ==========

    [Fact]
    public async Task YearlyBreakdown_GroupsByYear()
    {
        var db = "yearly_group";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 1000m, new DateTime(2023, 6, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 2000m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetYearlyBreakdownDataAsync("user1", false, false);

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task YearlyBreakdown_OutputHasFieldsFlutterExpects()
    {
        var db = "yearly_fields";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 3000m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -800m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetYearlyBreakdownDataAsync("user1", false, false);

        Assert.False(result.IsError);
        var item = result.Value[0];
        // Flutter reads: year, income, expenses, balance
        Assert.Equal(2024, item.Year);
        Assert.Equal(3000m, item.Income);
        Assert.Equal(-800m, item.Expenses);
        Assert.Equal(2200m, item.Balance);
    }

    // ========== GetBreakdownDataForMonthAsync ==========

    [Fact]
    public async Task BreakdownForMonth_RejectsInvalidMonth()
    {
        var db = "month_invalid";
        await SeedStandardData(db);
        var (svc, _) = CreateServices(db);

        var result0 = await svc.GetBreakdownDataForMonthAsync("user1", 0, 2024, false, false);
        var result13 = await svc.GetBreakdownDataForMonthAsync("user1", 13, 2024, false, false);

        Assert.True(result0.IsError);
        Assert.True(result13.IsError);
    }

    [Fact]
    public async Task BreakdownForMonth_DonutDataOnlyContainsExpenses()
    {
        var db = "month_donut";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 2000m, new DateTime(2024, 3, 1), "Salary");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -150m, new DateTime(2024, 3, 10), "Lunch");

        var (svc, _) = CreateServices(db);
        var result = await svc.GetBreakdownDataForMonthAsync("user1", 3, 2024, false, false);

        Assert.False(result.IsError);
        // Donut should only have expenses
        Assert.Single(result.Value.DonutData);
        Assert.True(result.Value.DonutData[0].Value < 0);
    }

    [Fact]
    public async Task BreakdownForMonth_OutputHasAllFieldsFlutterExpects()
    {
        var db = "month_fields";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 3000m, new DateTime(2024, 5, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -400m, new DateTime(2024, 5, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetBreakdownDataForMonthAsync("user1", 5, 2024, false, false);

        Assert.False(result.IsError);
        var data = result.Value;
        // Flutter reads: title, donutData, incomeStatements, expenseStatements, totalIncome, totalExpenses, balance
        Assert.Equal("May 2024", data.Title);
        Assert.NotNull(data.DonutData);
        Assert.Single(data.IncomeStatements);
        Assert.Single(data.ExpenseStatements);
        Assert.Equal(3000m, data.TotalIncome);
        Assert.Equal(-400m, data.TotalExpenses);
        Assert.Equal(2600m, data.Balance);
    }

    [Fact]
    public async Task BreakdownForMonth_OnlyIncludesStatementsInRequestedMonth()
    {
        var db = "month_range";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 1000m, new DateTime(2024, 1, 15));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 2000m, new DateTime(2024, 2, 15));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 3000m, new DateTime(2024, 3, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetBreakdownDataForMonthAsync("user1", 2, 2024, false, false);

        Assert.False(result.IsError);
        Assert.Equal(2000m, result.Value.TotalIncome);
    }

    // ========== GetBreakdownDataForYearAsync ==========

    [Fact]
    public async Task BreakdownForYear_RejectsInvalidYear()
    {
        var db = "year_invalid";
        await SeedStandardData(db);
        var (svc, _) = CreateServices(db);

        var result = await svc.GetBreakdownDataForYearAsync("user1", 1999, false, false);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task BreakdownForYear_DonutDataCappedAt15Categories()
    {
        var db = "year_donut_cap";
        var context = await SeedStandardData(db);

        // Seed 20 categories with expenses (IDs 100+ to avoid conflicts with standard seed)
        for (int i = 0; i < 20; i++)
        {
            var catId = 100 + i;
            await TestDbContextFactory.SeedCategoryAsync(context, catId, "user1", $"Cat{catId}");
            await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, catId, -100m * (i + 1), new DateTime(2024, 1, 1));
        }

        var (svc, _) = CreateServices(db);
        var result = await svc.GetBreakdownDataForYearAsync("user1", 2024, false, false);

        Assert.False(result.IsError);
        Assert.Equal(15, result.Value.DonutData.Count);
    }

    [Fact]
    public async Task BreakdownForYear_TitleIsYearString()
    {
        var db = "year_title";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 1000m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetBreakdownDataForYearAsync("user1", 2024, false, false);

        Assert.False(result.IsError);
        // Flutter uses title directly as display text
        Assert.Equal("2024", result.Value.Title);
    }

    // ========== GetSavingsRateChartDataAsync ==========

    [Fact]
    public async Task SavingsRate_CalculatesSavingsCorrectly()
    {
        var db = "savings_calc";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 3000m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -1000m, new DateTime(2024, 1, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetSavingsRateChartDataAsync("user1", false, false);

        Assert.False(result.IsError);
        // Month: income=3000, expenses=1000, savings=2000
        var janIncome = result.Value.IncomeChart.First(x => x.Key == "01/24");
        var janExpenses = result.Value.ExpensesChart.First(x => x.Key == "01/24");
        var janSavings = result.Value.SavingsChart.First(x => x.Key == "01/24");

        Assert.Equal(3000m, janIncome.Value);
        Assert.Equal(1000m, janExpenses.Value); // abs value
        Assert.Equal(2000m, janSavings.Value);
    }

    [Fact]
    public async Task SavingsRate_SavingsFlooredAtZero_WhenExpensesExceedIncome()
    {
        var db = "savings_floor";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 500m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -2000m, new DateTime(2024, 1, 15));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetSavingsRateChartDataAsync("user1", false, false);

        Assert.False(result.IsError);
        var janSavings = result.Value.SavingsChart.First(x => x.Key == "01/24");
        Assert.Equal(0m, janSavings.Value);
    }

    [Fact]
    public async Task SavingsRate_ThreeChartsHaveSameLength()
    {
        var db = "savings_length";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 1000m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -200m, new DateTime(2024, 3, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetSavingsRateChartDataAsync("user1", false, false);

        Assert.False(result.IsError);
        // Flutter asserts all three lists have same length before drawing
        Assert.Equal(result.Value.IncomeChart.Count, result.Value.ExpensesChart.Count);
        Assert.Equal(result.Value.IncomeChart.Count, result.Value.SavingsChart.Count);
    }

    [Fact]
    public async Task SavingsRate_YearlyAveragesHaveAllFields()
    {
        var db = "savings_yearly_fields";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 5000m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -2000m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetSavingsRateChartDataAsync("user1", false, false);

        Assert.False(result.IsError);
        Assert.NotEmpty(result.Value.AveragesPerYear);
        var yearData = result.Value.AveragesPerYear.First(x => x.Year == "2024");
        // Flutter reads: year, incomeAmount, expensesAmount, savingsAmount, savingsPercent
        Assert.Equal(5000m, yearData.IncomeAmount);
        Assert.Equal(2000m, yearData.ExpensesAmount);
        Assert.Equal(3000m, yearData.SavingsAmount);
        Assert.Equal(60m, yearData.SavingsPercent); // 3000/5000 * 100 = 60
    }

    [Fact]
    public async Task SavingsRate_ChartKeysAreMMSlashYYFormat()
    {
        var db = "savings_keys";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 100m, new DateTime(2024, 11, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetSavingsRateChartDataAsync("user1", false, false);

        Assert.False(result.IsError);
        // Flutter savings chart parses keys as "MM/yy"
        Assert.Contains(result.Value.IncomeChart, x => x.Key == "11/24");
    }

    // ========== GetCategoryAnalyticsChartDataAsync ==========

    [Fact]
    public async Task CategoryAnalytics_OnlyIncludesRequestedCategory()
    {
        var db = "cat_filter";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -100m, new DateTime(2024, 1, 1), "Food expense");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 3, 3000m, new DateTime(2024, 1, 1), "Salary");

        var (svc, _) = CreateServices(db);
        var result = await svc.GetCategoryAnalyticsChartDataAsync("user1", 2);

        Assert.False(result.IsError);
        // Year sums should only reflect category 2, using abs values
        var yearEntry = result.Value.YearSums.First(x => x.Key == "2024");
        Assert.Equal(100m, yearEntry.Value); // abs(-100)
    }

    [Fact]
    public async Task CategoryAnalytics_YearSumsIncludesSumTotal()
    {
        var db = "cat_sum";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -100m, new DateTime(2023, 6, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -200m, new DateTime(2024, 6, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetCategoryAnalyticsChartDataAsync("user1", 2);

        Assert.False(result.IsError);
        // Flutter expects a "Sum" entry at the end of yearSums
        var sumEntry = result.Value.YearSums.Last();
        Assert.Equal("Sum", sumEntry.Key);
        Assert.Equal(300m, sumEntry.Value);
    }

    [Fact]
    public async Task CategoryAnalytics_ChartDataKeysAreMMSlashYYFormat()
    {
        var db = "cat_keys";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -50m, new DateTime(2024, 7, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetCategoryAnalyticsChartDataAsync("user1", 2);

        Assert.False(result.IsError);
        // Flutter category chart parses keys as "MM/yy"
        Assert.Contains(result.Value.ChartData, x => x.Key == "07/24");
    }

    [Fact]
    public async Task CategoryAnalytics_UsesAbsoluteValues()
    {
        var db = "cat_abs";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -350m, new DateTime(2024, 1, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetCategoryAnalyticsChartDataAsync("user1", 2);

        Assert.False(result.IsError);
        var janData = result.Value.ChartData.First(x => x.Key == "01/24");
        Assert.Equal(350m, janData.Value); // abs(-350)
    }

    [Fact]
    public async Task CategoryAnalytics_MonthsWithNoDataGetZero()
    {
        var db = "cat_gaps";
        var context = await SeedStandardData(db);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -100m, new DateTime(2024, 1, 1));
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, -200m, new DateTime(2024, 3, 1));

        var (svc, _) = CreateServices(db);
        var result = await svc.GetCategoryAnalyticsChartDataAsync("user1", 2);

        Assert.False(result.IsError);
        // Feb should exist with value 0 — Flutter draws all months on the x-axis
        var febData = result.Value.ChartData.First(x => x.Key == "02/24");
        Assert.Equal(0m, febData.Value);
    }
}
