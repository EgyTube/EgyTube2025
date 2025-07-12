namespace EgyTube.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public Video? Video { get; set; }
        public string? UserId { get; set; }
    }
}