namespace EgyTube.Services
{
    public interface IVideoThumbService
    {
        /// <summary>تُعيد (المدة بالثواني، ومسار الصورة المصغّرة)</summary>
        Task<(double duration, string thumbPath)> GenerateAsync(string fullVideoPath);
    }
}
