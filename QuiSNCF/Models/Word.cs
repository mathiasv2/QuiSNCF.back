namespace QuiSNCF.Models;

public class Word
{
    public int WordId { get; set; }
    public string WordName { get; set; }
    public string Definition { get; set; }
    public DateOnly LastTimePlayed { get; set; }
    
}