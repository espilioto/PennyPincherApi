using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Charts;
using PennyPincher.Api.Extensions;

namespace PennyPincher.Api.Controllers;

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
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetOverviewBalanceChartDataAsync(userId);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetMonthlyBreakdownData")]
    public async Task<IActionResult> GetMonthlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetMonthlyBreakdownDataAsync(userId, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetBreakdownDataForMonth")]
    public async Task<IActionResult> GetBreakdownDataForMonth(int month, int year, bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetBreakdownDataForMonthAsync(userId, month, year, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetCategoryAnalyticsChartData")]
    public async Task<IActionResult> GetCategoryAnalyticsChartData(int categoryId)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetCategoryAnalyticsChartDataAsync(userId, categoryId);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetSavingsRateChartData")]
    public async Task<IActionResult> GetSavingsRateChartData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetSavingsRateChartDataAsync(userId, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetYearlyBreakdownData")]
    public async Task<IActionResult> GetYearlyBreakdownData(bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetYearlyBreakdownDataAsync(userId, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetBreakdownDataForYear")]
    public async Task<IActionResult> GetBreakdownDataForYear(int year, bool ignoreInitsAndTransfers, bool ignoreLoans)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _chartDataService.GetBreakdownDataForYearAsync(userId, year, ignoreInitsAndTransfers, ignoreLoans);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }
}
