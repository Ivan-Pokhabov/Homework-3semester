namespace ConferenceRegistration.Data;

using System.ComponentModel.DataAnnotations;

public class Participant
{
    public int ParticipantId { get; set; }

    [Required(ErrorMessage = "Please enter your name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter your email")]
    [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please specify whether you'll be a speaker or just attending")]
    public bool IsSpeaker { get; set; }
}