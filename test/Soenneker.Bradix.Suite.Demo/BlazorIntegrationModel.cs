using System.ComponentModel.DataAnnotations;

namespace Soenneker.Bradix.Suite.Demo;

public sealed partial class BlazorIntegrationModel
{
    [Required(ErrorMessage = "Choose a city.")]
    public string? City { get; set; }
}
