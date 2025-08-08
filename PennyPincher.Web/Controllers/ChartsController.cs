using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Charts;

namespace PennyPincher.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class ChartsController : ErrorOrApiController
{
    public readonly IChartDataService _chartDataService;

    public ChartsController(IChartDataService chartDataService)
    {
        _chartDataService = chartDataService;
    }

    [HttpGet("GetOverviewBalanceChartData")]
    public async Task<IActionResult> GetOverviewBalanceChartData()
    {
        var result = await _chartDataService.GetOverviewBalanceChartDataAsync();

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetMonthlyBreakdownData")]
    public async Task<IActionResult> GetMonthlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var result = await _chartDataService.GetMonthlyBreakdownDataAsync(ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetBreakdownDataForMonth")]
    public async Task<IActionResult> GetBreakdownDataForMonth(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var result = await _chartDataService.GetBreakdownDataForMonthAsync(month, year, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetCategoryAnalyticsChartData")]
    public async Task<IActionResult> GetCategoryAnalyticsChartData(int categoryId)
    {
        var result = await _chartDataService.GetCategoryAnalyticsChartDataAsync(categoryId);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetSavingsRateChartData")]
    public async Task<IActionResult> GetSavingsRateChartData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var result = await _chartDataService.GetSavingsRateChartDataAsync(ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetYearlyBreakdownData")]
    public async Task<IActionResult> GetYearlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var result = await _chartDataService.GetYearlyBreakdownDataAsync(ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetBreakdownDataForYear")]
    public async Task<IActionResult> GetBreakdownDataForYear(int year, bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var result = await _chartDataService.GetBreakdownDataForYearAsync(year, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }
}
