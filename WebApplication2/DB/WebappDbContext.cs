using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DB;

public partial class WebappDbContext : DbContext
{
    public WebappDbContext()
    {
    }

    public WebappDbContext(DbContextOptions<WebappDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Viewer> Viewers { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    public virtual DbSet<VotingEvent> VotingEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=BYADMINPRESENTS;Initial Catalog=WEBAppDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.Participants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Participant_Project");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasOne(d => d.VotingEvent).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_VotingEvent");
        });

        modelBuilder.Entity<Viewer>(entity =>
        {
            entity.HasOne(d => d.VotingEvent).WithMany(p => p.Viewers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Viewer_VotingEvent");
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.Votes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Project");

            entity.HasOne(d => d.Viewer).WithMany(p => p.Votes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Viewer");
        });

        modelBuilder.Entity<VotingEvent>(entity =>
        {
            entity.HasOne(d => d.Organizer).WithMany(p => p.VotingEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VotingEvent_Organizer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
