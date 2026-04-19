namespace CSIT_314_Group.DTO.UserAccountDTO
{
    public class UpdateUserDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Password { get; set; } = "";
        public string ProfileName { get; set; } = "";
    }
}
