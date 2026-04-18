namespace CSIT_314_Group.DTO.UserDTO
{
    public class LoginDTO
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool IsSuspended { get; set; }
    }
}
