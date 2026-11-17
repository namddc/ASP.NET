using System.ComponentModel.DataAnnotations;

namespace K22C_Day6.Models

{
    public class genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<movie>? Movies { get; set; }
    }
}
