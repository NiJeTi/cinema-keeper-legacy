using System;

using CinemaKeeper.Persistence.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Persistence;

public class AbobaContext : DbContext
{
    public AbobaContext(DbContextOptions<AbobaContext> options) : base(options)
    {
        Console.Out.WriteLine("AbobaContext created");
    }

    public DbSet<Quote> Quotes { get; set; }
}
