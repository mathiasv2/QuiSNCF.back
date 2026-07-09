namespace QuiSNCF.Exceptions;

public class AlreadyPlayedException(string playerName) : Exception($"{playerName} a déjà joué aujourd'hui");