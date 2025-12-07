using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintApp_Data.Entities
{
    public class ShapeTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Type { get; set; } // Rectangle, Ellipse, Polygon...

        public string ShapeJson { get; set; }

        public string IconPreview { get; set; }

        public int UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}