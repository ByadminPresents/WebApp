using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class User
{
    public int? Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Login {  get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? UniqueKey { get; set; }

    public int? ProjectId { get; set; }

    public int? VotingEventId { get; set; }

    public int Role { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual VotingEvent? VotingEvent { get; set; }

    public virtual ICollection<VotingEvent> VotingEvents { get; set; } = new List<VotingEvent>();
}
