using System.ComponentModel.DataAnnotations;

namespace PawnCloud.Configuration.Dto;

public class ChangeUiThemeInput
{
    [Required]
    [StringLength(32)]
    public string Theme { get; set; }
}
