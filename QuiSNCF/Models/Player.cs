namespace QuiSNCF.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Tries { get; set; }
    public int Season { get; set; }
    public ICollection<DailyPlay> DailyPlays { get; set; } 

}