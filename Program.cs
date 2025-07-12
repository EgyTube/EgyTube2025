using EgyTube.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using EgyTube.Services;
var builder = WebApplication.CreateBuilder(args);

/*─────────────────────── 1) الخدمات ───────────────────────*/

// ★  قاعدة البيانات (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ★  هوية ASP.NET (تسجيل/دخول)
builder.Services.AddDefaultIdentity<IdentityUser>(opts =>
{
    opts.SignIn.RequireConfirmedAccount = false;   // لا نطلب تأكيد بريد
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ★  مصادقة Google OAuth (زر «Sign in with Google»)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var g = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

// ★  MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

bool ffmpegExists =
    Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator)
        .Any(p => File.Exists(Path.Combine(p, "ffmpeg.exe"))) ||
    File.Exists(Path.Combine(builder.Environment.ContentRootPath, "ffmpeg.exe"));

if (ffmpegExists)
    builder.Services.AddSingleton<IVideoThumbService, VideoThumbService>();
else
    builder.Services.AddSingleton<IVideoThumbService, NoopVideoThumbService>();

// ★  حد رفع الملفات (مثال: 2 GB)
builder.Services.Configure<FormOptions>(o =>
    o.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024);

// ★  أنواع فيديو إضافية لخدمة static files
builder.Services.AddSingleton<IContentTypeProvider>(_ =>
{
    var p = new FileExtensionContentTypeProvider();
    p.Mappings[".webm"] = "video/webm";
    p.Mappings[".mkv"] = "video/x-matroska";
    return p;
});

/*─────────────────────── 2) بناء التطبيق ───────────────────────*/

var app = builder.Build();

/*─────────────────────── 3) خط الأنابيب ───────────────────────*/

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// يطبع مسار الاتصال الفعلي للتأكد
app.Logger.LogInformation("📡 Connection = {cs}",
    builder.Configuration.GetConnectionString("DefaultConnection"));

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = app.Services.GetRequiredService<IContentTypeProvider>()
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // لتشغيل ملفات الفيديو والصور

/*─────────────────────── 4) المسارات ───────────────────────*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Videos}/{action=Index}/{id?}");

app.MapRazorPages();   // صفحات الهوية (Login / Register)

app.Run();
