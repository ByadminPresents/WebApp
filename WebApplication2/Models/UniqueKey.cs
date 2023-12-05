using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("UniqueKey")]
public partial class UniqueKey
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uniqueKey_value")]
    [StringLength(36)]
    public string UniqueKeyValue { get; set; } = null!;

    [InverseProperty("UniqueKey")]
    public virtual ICollection<Organizer> Organizers { get; set; } = new List<Organizer>();

    [InverseProperty("UniqueKey")]
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    [InverseProperty("UniqueKey")]
    public virtual ICollection<Viewer> Viewers { get; set; } = new List<Viewer>();
}
