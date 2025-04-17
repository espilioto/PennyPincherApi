using ErrorOr;

namespace PennyPincher.Services.Utils;

public interface IUtils
{
    public ErrorOr<List<DateTime>> GetMonthList(DateTime start, DateTime end);
}
