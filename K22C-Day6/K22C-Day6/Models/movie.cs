using System.ComponentModel.DataAnnotations;

namespace K22C_Day6.Models
{
    public class movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public int GenreId { get; set; }
        public genre? Genre { get; set; }

    }
}
