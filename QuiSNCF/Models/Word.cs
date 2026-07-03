using QuiSNCF.Repository;

namespace QuiSNCF.Models;

public class Word : IPlayable
{
    public int WordId { get; set; }
    public string WordName { get; set; }
    public string Definition { get; set; }
    public DateOnly? LastTimePlayed { get; set; }
    public string DisplayName => WordName;
}