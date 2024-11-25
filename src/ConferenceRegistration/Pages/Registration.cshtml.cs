namespace ConferenceRegistration.Pages;

using ConferenceRegistration.Data;

[BindProperties]
public class RegistrationModel(ConferenceRegistrationDbContext context) : PageModel
{
    public Participant Participant { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        context.Participants.Add(Participant);
        await context.SaveChangesAsync();

        return RedirectToPage("./Thanks", Participant);
    }
}