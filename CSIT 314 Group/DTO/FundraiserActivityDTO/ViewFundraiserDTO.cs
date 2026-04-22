namespace CSIT_314_Group.DTO.FundraiserActivityDTO
{
    public class ViewFundraiserDTO
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Deadline { get; private set; }
        public bool Status { get; private set; }
        public double AmtRequested { get; private set; }
        public double AmtDonated { get; private set; }
        public int AmtOfViews { get; private set; }

        public string FraCategoryName { get; private set; }

        public ViewFundraiserDTO(int id, string name, string description, string deadline, double amtRequested, double amtDonated, int amtOfViews, bool status, string fraCategoryName)
        {
            Id = id;
            Name = name;
            Description = description;
            Deadline = deadline;
            AmtRequested = amtRequested;
            AmtDonated = amtDonated;
            AmtOfViews = amtOfViews;
            Status = status;
            FraCategoryName = fraCategoryName;
        }
    }
}
