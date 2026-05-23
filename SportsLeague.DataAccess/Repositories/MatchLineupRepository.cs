using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
{

    public MatchLineupRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId)
    {
        return await _dbSet
            .Where(ml => ml.MatchId == matchId)
            .Include(ml => ml.Player)
                .ThenInclude(p => p!.Team)
            .ToListAsync();
    }
    
    public async Task<List<MatchLineup>> GetByTeamIdAsync(int matchId, int teamId)
    {
        return await _dbSet
        .Where(ml => ml.MatchId == matchId && ml.Player!.TeamId == teamId)
        .Include(ml => ml.Player)
        .ToListAsync();
    }

    public async Task<MatchLineup> AddPlayerToLineupAsync(MatchLineup lineup)
    {
        var createdLineup = new MatchLineup
        {
            MatchId = lineup.MatchId,
            PlayerId = lineup.PlayerId,
            Position = lineup.Position,
            IsStarter = lineup.IsStarter
        };

        await _dbSet.AddAsync(lineup);
        await _context.SaveChangesAsync();

        return createdLineup;
    }

    public async Task<MatchLineup?> GetByMatchAndPlayerAsync(int matchId, int playerId)
    {
        return await _dbSet
            .Where(ml => ml.MatchId == matchId && ml.PlayerId == playerId)
            .FirstOrDefaultAsync();
    }

}
