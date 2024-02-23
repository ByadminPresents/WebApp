using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class VotingEvent
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Datetime { get; set; }

    public string Location { get; set; } = null!;

    public int OrganizerId { get; set; }

    public string Criterias { get; set; } = null!;

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
