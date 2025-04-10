﻿using ErrorOr;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.Services.Charts;

public interface IChartDataService
{
    public Task<ErrorOr<List<OverviewBalanceChartResponse>>> GetOverviewBalanceChartData();
}
