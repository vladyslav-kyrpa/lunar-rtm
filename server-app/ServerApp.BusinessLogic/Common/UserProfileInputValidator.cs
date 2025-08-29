using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ServerApp.BusinessLogic.Common;

public static class UserProfileInputValidator
{
    /// <summary>
    /// Check username validity
    /// </summary>
    /// <param name="value">Username</param>
    /// <returns>True - if valid</returns>
    public static bool IsUsernameValid(string value) =>
        Regex.IsMatch(value, @"^[a-z0-9._-]{2,64}$")
        && !GetBlackList().Contains(value);

    private static HashSet<string> GetBlackList() => [
        "deleted-user"
    ];

    /// <summary>
    /// Check email validity (format)
    /// </summary>
    /// <param name="value">Email</param>
    /// <returns>True - if valid</returns>
    public static bool IsEmailValid(string value) =>
        !string.IsNullOrEmpty(value)
        && Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    /// <summary>
    /// Check user display name validity
    /// </summary>
    /// <param name="value">DisplayName</param>
    /// <returns>True - if valid</returns>
    public static bool IsDisplayNameValid(string value) =>
        !string.IsNullOrEmpty(value)
        && value.Length > 0 && value.Length <= 64;

    /// <summary>
    /// Check user bio validity 
    /// </summary>
    /// <param name="value">Bio</param>
    /// <returns>True - if valid</returns>
    public static bool IsBioValid(string value) =>
        !string.IsNullOrEmpty(value)
        && value.Length > 0 && value.Length <= 300;

}