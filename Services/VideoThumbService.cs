using CliWrap;
using CliWrap.Buffered;
using EgyTube.Services;

namespace EgyTube.Services
{
    public class VideoThumbService : IVideoThumbService
    {
        private readonly IWebHostEnvironment _env;

        public VideoThumbService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(double, string)> GenerateAsync(string videoPath)
        {
            // ❶ حساب مدة الفيديو
            var probe = await Cli.Wrap("ffprobe")
                .WithArguments($"-v error -show_entries format=duration -of csv=p=0 \"{videoPath}\"")
                .ExecuteBufferedAsync();

            var seconds = double.Parse(probe.StandardOutput.Trim());

            // ❷ توليد صورة مصغّرة من الثانية 1
            var thumbsDir = Path.Combine(_env.WebRootPath, "thumbs");
            Directory.CreateDirectory(thumbsDir);

            var thumbFile = $"{Path.GetFileNameWithoutExtension(videoPath)}.jpg";
            var thumbFullPath = Path.Combine(thumbsDir, thumbFile);

            await Cli.Wrap("ffmpeg")
                .WithArguments($"-ss 1 -i \"{videoPath}\" -frames:v 1 -q:v 2 \"{thumbFullPath}\" -y")
                .ExecuteBufferedAsync();

            return (seconds, $"/thumbs/{thumbFile}");
        }
    }
}
