using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Charts;

namespace PennyPincher.Web.Controllers;

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
        var result = await _chartDataService.GetOverviewBalanceChartData();

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetMonthlyBreakdownData")]
    public async Task<IActionResult> GetMonthlyBreakdownData()
    {
        var result = await _chartDataService.GetMonthlyBreakdownData();

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }

    [HttpGet("GetBreakdownDataForMonth")]
    public async Task<IActionResult> GetBreakdownDataForMonth(int month, int year)
    {
        var result = await _chartDataService.GetBreakdownDataForMonth(month, year);

        return result.Match(
            chartData => Ok(chartData),
            errors => Problem(errors)
        );
    }
}
