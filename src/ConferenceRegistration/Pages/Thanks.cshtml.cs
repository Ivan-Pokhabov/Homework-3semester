using ConferenceRegistration.Data;

namespace ConferenceRegistration.Pages;

public class ThanksModel : PageModel
{
    public Participant Participant { get; set; } = new();

    public void OnGet(Participant participant)
    {
        Participant = participant;
    }
}