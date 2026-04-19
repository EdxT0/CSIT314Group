namespace CSIT_314_Group.Entity;

public class UserProfile
{
    public int Id { get; set; } // Link to user account
    public string ProfileName { get; set; } = "";
    public string Description { get; set; } = "";
    public String Status { get; set; } = "Active";
}