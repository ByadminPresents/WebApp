using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Vote
{
    public int Id { get; set; }

    public int Score { get; set; }

    public int ProjectId { get; set; }

    public int ViewerId { get; set; }

    public int Criteria { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User Viewer { get; set; } = null!;
}
