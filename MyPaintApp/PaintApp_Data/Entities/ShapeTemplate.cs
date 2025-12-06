using System.ComponentModel.DataAnnotations;

namespace PaintApp_Data.Entities
{
    public class ShapeTemplate
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string SerializedData { get; set; }
    }
}