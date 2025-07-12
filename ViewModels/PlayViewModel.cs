using EgyTube.Models;          // ← مهم حتى يتعرّف على Video و Comment
using System.Collections.Generic;

namespace EgyTube.ViewModels
{
    public class PlayViewModel
    {
        public Video Current { get; set; } = null!;   // الفيديو المعروض
        public List<Video> Others { get; set; } = new(); // الفيديوهات الجانبية
        public List<Comment> Comments { get; set; } = new(); // التعليقات (اختياري)
    }
}
