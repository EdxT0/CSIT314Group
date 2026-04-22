using System.ComponentModel.DataAnnotations;

namespace CSIT_314_Group.DTO.FundraiserActivityDTO
{
    public class CreateFundraiserDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string deadline { get; set; }

        [Required]
        public int? fraCategoryId { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "Amount requested must be more than 0.")]
        public double amtRequested { get; set; }

    }
}
