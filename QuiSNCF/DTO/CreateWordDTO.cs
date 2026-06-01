namespace QuiSNCF.DTO;

public class CreateWordDTO
{
    public string WordName { get; set; }
    public string Definition { get; set; }
    public DateOnly LastTimePlayed { get; set; }
}