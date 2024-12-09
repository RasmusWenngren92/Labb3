using System;
using System.Collections.Generic;

namespace Labb3_Anropa_databasen.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int? StudentIdFk { get; set; }

    public int? CourseIdFk { get; set; }

    public int? TeacherIdFk { get; set; }

    public virtual Course? CourseIdFkNavigation { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Student? StudentIdFkNavigation { get; set; }

    public virtual Employee? TeacherIdFkNavigation { get; set; }
}
