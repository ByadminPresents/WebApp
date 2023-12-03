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

    [ForeignKey("EmailId")]
    [InverseProperty("Organizers")]
    public virtual Email Email { get; set; } = null!;

    [InverseProperty("Organizer")]
    public virtual ICollection<VotingEvent> VotingEvents { get; set; } = new List<VotingEvent>();
}
