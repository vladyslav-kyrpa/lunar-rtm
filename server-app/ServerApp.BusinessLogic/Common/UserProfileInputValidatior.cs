using System.Text.RegularExpressions;

namespace ServerApp.BusinessLogic.Common;

public class UserProfileInputValidator
{
    public static bool IsUsernameValid(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value), "Username is null or empty");

        if (value.Length < 2 || value.Length > 64)
            return false;

        var allowedChars = "abcdefghijklmnopqrstuvwxyz1234567890.-_";
        foreach (var c in value)
            if (!allowedChars.Contains(c)) return false;

        if (GetBlackList().Contains(value))
            return false;

        return true;
    }

    public static bool IsEmailValid(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value), "Email is null or empty");
        return Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private static HashSet<string> GetBlackList() => [
        "deleted-user"
    ];
}