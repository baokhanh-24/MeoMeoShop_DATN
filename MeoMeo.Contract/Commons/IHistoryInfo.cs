using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.Commons;

public interface IHasHistoryInfo
{
    DateTime CreationTime { get; }
    string Content { get; }
    string Actor { get; }
    EHistoryType Type { get; }
}
