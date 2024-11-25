namespace ConferenceRegistration.Pages;

using ConferenceRegistration.Data;

public class ListParticipantsModel(ConferenceRegistrationDbContext context) : PageModel
{
    public IList<Participant> Participants { get; private set; } = new List<Participant>();

    public void OnGet()
    {
        Participants = context.Participants.OrderBy(p => p.ParticipantId).ToList();
    }
}