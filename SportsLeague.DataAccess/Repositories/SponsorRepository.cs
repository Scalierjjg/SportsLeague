using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
{
    public SponsorRepository(LeagueDbContext context) : base(context)
    {

    }

    public async Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category)
    {

        return await _dbSet

        .Where(s => s.Category == category)

        .ToListAsync();

    }

    public async Task<IEnumerable<Tournament>> GetByIdWithTournamentAsync(int id)
    {
        return await _dbSet
        .Where(s => s.Id == id)
        .SelectMany(s => s.TournamentSponsors)
        .Select(ts => ts.Tournament)
        .ToListAsync();
    }

    public async Task<Sponsor?> SearchByNameAsync(string name)
    {
        return await _dbSet
        .FirstOrDefaultAsync(s => s.Name.ToLower().Contains(name.ToLower()));
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Sponsor>> GetSponsorsByTournamentIdAsync(int tournamentId)
    {
        return await _dbSet
        .Where(s => s.TournamentSponsors
        .Any(ts => ts.TournamentId == tournamentId))
        .ToListAsync();
    }

}
