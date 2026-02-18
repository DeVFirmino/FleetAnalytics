using System.ComponentModel.DataAnnotations;

namespace FleetAnalytics.Application.DTOs;

public class SaveDriverDto
{
    [Required(ErrorMessage = "First name is required.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "License number is required.")]
    public string? LicenseNumber { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
}
