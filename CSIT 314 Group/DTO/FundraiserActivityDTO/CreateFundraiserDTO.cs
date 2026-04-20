using System.ComponentModel.DataAnnotations;

namespace CSIT_314_Group.DTO.FundraiserActivityDTO
{
    public class CreateFundraiserDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]

        public string Deadline { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount requested must be more than 0.")]

        public double amtRequested { get; set; }

    }
}
