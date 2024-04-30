using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Todo.Models;

public partial class TodoListContext : DbContext
{
    public TodoListContext()
    {
    }

    public TodoListContext(DbContextOptions<TodoListContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<JobTitle> JobTitles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TodoList> TodoLists { get; set; }

    public virtual DbSet<UploadFile> UploadFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\JasonH\\DB\\TodoList.mdf;Integrated Security=True;Connect Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__Division__20EFC6A8F99D2514");

            entity.ToTable("Division");

            entity.Property(e => e.DivisionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F114A9BEF50");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Account).IsRequired();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Password).IsRequired();

            entity.HasOne(d => d.Division).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable_1");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.Employees)
                .HasForeignKey(d => d.JobTitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable");
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.JobTitleId).HasName("PK__JobTitle__35382FE932476E76");

            entity.ToTable("JobTitle");

            entity.Property(e => e.JobTitleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Table__8AFACE1A55FB72E9");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasKey(e => e.TodoId).HasName("PK__Table__95862552FC49C675");

            entity.ToTable("TodoList");

            entity.Property(e => e.TodoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.InsertTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.InsertEmployee).WithMany(p => p.TodoListInsertEmployees)
                .HasForeignKey(d => d.InsertEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable");

            entity.HasOne(d => d.UpdateEmployee).WithMany(p => p.TodoListUpdateEmployees)
                .HasForeignKey(d => d.UpdateEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable_1");
        });

        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.HasKey(e => e.UploadFileId).HasName("PK__Table__6F0F98BF69AEBA07");

            entity.ToTable("UploadFile");

            entity.Property(e => e.UploadFileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Src).IsRequired();

            entity.HasOne(d => d.Todo).WithMany(p => p.UploadFiles)
                .HasForeignKey(d => d.TodoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_ToTable");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
