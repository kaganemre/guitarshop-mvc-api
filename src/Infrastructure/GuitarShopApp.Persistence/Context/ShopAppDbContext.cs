using GuitarShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GuitarShopApp.Persistence.Context;

public class ShopAppDbContext : DbContext
{
    public ShopAppDbContext(DbContextOptions<ShopAppDbContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(
                new List<User> {
                    new() { Id =1, FullName="Admin", Email="info@adminuser.com", Password="Password_536", RoleName="admin", EmailConfirmed=true},
                    new() { Id =2, FullName="Paul Gilbert", Email="info@paulgilbert.com", Password="Paul_536", EmailConfirmed=true}
                }
            );

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
              .HasIndex(u => u.Url)
              .IsUnique();

        modelBuilder.Entity<Category>().HasData(
                new List<Category> {
                    new() { Id =1, Name="Guitar", Url="guitar"},
                    new() { Id =2, Name="Amplifier", Url="amplifier"},
                    new() { Id =3, Name="Pedal", Url="pedal"}
                }
            );

        modelBuilder.Entity<Product>().HasData(
                new List<Product>
                {
                    new() { Id=1, Name="Jackson RR24", Price=4500, Description="Harkening back to the early '90s when import Jackson® guitars were manufactured exclusively in Japan, we introduce the all-new Jackson MJ Series - an exciting and innovative collection of instruments attentively crafted in Japan. The MJ Series blends Jackson's world-renowned legacy of designing high-performance instruments with an assortment of top-tier features at a competitive price point.", Image = "1.png", CategoryId=1},
                    new() { Id=2, Name="Dean Dimebag CHF", Price=3000, Description="The Dean Dimebag CHF Electric Guitar puts Dime's favorite body design behind a stunning graphic. The sleek, set mahogany neck with pau ferro fretboard is designed for speed, while the Seymour Duncan SH13 Dimebucker and  DMT Design neck humbuckers deliver all the high-output sonics you'll need. It also features dot inlays, classic V headstock shape, 24-3/4' scale, and Grover tuners. The Floyd Rose Special bridge will keep you in fine dive bombing form.", Image="4.png", CategoryId=1, IsHome=true},
                    new() { Id=3, Name="Gibson Slash Les Paul", Price=6000, Description="Gibson and Slash are proud to present the Slash Collection Gibson Les Paul™ Standard. It represents influential Gibson guitars Slash has used during his career, inspiring multiple generations of players around the world. The Slash Collection of Gibson guitars can be seen live on stage with Slash today.", Image = "2.png", CategoryId=1, IsHome=true},
                    new() { Id=4, Name="ESP Alexi Hexed", Price=4000, Description="Created by hand, one at a time by the artisans of the ESP Custom Shop in Japan, the ESP Alexi Hexed is the identical model designed and played by one of the most beloved and influential figures in metal music: Alexi Laiho of Children of Bodom/Bodom After Midnight. The ESP Alexi Hexed is offered in an offset V shape with neck-thru-body construction at 25.5” scale.", Image = "3.png", CategoryId=1, IsHome=true},
                    new() { Id=5, Name="Charvel HSS HT RW", Price=5000, Description="Guitar virtuoso Jake E Lee blazed trails in the 1980s with Ratt and Rough Cutt before landing his legendary gig as Ozzy Osbourne's lead guitarist. His acclaimed career continued with Badlands and now Red Dragon Cartel, and Charvel has been there every step of the way. Charvel honors the dynamic guitarist with the new Jake E Lee Signature Pro-Mod So-Cal, based on the distinctive white 'Charvel-ized' instrument he acquired back in 1975.", Image="5.png", CategoryId=1},
                    new() { Id=6, Name="Marshall MG30GFX 30W", Price=1500, Description="The Marshall MG30GFX Gold 30W Guitar Combo features the iconic 'gold' front panel design and delivers 30 watts of portable Marshall tone with added sound effects and reverb. It is the ideal amplifier for band rehearsals and for small/medium gigs, with some added features making it perfect for practice.", Image="6.png", CategoryId=2},
                    new() { Id=7, Name="Boss TR-2 Tremolo Pedal", Price=1000, Description="The Boss TR-2 Tremolo Guitar effects pedal delivers a vintage-style tremolo that can be fully controlled thanks to its intuitive controls. A designated Wave, Rate, and depth dial ensure every parameter can be intuitively altered to the performer's preference", Image="7.png", CategoryId=3}
                }
            );

    }


}
