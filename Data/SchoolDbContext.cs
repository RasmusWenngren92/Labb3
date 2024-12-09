using System;
using System.Collections.Generic;
using Labb3_Anropa_databasen.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb3_Anropa_databasen.Data;

public partial class SchoolDbContext : DbContext
{
    public SchoolDbContext()
    {
    }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187092125E3");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(30);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Employee__EDF259446484B98E");

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.FirstName).HasMaxLength(10);
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Subject).HasMaxLength(20);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F6877FB137C6997");

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.CourseIdFk).HasColumnName("CourseID_FK");
            entity.Property(e => e.StudentIdFk).HasColumnName("StudentID_FK");
            entity.Property(e => e.TeacherIdFk).HasColumnName("TeacherID_FK");

            entity.HasOne(d => d.CourseIdFkNavigation).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseIdFk)
                .HasConstraintName("FK__Enrollmen__Cours__2E1BDC42");

            entity.HasOne(d => d.StudentIdFkNavigation).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentIdFk)
                .HasConstraintName("FK__Enrollmen__Stude__2D27B809");

            entity.HasOne(d => d.TeacherIdFkNavigation).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.TeacherIdFk)
                .HasConstraintName("FK__Enrollmen__Teach__2F10007B");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A373D838EFB");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.EnrollmentIdFk).HasColumnName("EnrollmentID_FK");
            entity.Property(e => e.TeacherIdFk).HasColumnName("TeacherID_FK");

            entity.HasOne(d => d.EnrollmentIdFkNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.EnrollmentIdFk)
                .HasConstraintName("FK__Grades__Enrollme__34C8D9D1");

            entity.HasOne(d => d.TeacherIdFkNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.TeacherIdFk)
                .HasConstraintName("FK__Grades__TeacherI__35BCFE0A");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A7968C72E03");

            entity.ToTable(tb => tb.HasTrigger("trg_SetGradeSetDate"));

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.FirstName).HasMaxLength(10);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LastName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
