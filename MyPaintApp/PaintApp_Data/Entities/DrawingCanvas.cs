using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintApp_Data.Entities
{
    public class DrawingCanvas
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Untitled";

        // Kích thước canvas
        public double Width { get; set; }
        public double Height { get; set; }
        public string BackgroundColor { get; set; } = "#FFFFFF"; // Lưu mã Hex

        public string SerializedShapes { get; set; } = "[]";

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}