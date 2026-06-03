namespace QuiSNCF.Models;

public class DailyPlay
{
    public int DailyPlayId { get; set; }

    public int PlayerId { get; set; }
    public GameType GameType { get; set; }
    public DateOnly PlayedDate { get; set; }
    public int Score { get; set; }
    public int Tries { get; set; }
    
    public Player Player { get; set; }

}