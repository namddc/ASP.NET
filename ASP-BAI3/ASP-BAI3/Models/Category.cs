namespace ASP_BAI3.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;// dấu hỏi hay = để tránh lỗi null;
        public List<product>? products { get; set; }
    }
}
