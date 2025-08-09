
using System.Text.RegularExpressions;

public static class GameUtils
{
    public static string ActionNameHint(string action, string name, string hint)
    {
        return hint
            .Replace("{ACTION}", action)
            .Replace("{NAME}",
                Regex.Replace(name,
                    "(\\B[A-Z])",
                    " $1")
            );
    }

    public static string SplitPascalCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "(\\B[A-Z])", " $1");
    }
}
