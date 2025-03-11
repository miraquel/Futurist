using System.Text.RegularExpressions;

namespace Futurist.Common.Helpers;

public static partial class RegexHelper
{
    [GeneratedRegex(@"^(>=|<=|>|<|<>)?\s*(-?\d+(?:\.\d+)?)$")]
    public static partial Regex LogicalOperatorRegex();
}