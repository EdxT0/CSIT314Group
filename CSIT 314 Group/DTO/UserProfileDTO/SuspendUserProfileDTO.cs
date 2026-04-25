namespace CSIT_314_Group.DTO.UserProfileDTO;

public class SuspendUserProfileDTO
{
    public int Id { get; set; }
    public required string ProfileName { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }

}