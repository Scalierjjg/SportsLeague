namespace SportsLeague.Domain.Entities;

public class MatchLineup : AuditBase
{
    public int MatchId { get; set; }

    public int PlayerId { get; set; }

    public bool IsStarter { get; set; }

    public string Position { get; set; } = string.Empty;
    // GK, CB, CMD, ST

    // Navigation Properties
    public Match? Match { get; set; }

    public Player? Player { get; set; }


}
