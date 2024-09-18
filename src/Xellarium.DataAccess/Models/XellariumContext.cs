using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.DataAccess.Models;

public class XellariumContext : DbContext
{
    public XellariumContext(DbContextOptions<XellariumContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Rule> Rules { get; set; } = null!;
    public DbSet<Collection> Collections { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                    .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<User>()
            .Navigation(u => u.Collections)
            .AutoInclude();

        modelBuilder.Entity<User>()
            .Navigation(u => u.Rules)
            .AutoInclude();

        modelBuilder.Entity<User>()
                    .HasMany(u => u.Collections)
                    .WithOne(c => c.Owner);
        
        modelBuilder.Entity<User>()
                    .HasMany(u => u.Rules)
                    .WithOne(r => r.Owner);

        modelBuilder.Entity<Neighborhood>()
                    .HasKey(n => n.Id);

        modelBuilder.Entity<Neighborhood>()
            .Property(n => n.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Neighborhood>()
            .Property(n => n.Offsets)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Vec2[]>(v, JsonSerializerOptions.Default)!,
                new ValueComparer<IList<Vec2>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()
                )
            );
        
        modelBuilder.Entity<Rule>()
                    .HasKey(r => r.Id);
        
        modelBuilder.Entity<Rule>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Rule>()
            .HasOne(typeof(Neighborhood))
            .WithMany()
            .HasForeignKey("NeighborhoodId");
        
        modelBuilder.Entity<Rule>()
                    .HasMany(r => r.Collections)
                    .WithMany(c => c.Rules);

        modelBuilder.Entity<Rule>()
            .OwnsOne(r => r.GenericRule)
            .Property(r => r.StateTransitions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<IList<Transition>[]>(v, JsonSerializerOptions.Default)!,
                new ValueComparer<IList<Transition>[]>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()
                )
            );
        
        modelBuilder.Entity<Rule>()
                    .HasOne(r => r.Owner)
                    .WithMany(u => u.Rules);
        
        modelBuilder.Entity<Rule>()
            .Navigation(r => r.Owner)
            .AutoInclude();
        
        modelBuilder.Entity<Collection>()
                    .HasKey(c => c.Id);
        
        modelBuilder.Entity<Collection>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Collection>()
                    .HasMany(c => c.Rules)
                    .WithMany(r => r.Collections);
        
        modelBuilder.Entity<Collection>()
                    .HasOne(c => c.Owner)
                    .WithMany(u => u.Collections);
        
        modelBuilder.Entity<Collection>()
            .Navigation(c => c.Owner)
            .AutoInclude();
    }
}