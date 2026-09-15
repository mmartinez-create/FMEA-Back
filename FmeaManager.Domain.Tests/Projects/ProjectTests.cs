using FmeaManager.Domain.Projects;

namespace FmeaManager.Domain.Tests.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateDraftProjectAndNormalizeCode()
    {
        var customerProfileId = Guid.NewGuid();

        var project = Project.Create(
            "  ford-batt-001  ",
            "12V Battery Launch",
            "Battery X",
            "Toluca",
            customerProfileId,
            "Quality Engineering",
            "cris");

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal("FORD-BATT-001", project.Code);
        Assert.Equal("12V Battery Launch", project.Name);
        Assert.Equal(customerProfileId, project.CustomerProfileId);
        Assert.Equal(ProjectStatus.Draft, project.Status);
        Assert.NotEqual(default, project.CreatedAt);
        Assert.Equal("cris", project.CreatedBy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrowArgumentException(string code)
    {
        var action = () => Project.Create(
            code,
            "12V Battery Launch",
            null,
            null,
            Guid.NewGuid(),
            "Quality Engineering",
            "cris");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCustomerProfile_ShouldThrowArgumentException()
    {
        var action = () => Project.Create(
            "FORD-BATT-001",
            "12V Battery Launch",
            null,
            null,
            Guid.Empty,
            "Quality Engineering",
            "cris");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("customerProfileId", exception.ParamName);
    }

    [Fact]
    public void Lifecycle_ShouldMoveFromDraftToArchivedInOrder()
    {
        var project = CreateProject();

        project.Activate();
        Assert.Equal(ProjectStatus.Active, project.Status);

        project.Complete();
        Assert.Equal(ProjectStatus.Completed, project.Status);

        project.Archive();
        Assert.Equal(ProjectStatus.Archived, project.Status);
    }

    [Fact]
    public void UpdateInformation_WhenProjectIsArchived_ShouldThrowInvalidOperationException()
    {
        var project = CreateProject();
        project.Activate();
        project.Complete();
        project.Archive();

        var action = () => project.UpdateInformation(
            "New Name",
            null,
            null,
            "New Owner");

        Assert.Throws<InvalidOperationException>(action);
    }

    private static Project CreateProject()
    {
        return Project.Create(
            "FORD-BATT-001",
            "12V Battery Launch",
            "Battery X",
            "Toluca",
            Guid.NewGuid(),
            "Quality Engineering",
            "cris");
    }
}
