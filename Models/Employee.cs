using System;
using System.Collections.Generic;

namespace Labb3_Anropa_databasen.Models;

public partial class Employee
{
    public int TeacherId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Role { get; set; }

    public DateOnly? HireDate { get; set; }

    public string? Subject { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
