using Microsoft.AspNetCore.Identity;

public class Video
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string FilePath { get; set; } = "";

    public string? ThumbnailPath { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int Views { get; set; } = 0;

    public int Likes { get; set; } = 0;

    // 🔑 مفتاح أجنبي يشير إلى جدول Users
    public string? UploaderId { get; set; }

    // 🧭 التنقُّل إلى كيان المستخدم
    public IdentityUser? Uploader { get; set; }

    // مدة الفيديو بالثواني
    public double Duration { get; set; } = 0;
}
