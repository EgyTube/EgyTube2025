using System.Collections.Generic;

namespace EgyTube.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public List<Video>? Videos { get; set; }
    }
}