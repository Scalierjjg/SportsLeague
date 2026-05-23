using SportsLeague.Domain.Entities;



namespace SportsLeague.Domain.Interfaces.Repositories;



public interface IMatchLineupRepository : IGenericRepository<MatchLineup>
{

    Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId);
    Task<List<MatchLineup>> GetByTeamIdAsync(int matchId, int teamId);
    Task<MatchLineup> AddPlayerToLineupAsync(MatchLineup entity);
    Task<MatchLineup?> GetByMatchAndPlayerAsync(int matchId, int playerId);

}

