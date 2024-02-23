using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int VotingEventId { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual VotingEvent VotingEvent { get; set; } = null!;
}
