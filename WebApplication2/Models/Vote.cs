using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("Vote")]
public partial class Vote
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("score")]
    public int Score { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [Column("viewer_id")]
    public int ViewerId { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Votes")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("ViewerId")]
    [InverseProperty("Votes")]
    public virtual Viewer Viewer { get; set; } = null!;
}
