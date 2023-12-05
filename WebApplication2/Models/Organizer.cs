using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Organizer")]
public partial class Organizer
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

    [Column("password")]
    [StringLength(50)]
    public string Password { get; set; } = null!;

    [Column("uniqueKey_id")]
    public int? UniqueKeyId { get; set; }

    [ForeignKey("EmailId")]
    [InverseProperty("Organizers")]
    public virtual Email Email { get; set; } = null!;

    [ForeignKey("UniqueKeyId")]
    [InverseProperty("Organizers")]
    public virtual UniqueKey? UniqueKey { get; set; }

    [InverseProperty("Organizer")]
    public virtual ICollection<VotingEvent> VotingEvents { get; set; } = new List<VotingEvent>();
}
