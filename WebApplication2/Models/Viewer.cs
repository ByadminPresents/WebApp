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

    [Column("email_id")]
    public int EmailId { get; set; }

    [Column("uniqueKey_id")]
    public int? UniqueKeyId { get; set; }

    [Column("votingEvent_id")]
    public int VotingEventId { get; set; }

    [ForeignKey("EmailId")]
    [InverseProperty("Viewers")]
    public virtual Email Email { get; set; } = null!;

    [ForeignKey("UniqueKeyId")]
    [InverseProperty("Viewers")]
    public virtual UniqueKey? UniqueKey { get; set; }

    [InverseProperty("Viewer")]
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [ForeignKey("VotingEventId")]
    [InverseProperty("Viewers")]
    public virtual VotingEvent VotingEvent { get; set; } = null!;
}
