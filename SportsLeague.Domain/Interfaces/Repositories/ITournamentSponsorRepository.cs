using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
{
    Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentIdAsync(int tournamentId);
    Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId);
    Task<IEnumerable<TournamentSponsor>> SearchByTournamentNameAsync(string tournamentName);
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
}
