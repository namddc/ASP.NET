using Microsoft.EntityFrameworkCore;

public class K22C_Day6Context(DbContextOptions<K22C_Day6Context> options) : DbContext(options)
{
    public DbSet<K22C_Day6.Models.genre> genre { get; set; } = default!;
    public DbSet<K22C_Day6.Models.movie> movie { get; set; } = default!;
}
