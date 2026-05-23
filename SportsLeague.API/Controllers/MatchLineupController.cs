using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]

[Route("api/match/{matchId}")]
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _matchLineupService;
    private readonly IMapper _mapper;

    public MatchLineupController(
        IMatchLineupService matchLineupService, IMapper mapper)
    {
        _matchLineupService = matchLineupService;
        _mapper = mapper;
    }

    [HttpPost("lineup")]
    public async Task<ActionResult<MatchLineupResponseDTO>> AddPlayerToLineup(
        int matchId, MatchLineupRequestDTO dto)
    {
        try
        {
            var lineup = _mapper.Map<MatchLineup>(dto);
            var created = await _matchLineupService.AddPlayerToLineupAsync(matchId, dto.PlayerId, lineup);
            return StatusCode(StatusCodes.Status201Created, _mapper.Map<MatchLineupResponseDTO>(created));
        }
        catch (KeyNotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        }
    }

    [HttpGet("lineup")]
    public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByMatchId(int matchId)
    {
        try
        {
            var lineups = await _matchLineupService.GetByMatchIdAsync(matchId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("lineup/team/{teamId}")]
    public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByTeamId(int matchId, int teamId)
    {
        try
        {
            var lineups = await _matchLineupService.GetByTeamIdAsync(matchId, teamId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("lineup/{id}")]
    public async Task<IActionResult> DeletePlayerFromLineup(int matchId, int id)
    {
        try
        {
            await _matchLineupService.DeletePlayerAsync(matchId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
