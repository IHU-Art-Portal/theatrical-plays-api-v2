using Theatrical.Services.Curators;

namespace Theatrical.xUnit;

public class UnitTest1
{
    private readonly TestTestCurator _testTestCurator;

    public UnitTest1()
    {
        _testTestCurator = new TestTestCurator();
    }
    
    [Fact]
    public void TestFullNameValidation()
    {
        // Arrange
        string validFullName = "Κώστας Παπαδόπουλος";           //valid cases: Κώστας, Kostas, Kostas Papadopoulos, etc.
        string invalidFullName = "Α1Β2Γ3";                      //invalid cases: Κώστας παπαδόπουλος, κώστας Παπαδόπουλος, etc.
        
        // Act & Assert
        Assert.True(_testTestCurator.ValidateFullName(validFullName));
        Assert.False(_testTestCurator.ValidateFullName(invalidFullName));
    }
    
    [Fact]
    public void TestRoleValidation()
    {
        // Arrange
        string validRole = "Manager";
        string invalidRole = "123";
        
        // Act & Assert
        Assert.True(_testTestCurator.ValidateRole(validRole));
        Assert.False(_testTestCurator.ValidateRole(invalidRole));
    }
    
    [Fact]
    public void TestVenueTitleValidation()
    {
        // Arrange
        string validTitle = "Concert Hall";
        string invalidTitle = "123";
        
        // Act & Assert
        Assert.True(_testTestCurator.ValidateVenueTitle(validTitle));
        Assert.False(_testTestCurator.ValidateVenueTitle(invalidTitle));
    }
    
    [Fact]
    public void TestProductionTitleValidation()
    {
        // Arrange
        string validTitle = "Theatre Play";
        string invalidTitle = "123";
        
        // Act & Assert
        Assert.True(_testTestCurator.ValidateProductionTitle(validTitle));
        Assert.False(_testTestCurator.ValidateProductionTitle(invalidTitle));
    }
}