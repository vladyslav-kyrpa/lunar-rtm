namespace ServerApp.BusinessLogic.Common;

public static class ChatInputValidator
{
    public static bool IsValidTitle(string value) =>
        !string.IsNullOrEmpty(value) && value.Length > 0 && value.Length <= 100;

    public static bool IsValidDescription(string value) =>
        !string.IsNullOrEmpty(value) && value.Length > 0 && value.Length <= 500;
}