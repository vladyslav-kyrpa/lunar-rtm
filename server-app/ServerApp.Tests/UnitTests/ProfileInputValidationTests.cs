using ServerApp.BusinessLogic.Common;

namespace ServerApp.Tests.UnitTests;

public class ProfileInputValidationTests
{
    [Theory]
    [InlineData("user1-.", true)]
    [InlineData("a", false)]
    [InlineData("", false)]
    [InlineData("some user", false)]
    public void UsernameValidation(string username, bool isValid)
    {
        // Act
        var result = UserProfileInputValidator.IsUsernameValid(username); 

        // Assert
        if (isValid) Assert.True(result);
        else Assert.False(result);
    } 
}