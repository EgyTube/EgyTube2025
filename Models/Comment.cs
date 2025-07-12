// Models/Comment.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EgyTube.Models;
public class Comment
{
    public int Id { get; set; }

    [Required, StringLength(500)]
    public string Body { get; set; } = "";

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    // links
    public int VideoId { get; set; }
    public Video? Video { get; set; }

    public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
}
