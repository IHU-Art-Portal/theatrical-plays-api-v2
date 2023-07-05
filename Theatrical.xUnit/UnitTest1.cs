using Theatrical.Services.Curators;

namespace Theatrical.xUnit;

public class UnitTest1
{
    private readonly DataCurator curator;

    public UnitTest1()
    {
        curator = new DataCurator();
    }
    
    [Fact]
    public void TestFullNameValidation()
    {
        // Arrange
        string validFullName = "Κώστας Παπαδόπουλος";           //valid cases: Κώστας, Kostas, Kostas Papadopoulos, etc.
        string invalidFullName = "Α1Β2Γ3";                      //invalid cases: Κώστας παπαδόπουλος, κώστας Παπαδόπουλος, etc.
        
        // Act & Assert
        Assert.True(curator.ValidateFullName(validFullName));
        Assert.False(curator.ValidateFullName(invalidFullName));
    }
    
    [Fact]
    public void TestRoleValidation()
    {
        // Arrange
        string validRole = "Manager";
        string invalidRole = "123";
        
        // Act & Assert
        Assert.True(curator.ValidateRole(validRole));
        Assert.False(curator.ValidateRole(invalidRole));
    }
    
    [Fact]
    public void TestVenueTitleValidation()
    {
        // Arrange
        string validTitle = "Concert Hall";
        string invalidTitle = "123";
        
        // Act & Assert
        Assert.True(curator.ValidateVenueTitle(validTitle));
        Assert.False(curator.ValidateVenueTitle(invalidTitle));
    }
    
    [Fact]
    public void TestProductionTitleValidation()
    {
        // Arrange
        string validTitle = "Theatre Play";
        string invalidTitle = "123";
        
        // Act & Assert
        Assert.True(curator.ValidateProductionTitle(validTitle));
        Assert.False(curator.ValidateProductionTitle(invalidTitle));
    }
}