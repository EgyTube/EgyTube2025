using EgyTube.Services;

namespace EgyTube.Services
{
    /// <summary>
    /// نسخة صامتة تعمل بدون FFmpeg وتعيد صورة افتراضية فقط
    /// </summary>
    public class NoopVideoThumbService : IVideoThumbService
    {
        public Task<(double, string)> GenerateAsync(string _)
        {
            // ترجع مدة = 0 وصورة ثابتة
            return Task.FromResult((0d, "/images/thumb-placeholder.png"));

        }
    }
}
