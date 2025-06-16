using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Data
{
    public class MovieTicketContext : IdentityDbContext<ApplicationUser>
    {
        public MovieTicketContext (DbContextOptions<MovieTicketContext> options)
            : base(options)
        {
        }        public DbSet<MovieTicketSystem.Models.Movie> Movies { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Screen> Screens { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Showtime> Showtimes { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Seat> Seats { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Booking> Bookings { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Ticket> Tickets { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.Payment> Payments { get; set; } = default!;
        public DbSet<MovieTicketSystem.Models.PaymentCard> PaymentCards { get; set; } = default!;protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure foreign key relationships to avoid multiple cascade paths
            
            // Ticket -> Seat: No Action (to avoid cycle with Screen -> Seat -> Ticket and Screen -> Showtime -> Booking -> Ticket)
            modelBuilder.Entity<Models.Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ticket -> Booking: No Action (manual deletion required)
            modelBuilder.Entity<Models.Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Showtime -> Screen: No Action (to avoid cycle)
            modelBuilder.Entity<Models.Showtime>()
                .HasOne(st => st.Screen)
                .WithMany(s => s.Showtimes)
                .HasForeignKey(st => st.ScreenId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure decimal precision to avoid warnings
            modelBuilder.Entity<Models.Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Models.Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Models.Showtime>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Models.Ticket>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);            // Keep default cascade for simple parent-child relationships
            // Seat -> Screen: Cascade (default)
            // Booking -> User: Cascade (default)  
            // Booking -> Showtime: Cascade (default)
            // Payment -> Booking: Cascade (default)
            // Showtime -> Movie: Cascade (default)

            // Cấu hình các bảng Identity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "AspNetUsers");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "AspNetRoles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AspNetUserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AspNetUserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AspNetRoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AspNetUserTokens");
            });
        }
    }
}
