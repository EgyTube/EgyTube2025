using System.Security.Claims;
using EgyTube.Data;
using EgyTube.Models;
using EgyTube.Services;
using EgyTube.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgyTube.Controllers;

public class VideosController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly IWebHostEnvironment _env;
    private readonly IVideoThumbService _thumb;
    private readonly ILogger<VideosController> _log;

    public VideosController(ApplicationDbContext ctx,
                            IWebHostEnvironment env,
                            IVideoThumbService thumb,
                            ILogger<VideosController> log)
    {
        _ctx = ctx;
        _env = env;
        _thumb = thumb;
        _log = log;
    }

    /*────────── الصفحة الرئيسية ──────────*/
    public async Task<IActionResult> Index()
        => View(await _ctx.Videos
                         .OrderByDescending(v => v.UploadedAt)
                         .ToListAsync());

    /*────────── تشغيل فيديو ──────────*/
    public async Task<IActionResult> Play(int id)
    {
        var video = await _ctx.Videos.FindAsync(id);
        if (video is null) return NotFound();

        video.Views++;
        await _ctx.SaveChangesAsync();

        var others = await _ctx.Videos
                              .Where(v => v.Id != id)
                              .OrderByDescending(v => v.UploadedAt)
                              .Take(12)
                              .ToListAsync();

        return View(new PlayViewModel { Current = video, Others = others });
    }

    /*────────── فيديوهاتي ──────────*/
    [Authorize]
    public async Task<IActionResult> MyVideos()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var list = await _ctx.Videos
                            .Where(v => v.UploaderId == uid)
                            .OrderByDescending(v => v.UploadedAt)
                            .ToListAsync();
        return View("Index", list);
    }

    /*────────── بحث ──────────*/
    public IActionResult Search(string? query)
    {
        var q = string.IsNullOrWhiteSpace(query)
            ? _ctx.Videos
            : _ctx.Videos.Where(v => v.Title.Contains(query));

        return View("Index",
            q.OrderByDescending(v => v.UploadedAt).ToList());
    }

    /*────────── إعجاب AJAX ──────────*/
    [Authorize, HttpPost]
    public async Task<IActionResult> Like(int id)
    {
        var v = await _ctx.Videos.FindAsync(id);
        if (v is null) return NotFound();

        v.Likes++;
        await _ctx.SaveChangesAsync();
        return Json(new { likes = v.Likes });
    }

    /*────────── رفع فيديو ──────────*/
    [Authorize]
    public IActionResult Create() => View(new VideoCreateViewModel());

    [Authorize, HttpPost, ValidateAntiForgeryToken,
     RequestSizeLimit(1_073_741_824)] // 1 GB
    public async Task<IActionResult> Create(VideoCreateViewModel vm)
    {
        if (!ModelState.IsValid || vm.VideoFile is null) return View(vm);

        var ext = Path.GetExtension(vm.VideoFile.FileName).ToLowerInvariant();
        if (ext is not ".mp4" and not ".webm")
        {
            ModelState.AddModelError("",
                "يرجى رفع ملف MP4 أو WebM فقط.");
            return View(vm);
        }

        // ❶ حفظ الفيديو
        var vidsDir = Path.Combine(_env.WebRootPath, "videos");
        Directory.CreateDirectory(vidsDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(vidsDir, fileName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await vm.VideoFile.CopyToAsync(fs);

        // ❷ توليد المصغّرة PNG والمدة
        double duration;
        string thumbPath;
        try
        {
            (duration, thumbPath) = await _thumb.GenerateAsync(fullPath);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex,
                "تعذّر توليد مصغّرة – سيتم استخدام صورة افتراضيّة");
            duration = 0;
            thumbPath = "/images/thumb-placeholder.png";
        }

        // ❸ إنشاء السجلّ
        var video = new Video
        {
            Title = vm.Title,
            FilePath = $"/videos/{fileName}",
            ThumbnailPath = thumbPath,              // ← .png
            Duration = duration,
            UploadedAt = DateTime.UtcNow,
            UploaderId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _ctx.Add(video);
        await _ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
