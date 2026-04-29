

namespace CSIT_314_Group.DTO.UserProfileDTO;

public class CreateUserProfileDTO
{
    public int Id { get; set; }
    public required String ProfileName { get; set; }
    public  required String Description { get; set; }
    
    
}