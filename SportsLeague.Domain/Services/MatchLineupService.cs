using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Numerics;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly MatchValidationHelper _validationHelper;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchRepository matchRepository,
        IMatchLineupRepository matchLineupRepository,
        MatchValidationHelper validationHelper,
        ILogger<MatchLineupService> logger)
    {
        _matchRepository = matchRepository;
        _matchLineupRepository = matchLineupRepository;
        _validationHelper = validationHelper;
        _logger = logger;
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId)
    {
        _logger.LogInformation("Getting lineup for match {MatchId}", matchId);
        var lineups = await _matchLineupRepository.GetByMatchIdAsync(matchId);

        if (lineups == null || !lineups.Any())
        {
            _logger.LogWarning("No lineup found for the id {MatchId}", matchId);
            throw new KeyNotFoundException($"No lineup found for the ID {matchId}");
        }
        
        return lineups;
    }

    public async Task<List<MatchLineup>> GetByTeamIdAsync(int matchId, int teamId)
    {
        _logger.LogInformation("Getting lineup for match {MatchId} and team {TeamId}", matchId, teamId);
        var lineups = await _matchLineupRepository.GetByTeamIdAsync(matchId, teamId);
        if (lineups == null || !lineups.Any())
        {
            _logger.LogWarning("No lineup found for the id {MatchId} and team {TeamId}", matchId, teamId);
            throw new KeyNotFoundException($"No lineup found for the ID {matchId} and team {teamId}");
        }
        
        return lineups;
    }

    public async Task<MatchLineup> AddPlayerToLineupAsync(int matchId, int playerId, MatchLineup lineup)
    {
        var match = await _validationHelper.ValidateMatchForLineupAsync(matchId);
        var player = await _validationHelper.ValidatePlayerInMatchAsync(playerId, match);

        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
        {
            _logger.LogWarning("Player {PlayerId} (Team {PlayerTeamId}) does not belong to Home ({HomeTeamId}) or Away ({AwayTeamId})",
                player.Id, player.TeamId, match.HomeTeamId, match.AwayTeamId);
            throw new InvalidOperationException("El jugador no pertenece a ninguno de los dos equipos de este encuentro.");
        }

        // Validar que no esté ya inscrito

        var existing = await _matchLineupRepository

        .GetByMatchAndPlayerAsync(matchId, playerId);

        if (existing != null)
        {

            throw new InvalidOperationException(

            "This player is already on the lineup");

        }

        var existingLineups = await _matchLineupRepository.GetByMatchIdAsync(matchId);

        // Maximun 11 starters per team
        if (lineup.IsStarter && existingLineups != null)
        {
            // Contamos usando el .Count() normal de LINQ
            int currentStartersCount = existingLineups.Count(ml =>
                ml.Player?.TeamId == player.TeamId && ml.IsStarter);

            if (currentStartersCount >= 11)
            {
                _logger.LogWarning("Team {TeamId} already has 11 starters in match {MatchId}", player.TeamId, matchId);
                throw new InvalidOperationException("No se pueden agregar más titulares. El equipo ya tiene 11 jugadores en la cancha.");
            }
        }


        lineup.MatchId = matchId;
        lineup.PlayerId = playerId;

        var createdLineup = await _matchLineupRepository.CreateAsync(lineup);

        _logger.LogInformation("Player {PlayerId} successfully added to match {MatchId}.", lineup.PlayerId, matchId);

        var refreshedLineups = await _matchLineupRepository.GetByMatchIdAsync(matchId);

        //Brings the team information
        return refreshedLineups!.First(ml => ml.PlayerId == playerId);


    }

    public async Task DeletePlayerAsync(int matchId, int playerId)
    {
        var match = await _validationHelper.ValidateMatchForLineupAsync(matchId);
        await _validationHelper.ValidatePlayerInMatchAsync(playerId, match);

        var lineups = await _matchLineupRepository.GetByMatchIdAsync(matchId);
        var lineupEntry = lineups?.FirstOrDefault(ml => ml.PlayerId == playerId);

        if (lineupEntry != null)
        {
            await _matchLineupRepository.DeleteAsync(lineupEntry.Id);
        }

        _logger.LogInformation("Player {PlayerId} removed from match {MatchId} lineup", playerId, matchId);
    }

}
