using System.ComponentModel.DataAnnotations;

namespace CSIT_314_Group.Entity
{
    public class Fundraiser
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime Deadline { get; private set; }
        public bool Status { get; private set; }
        public double AmtRequested { get; private set; }
        public double AmtDonated { get; private set; }
        public int AmtOfViews { get; private set; }
        public Fundraiser(string name, string description, DateTime deadline, double amtRequested)
        {
            Name = name;
            Description = description;
            Deadline = deadline;
            AmtRequested = amtRequested;
            AmtDonated = 0;
            AmtOfViews = 0;
            Status = false;
        }
        public Fundraiser(int id ,string name, string description, DateTime deadline, double amtRequested, double amtDonated, int amtOfViews, bool status)
        {
            Id = id;
            Name = name;
            Description = description;
            Deadline = deadline;
            AmtRequested = amtRequested;
            AmtDonated = amtDonated;
            AmtOfViews = amtOfViews;
            Status = status;
        }
        public void AddDonation(double amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Donation must be more than 0.");

            AmtDonated += amount;
        }

        public void IncrementViews()
        {
            AmtOfViews++;
        }

        public void CloseFundraiser()
        {
            Status = false;
        }
    }
}
