using System.Security.Claims;
using EgyTube.Data;
using EgyTube.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgyTube.Controllers;

[Authorize]
public class CommentsController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public CommentsController(ApplicationDbContext ctx) => _ctx = ctx;

    /*──────────────── إضافة تعليق ────────────────*/
    [HttpPost]
    public async Task<IActionResult> Add(int videoId, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return BadRequest("لا يمكن إرسال تعليق فارغ.");

        /* جلب معرف المستخدم والتحقق */
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();   // لن يصل هنا أساسًا مع [Authorize]، لكن احتياطًا

        var comment = new Comment
        {
            VideoId = videoId,
            Body = body.Trim(),
            UserId = userId                          // الآن غير قابل لـ null
        };

        _ctx.Add(comment);
        await _ctx.SaveChangesAsync();

        /* بناء HTML صغير لإعادته عبر AJAX */
        var userName = User.Identity?.Name ?? "مستخدم";
        var html = $@"
<li class='mb-2'>
  <strong>{userName}</strong>
  <small class='text-muted'>{comment.PostedAt:g}</small><br/>
  {System.Net.WebUtility.HtmlEncode(comment.Body)}
</li>";

        return Content(html, "text/html; charset=utf-8");
    }

    /*────────────── قائمة التعليقات (Partial) ──────────────*/
    [AllowAnonymous]
    public async Task<IActionResult> List(int videoId)
    {
        var items = await _ctx.Comments
                              .Where(c => c.VideoId == videoId)
                              .Include(c => c.User)
                              .OrderByDescending(c => c.PostedAt)
                              .ToListAsync();

        return PartialView("_CommentsList", items);
    }
}
