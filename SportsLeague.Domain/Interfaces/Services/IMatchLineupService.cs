using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;


public interface IMatchLineupService
{

    //Registrar lineup
    //Task<MatchLineup> CreateLineupAsync(int matchId, MatchLineup lineup);

    //Registrar jugador en el lineup
    Task<MatchLineup> AddPlayerToLineupAsync(int matchId, int playerId, MatchLineup lineup);

    // Obtener lineup de un partido
    Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId);

    // Obtener lineup de un equipo
    Task<List<MatchLineup>> GetByTeamIdAsync(int matchId, int teamId);

    //Eliminar jugador del lineup
    Task DeletePlayerAsync(int matchId, int playerId);

}
