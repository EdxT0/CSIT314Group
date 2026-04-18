namespace CSIT_314_Group.DTO.UserDTO
{
    public class UserAccountDTO
    {
        public int id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ProfileName { get; set; } 
        public bool IsSuspended { get; set; } = false;


    }
}
