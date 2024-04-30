using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Todo.Dto;

namespace Todo.Models;

public partial class TodoListContext : DbContext
{
    

    public virtual DbSet<TodoListSelectDto> TodoListSelectDto { get; set; }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoListSelectDto>().HasNoKey();
    }
}
