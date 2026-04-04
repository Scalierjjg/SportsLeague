using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ISponsorRepository : IGenericRepository<Sponsor>
{
    Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category);
    Task<IEnumerable<Tournament>> GetByIdWithTournamentAsync(int id);
    Task<Sponsor?> SearchByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<IEnumerable<Sponsor>> GetSponsorsByTournamentIdAsync(int tournamentId);
    
}
