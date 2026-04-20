
namespace CSIT_314_Group.DTO.FundraiserActivityDTO
{
    public class UpdateFundraiserDTO
    {
        public int fraId { get; set; }
        public string fraName { get; set; } = "";
        public string description { get; set; } = "";
        public string deadline { get; set; } = "";
        public bool? status { get; set; } 
        public double? amtRequested { get; set; } 

    }
}
