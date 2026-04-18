namespace CSIT_314_Group.DTO.UserDTO
{
    public class LoginDTO
    {
        public string email { get; set; } = "";
        public string password { get; set; } = "";
        public bool IsSuspended { get; set; }
    }
}
