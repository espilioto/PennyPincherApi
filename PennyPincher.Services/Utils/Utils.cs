using ErrorOr;
using Microsoft.Extensions.Logging;
using PennyPincher.Services.Statements;

namespace PennyPincher.Services.Utils;

public class Utils : IUtils
{
    private readonly ILogger<StatementsService> _logger;

    public Utils(ILogger<StatementsService> logger)
    {
        _logger = logger;
    }

    public ErrorOr<List<DateTime>> GetMonthList(DateTime start, DateTime end)
    {
        try
        {
            var result = new List<DateTime>();

            for (var d = start; d <= end; d = d.AddMonths(1))
            {
                result.Add(d);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public ErrorOr<List<DateTime>> GetYearList(DateTime start, DateTime end)
    {
        try
        {
            var result = new List<DateTime>();

            for (int d = start.Year; d <= end.Year; d++)
            {
                result.Add(new DateTime(d, 1, 1));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
