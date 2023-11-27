using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("VotingEvent")]
public partial class VotingEvent
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(150)]
    public string? Description { get; set; }

    [Column("datetime", TypeName = "datetime")]
    public DateTime Datetime { get; set; }

    [Column("location")]
    [StringLength(70)]
    public string Location { get; set; } = null!;

    [Column("organizer_id")]
    public int OrganizerId { get; set; }

    [ForeignKey("OrganizerId")]
    [InverseProperty("VotingEvents")]
    public virtual Organizer Organizer { get; set; } = null!;

    [InverseProperty("VotingEvent")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("VotingEvent")]
    public virtual ICollection<Viewer> Viewers { get; set; } = new List<Viewer>();
}
