using System;
using app.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hospital> Hospitals { get; set; }

    public DbSet<HospitalService> HospitalServices { get; set; }

    public DbSet<HospitalServiceAssignment> HospitalServiceAssignment { get; set; }

    public DbSet<Representative> Representative { get; set; }
    public DbSet<PlaceDetails> PlaceDetails { get; set; }
    public DbSet<PlacePrediction> PlacePrediction { get; set; } = null!;
    public DbSet<PlaceResult> PlaceResults { get; set; } = null!;
   public DbSet<UserLocation> UserLocation { get; set; } = null!;


}