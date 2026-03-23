namespace QuiSNCF.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Tries { get; set; }
    public DateOnly ScoreDate { get; set; }
}