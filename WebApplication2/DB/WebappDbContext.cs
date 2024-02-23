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

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    public virtual DbSet<VotingEvent> VotingEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=BYADMINPRESENTS; Database=WEBAppDB; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.VotingEventId).HasColumnName("votingEvent_id");

            entity.HasOne(d => d.VotingEvent).WithMany(p => p.Projects)
                .HasForeignKey(d => d.VotingEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_VotingEvent");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(60)
                .HasColumnName("login");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .HasColumnName("password");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.UniqueKey)
                .HasMaxLength(36)
                .HasColumnName("uniqueKey");
            entity.Property(e => e.VotingEventId).HasColumnName("votingEvent_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Users)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_User_Project");

            entity.HasOne(d => d.VotingEvent).WithMany(p => p.Users)
                .HasForeignKey(d => d.VotingEventId)
                .HasConstraintName("FK_User_VotingEvent");
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.ToTable("Vote");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Criteria).HasColumnName("criteria");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ViewerId).HasColumnName("viewer_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Votes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Project");

            entity.HasOne(d => d.Viewer).WithMany(p => p.Votes)
                .HasForeignKey(d => d.ViewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_User");
        });

        modelBuilder.Entity<VotingEvent>(entity =>
        {
            entity.ToTable("VotingEvent");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Criterias)
                .HasMaxLength(150)
                .HasColumnName("criterias");
            entity.Property(e => e.Datetime)
                .HasColumnType("datetime")
                .HasColumnName("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.Location)
                .HasMaxLength(70)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");

            entity.HasOne(d => d.Organizer).WithMany(p => p.VotingEvents)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VotingEvent_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
