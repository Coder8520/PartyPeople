using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Website.Models;

/// <summary>
/// The Attendance model.
/// </summary>
public class Attendance
{
    /// <summary>
    /// The unique identifier of this attendance model.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The attending employee.
    /// </summary>
    /// <remarks>
    /// Currently accounts for one employee per attendance only, and by ID rather than name.
    /// </remarks>
    [DisplayName("Employee ID")]
    public int? EmployeeID { get; init; }

    /// <summary>
    /// The event to be attended.
    /// </summary>
    [DisplayName("Event ID")]
    public int? EventID { get; init; }
}
