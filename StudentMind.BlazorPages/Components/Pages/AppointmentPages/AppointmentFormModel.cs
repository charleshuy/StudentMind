using System.ComponentModel.DataAnnotations;

public class AppointmentFormModel
{
    [Required(ErrorMessage = "Please select a psychologist.")]
    public string PsychologistId { get; set; } = "";

    [Required(ErrorMessage = "Please select a user.")]
    public string UserId { get; set; } = "";
}