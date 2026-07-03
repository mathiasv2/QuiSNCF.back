namespace QuiSNCF.Repository;

public interface IPlayable
{
    DateOnly? LastTimePlayed { get; set; }
    string DisplayName { get; } 
}