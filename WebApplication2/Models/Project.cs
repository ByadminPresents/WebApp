using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Project")]
public partial class Project
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [StringLength(50)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(300)]
    public string Description { get; set; } = null!;

    [Column("votingEvent_id")]
    public int VotingEventId { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    [InverseProperty("Project")]
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [ForeignKey("VotingEventId")]
    [InverseProperty("Projects")]
    public virtual VotingEvent VotingEvent { get; set; } = null!;
}
