using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Viewer")]
public partial class Viewer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(60)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("unique_key")]
    [StringLength(150)]
    public string? UniqueKey { get; set; }

    [Column("votingEvent_id")]
    public int VotingEventId { get; set; }

    [InverseProperty("Viewer")]
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [ForeignKey("VotingEventId")]
    [InverseProperty("Viewers")]
    public virtual VotingEvent VotingEvent { get; set; } = null!;
}
