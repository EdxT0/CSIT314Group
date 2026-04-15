namespace CSIT_314_Group.Entity
{
    public class UserAccountEntity
    {
        public int id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string HashedPassword { get; set; } = "";
        public string Role { get; set; } = "User";
    }

}
