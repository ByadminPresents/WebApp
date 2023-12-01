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
    public int Id { get; set; }

    [Column("name")]
    [StringLength(60)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; } = null;

    [Column("unique_key")]
    [StringLength(150)]
    public string UniqueKey { get; set; } = null!;

    [Column("project_id")]
    public int ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Participants")]
    public virtual Project Project { get; set; } = null!;
}
