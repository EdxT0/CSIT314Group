namespace CSIT_314_Group.DTO
{
    public class CreateUserDTO
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Password { get; set; } = "";
        public bool IsSuspended { get; set; } = false;
        public string Profile { get; set; } = "user";
    }
}
