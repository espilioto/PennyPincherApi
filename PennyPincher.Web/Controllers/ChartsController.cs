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
}
