using ByBitApi.Models;

namespace ByBitApi.Interface;

public interface IByBitService
{
    delegate void CostHandler(ByBitResponse cost);

    event CostHandler? ChangeProgress;

    Task CheckIsStarted();

    Task StartAsync(ByBitQuery query);

    Task ReStartAsync(ByBitQuery query);

    Task<string?> GetBalance();
}