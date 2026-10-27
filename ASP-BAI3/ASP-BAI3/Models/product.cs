namespace ASP_BAI3.Models
{
    public class product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public String desription { get; set; } = string.Empty;
    }
}
