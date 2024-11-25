namespace ConferenceRegistration.Data;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class ConferenceRegistrationDbContext : DbContext
{
    public ConferenceRegistrationDbContext(
        DbContextOptions<ConferenceRegistrationDbContext> options)
    : base(options)
    {
    }

    public DbSet<Participant> Participants => Set<Participant>();
}