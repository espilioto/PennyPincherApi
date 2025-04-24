namespace PennyPincher.Contracts.Charts;

public record CategoryAnalyticsResponse(
        List<GenericKeyValueResponse> YearAverages,
        List<GenericKeyValueResponse> ChartData
    );