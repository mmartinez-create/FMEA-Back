using FmeaManager.Domain.CustomerProfiles;

namespace FmeaManager.Domain.Tests.CustomerProfiles;

public sealed class CustomerProfileTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActiveProfileAndNormalizeCode()
    {
        var profile = CustomerProfile.Create(
            "  ford  ",
            "Ford",
            "Ford customer profile");

        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Equal("FORD", profile.Code);
        Assert.Equal("Ford", profile.Name);
        Assert.Equal("Ford customer profile", profile.Description);
        Assert.True(profile.IsActive);
    }

    [Fact]
    public void Create_WithEmptyCode_ShouldThrowArgumentException()
    {
        var action = () => CustomerProfile.Create("", "Ford");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("code", exception.ParamName);
    }
}
