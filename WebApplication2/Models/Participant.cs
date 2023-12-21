using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Participant")]
public partial class Participant
{
    [Key]
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    [StringLength(60)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("email_id")]
    public int? EmailId { get; set; }

    [Column("uniqueKey_id")]
    public int? UniqueKeyId { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [ForeignKey("EmailId")]
    [InverseProperty("Participants")]
    public virtual Email? Email { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Participants")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UniqueKeyId")]
    [InverseProperty("Participants")]
    public virtual UniqueKey? UniqueKey { get; set; }
}
