using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintApp_Data.Entities
{
    public class UserProfile
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Tên không được quá 50 ký tự")]
        public string UserName { get; set; }

        public string AvatarIcon { get; set; } = "Assets/DefaultAvatar.jpg";

        public double DefaultCanvasWidth { get; set; } = 800;
        public double DefaultCanvasHeight { get; set; } = 600;

        public string ThemePreference { get; set; } = "System"; // Light, Dark, System

        public string DefaultCanvasColor { get; set; } = "#FFFFFFFF";
        public Double DefaultStrokeSize { get; set; } = 2.0;
        public string DefaultStrokeColor { get; set; } = "#FF000000";
        public int DefaultStrokeStyle { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastAccessed { get; set; } = DateTime.Now;

        // --- Navigation Properties (Quan hệ với các bảng khác) ---
        // public virtual ICollection<DrawingCanvas> DrawingCanvases { get; set; }

    }
}
