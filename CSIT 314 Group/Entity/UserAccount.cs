namespace CSIT_314_Group.Entity
{
    public class UserAccount
    {
        public int id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string HashedPassword { get; set; } = "";
        public string Profile { get; set; } = "User";
    }

}
