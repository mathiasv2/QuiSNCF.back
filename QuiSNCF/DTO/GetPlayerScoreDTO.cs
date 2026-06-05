namespace QuiSNCF.DTO;

public class GetPlayerScoreDTO
{
    public string Name { get; set; }
    public DateOnly PlayedDate { get; set; }
    public int Score { get; set; }
    public int Tries { get; set; }
}