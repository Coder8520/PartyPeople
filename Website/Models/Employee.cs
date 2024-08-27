using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Website.Models;

/// <summary>
/// The employee model.
/// </summary>
public class Employee
{
    /// <summary>
    /// The unique identifier for this employee model.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The first name for this employee model.
    /// </summary>
    [DisplayName("First Name")]
    public required string FirstName { get; init; }

    /// <summary>
    /// The last name for this employee model.
    /// </summary>
    [DisplayName("Last Name")]
    public required string LastName { get; init; }

    /// <summary>
    /// The date of birth for this employee model.
    /// </summary>
    [DisplayName("Date of Birth")]
    public required DateOnly DateOfBirth { get; init; }

    public bool IsAttendingEvent { get; set; }

    public int? DrinkId { get; set; }

    public Drink? FavouriteDrink { get; set; }

    public IEnumerable<Drink>? FavouriteDrinks { get; set; }
}