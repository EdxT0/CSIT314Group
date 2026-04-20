namespace CSIT_314_Group.DTO.FundraiserActivityDTO;

public class UpdateFundraiserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Deadline { get; set; } = string.Empty;   // dd-MM-yyyy
    public double AmtRequested { get; set; }
    public bool Status { get; set; }
}