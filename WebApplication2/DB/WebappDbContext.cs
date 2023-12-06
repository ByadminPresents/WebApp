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

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<UniqueKey> UniqueKeys { get; set; }

    public virtual DbSet<Viewer> Viewers { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    public virtual DbSet<VotingEvent> VotingEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=BYADMINLAPTOP;Initial Catalog=WEBAppDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.HasOne(d => d.Email).WithMany(p => p.Organizers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizer_Email");

            entity.HasOne(d => d.UniqueKey).WithMany(p => p.Organizers).HasConstraintName("FK_Organizer_UniqueKey");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasOne(d => d.Email).WithMany(p => p.Participants).HasConstraintName("FK_Participant_Email");

            entity.HasOne(d => d.Project).WithMany(p => p.Participants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Participant_Project");

            entity.HasOne(d => d.UniqueKey).WithMany(p => p.Participants).HasConstraintName("FK_Participant_UniqueKey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasOne(d => d.VotingEvent).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_VotingEvent");
        });

        modelBuilder.Entity<Viewer>(entity =>
        {
            entity.HasOne(d => d.Email).WithMany(p => p.Viewers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Viewer_Email");

            entity.HasOne(d => d.UniqueKey).WithMany(p => p.Viewers).HasConstraintName("FK_Viewer_UniqueKey");

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
