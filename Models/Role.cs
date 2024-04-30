using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public Guid EmployeeId { get; set; }

    public string Name { get; set; }
}
