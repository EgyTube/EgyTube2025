using System.ComponentModel.DataAnnotations;

public class VideoCreateViewModel
{
    [Required, StringLength(120)]
    public string Title { get; set; } = "";

    [Required]
    public IFormFile VideoFile { get; set; } = null!;
}