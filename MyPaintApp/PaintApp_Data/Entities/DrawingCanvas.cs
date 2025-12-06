using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintApp_Data.Entities
{
    public class DrawingCanvas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "Untitled";

        // Kích thước canvas
        public double Width { get; set; }
        public double Height { get; set; }
        public string BackgroundColor { get; set; } = "#FFFFFF";

        public string DataJson { get; set; } = "";

        public string ThumbnailImage { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;

        public int UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}