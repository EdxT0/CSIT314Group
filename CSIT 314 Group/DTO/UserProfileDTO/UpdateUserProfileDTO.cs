namespace CSIT_314_Group.DTO.UserProfileDTO;

public class UpdateUserProfileDTO
{
    public int Id { get; set; }

    public required string ProfileName { get; set; }
    public required string Description { get; set; }
    
}