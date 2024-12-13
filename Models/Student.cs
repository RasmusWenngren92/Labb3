using System;
using System.Collections.Generic;

namespace Labb3_Anropa_databasen.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? EnrollmentDate { get; set; }

    public string? Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly? GraduationDate { get; set; }
    
    
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
