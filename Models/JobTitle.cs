using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class JobTitle
{
    public Guid JobTitleId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
