using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Email")]
public partial class Email
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("email_value")]
    [StringLength(100)]
    public string EmailValue { get; set; } = null!;

    [InverseProperty("Email")]
    public virtual ICollection<Organizer> Organizers { get; set; } = new List<Organizer>();

    [InverseProperty("Email")]
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    [InverseProperty("Email")]
    public virtual ICollection<Viewer> Viewers { get; set; } = new List<Viewer>();
}
