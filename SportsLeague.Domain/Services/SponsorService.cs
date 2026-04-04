using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentRepository tournamentRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ILogger<SponsorService> logger)
    {
        _tournamentRepository = tournamentRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _sponsorRepository = sponsorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);

        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)

            _logger.LogWarning("Sponsor with ID {sponsorId} not found", id);

        return sponsor;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)

    {

        //Ensuring that the sponsor name is unique in the database
        var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
        if (exists)
        {
            throw new InvalidOperationException(
            "The sponsor name already exists in the databaes");
        }


        //Ensuring that the email has a correct format
        if (!await IsEmailFormatCorrectAsync(sponsor.ContactEmail))
        {
            throw new InvalidOperationException(
            "El correo electrónico no tiene un formato válido");
        }



        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);

        return await _sponsorRepository.CreateAsync(sponsor);

    }

    public Task<bool> IsEmailFormatCorrectAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult(false);

        var validator = new EmailAddressAttribute();
        return Task.FromResult(validator.IsValid(email));
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {

        var existing = await _sponsorRepository.GetByIdAsync(id);

        if (existing == null)

            throw new KeyNotFoundException($"Sponsor not found with the ID {id}");

        //Ensuring that the sponsor name is unique in the database
        if (!await _sponsorRepository.ExistsByNameAsync(existing.Name))
        {
            throw new InvalidOperationException(
            "The sponsor name already exists in the databaes");
        }

        //Ensuring that the email has a correct format
        if (!await IsEmailFormatCorrectAsync(existing.ContactEmail))
        {
            throw new InvalidOperationException(
            "El correo electrónico no tiene un formato válido");
        }


        existing.Name = sponsor.Name;

        existing.Category = sponsor.Category;

        existing.ContactEmail = sponsor.ContactEmail;

        existing.Phone = sponsor.Phone;

        existing.WebsiteUrl = sponsor.WebsiteUrl;

        existing.UpdatedAt = DateTime.UtcNow;


        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);

        await _sponsorRepository.UpdateAsync(existing);

    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);

        if (existing == null)

            throw new KeyNotFoundException($"Sponsor not found with the ID {id}");


        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);

        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category)
    {

        _logger.LogInformation("Retrieving the sponsors with the category " + category.ToString());
        return await _sponsorRepository.GetByCategoryAsync(category);
    }

    public async Task<IEnumerable<Tournament>> GetSponsorByIdWithTournamentAsync(int id)
    {
        _logger.LogInformation("Retrieving tournaments for sponsor with ID: {SponsorId}", id);

        return await _sponsorRepository.GetByIdWithTournamentAsync(id);
    }

    public async Task<IEnumerable<Sponsor>> GetSponsorsByTournamentIdAsync(int tournamentId)
    {
        _logger.LogInformation("Retrieving sponsors for tournament with ID: {TournamentId}", tournamentId);
        return await _sponsorRepository.GetSponsorsByTournamentIdAsync(tournamentId);
    }

    public async Task<Sponsor?> SearchSponsorByNameAsync(string name)
    {
        _logger.LogInformation("Searching sponsor with name containing: {SponsorName}", name);
        var sponsor = await _sponsorRepository.SearchByNameAsync(name);
        if (sponsor == null)
            _logger.LogWarning("No sponsor found with name containing: {SponsorName}", name);
        return sponsor;
    }

    public async Task RegisterSponsorAsync(int tournamentId, int sponsorId, decimal contractAmount)
    {

        //Tournament must exist

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)

            throw new KeyNotFoundException(

            $"Tournament with ID {tournamentId} was not found");

        //Solo se pueden inscribir equipos en torneos Pending

        if (tournament.Status != TournamentStatus.Pending)
        {

            throw new InvalidOperationException(

            "You can only change Pending tournaments");
        }


        //Sponsor must exist

        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);

        if (!sponsorExists)

            throw new KeyNotFoundException(

            $"Sponsor with ID {sponsorId} was not found");




        // Validar que no esté ya inscrito

        var existing = await _tournamentSponsorRepository

        .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

        if (existing != null)
        {

            throw new InvalidOperationException(

            "This sponsor is already on the tournament");

        }

        if (contractAmount <= 0)
        {
            throw new InvalidOperationException("Contract amount must be greater than 0");
        }


        var tournamentSponsor = new TournamentSponsor
        {

            TournamentId = tournamentId,

            SponsorId = sponsorId,

            ContractAmount = contractAmount

        };


        _logger.LogInformation("Registering sponsor {SponsorId} in tournament {TournamentId}", sponsorId, tournamentId);

        await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

    }

    public async Task UnboundSponsorAsync(int tournamentId, int sponsorId)
    {
        //Tournament must exist

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)

            throw new KeyNotFoundException(

            $"Tournament with ID {tournamentId} was not found");

        //Solo se pueden inscribir equipos en torneos Pending

        if (tournament.Status != TournamentStatus.Pending)
        {

            throw new InvalidOperationException(

            "You can only change Pending tournaments");
        }


        //Sponsor must exist

        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);

        if (!sponsorExists)

            throw new KeyNotFoundException(

            $"Sponsor with ID {sponsorId} was not found");


        // Validar que si esté ya inscrito

        var existing = await _tournamentSponsorRepository

        .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

        if (existing == null)
        {

            throw new KeyNotFoundException($"No registration found for sponsor ID {sponsorId} in tournament ID {tournamentId}");
            

        }
        
        await _tournamentSponsorRepository.DeleteAsync(existing.Id);

        _logger.LogInformation("Unbinding sponsor {SponsorId} from tournament {TournamentId}", sponsorId, tournamentId);
    }
}

    
