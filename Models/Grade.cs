using System;
using System.Collections.Generic;

namespace Labb3_Anropa_databasen.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? NumericGrade { get; set; }

    public DateOnly? GradeSetDate { get; set; }

    public int? EnrollmentIdFk { get; set; }

    public int? TeacherIdFk { get; set; }

    public virtual Enrollment? EnrollmentIdFkNavigation { get; set; }

    public virtual Employee? TeacherIdFkNavigation { get; set; }
}
