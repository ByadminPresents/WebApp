using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("UniqueKey")]
[Index("UniqueKeyValue", Name = "IX_UniqueKey", IsUnique = true)]
public partial class UniqueKey
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uniqueKey_value")]
    [StringLength(36)]
    public string UniqueKeyValue { get; set; } = null!;

    [InverseProperty("UniqueKey")]
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    [InverseProperty("UniqueKey")]
    public virtual ICollection<Viewer> Viewers { get; set; } = new List<Viewer>();
}
