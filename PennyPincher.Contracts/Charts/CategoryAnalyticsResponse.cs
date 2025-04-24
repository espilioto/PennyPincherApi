namespace PennyPincher.Contracts.Charts;

public record CategoryAnalyticsResponse(
        List<GenericKeyValueResponse> YearSums,
        List<GenericKeyValueResponse> ChartData
    );