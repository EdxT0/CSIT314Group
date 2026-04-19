public class UserAccount
{
    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public string Email { get; private set; } = "";
    public string PhoneNumber { get; private set; } = "";
    public string HashedPassword { get; private set; } = "";
    public int? ProfileId { get; private set; }
    public bool IsSuspended { get; private set; } = false;

    public UserAccount(
        string name,
        string email,
        string phoneNumber,
        int? profileId,
        bool isSuspended
        )
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        ProfileId = profileId;
        IsSuspended = isSuspended;
    }
    public UserAccount(
       int id,
       string name,
       string email,
       string phoneNumber,
       string hashedPassword,
       int? profileId,
       bool isSuspended)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        HashedPassword = hashedPassword;
        ProfileId = profileId;
        IsSuspended = isSuspended;
    }



    public void UpdateContactDetails(string name, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void setPassword(string newHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new ArgumentException("Password hash cannot be empty.");

        HashedPassword = newHashedPassword;
    }

}