namespace SportsLeague.API.DTOs.Request;

public class MatchLineupRequestDTO
{
    public int PlayerId { get; set; }
    public string Position { get; set; } = string.Empty;
    public bool IsStarter { get; set; }
}
