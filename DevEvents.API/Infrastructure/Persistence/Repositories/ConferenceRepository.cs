using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevEvents.API.Infrastructure.Persistence.Repositories;

public class ConferenceRepository : IConferenceRepository
{
    private readonly AppDbContext _dbContext;

    public ConferenceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Conference conference)
    {
        _dbContext.Conferences.Add(conference);
        await _dbContext.SaveChangesAsync();

        return conference.Id;
    }

    public async Task AddRegistrationFromAttendee(int idConference, Attendee attendee)
    {
        await _dbContext.Attendees.AddAsync(attendee);
        await _dbContext.SaveChangesAsync();

        var registration = new Registration(idConference, attendee.Id);

        await _dbContext.Registrations.AddAsync(registration);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddSpeaker(Speaker speaker)
    {
        await _dbContext.Speakers.AddAsync(speaker);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var conference = await GetById(id);

        conference.MarkAsDeleted();

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await _dbContext.Conferences.AnyAsync(c => c.Id == id);
    }

    public async Task<Conference[]> GetAll()
    {
        var conferences = await _dbContext.Conferences
                            .Include(c => c.Speakers)
                            .Include(c => c.Registrations)
                        .ToArrayAsync();

        return conferences;
    }

    public async Task<Conference?> GetById(int id)
    {
        var conference = await _dbContext.Conferences
                    .Include(c => c.Speakers)
                    .Include(c => c.Registrations)
                        .ThenInclude(r => r.Attendee)
                    .SingleOrDefaultAsync(c => c.Id == id);

        return conference;
    }

    public async Task Update(Conference conference)
    {
        _dbContext.Conferences.Update(conference);
        await _dbContext.SaveChangesAsync();
    }
}
