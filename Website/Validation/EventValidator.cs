using FluentValidation;
using Website.Models;

namespace Website.Validation;

/// <summary>
/// The validator for <see cref="Event"/> objects.
/// </summary>
public class EventValidator : AbstractValidator<Event>
{
    /// <summary>
    /// Instantiates a new instance of the validator.
    /// </summary>
    public EventValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255);

        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.Now)
            .WithMessage($"The Event start date must be in the future.");

        RuleFor(x => x.EndDateTime)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage($"The Event end date must be be after the start date.");

        RuleFor(x => x.MaximumCapacity)
            .GreaterThan(0)
            .When(x => x.MaximumCapacity is not null)
            .WithMessage($"The Maximum Capacity must be greater than 0. Leave this empty if there is no limit.");
        
        // TODO rule for AttendingEmployees if over capacity 
        RuleFor(m => m.AttendingEmployees).Must((model, attendingEmployees) => attendingEmployees.Count() <= model.MaximumCapacity)
            .When(m => m.MaximumCapacity is not null && m.AttendingEmployees is not null)
            .WithMessage($"The number of attending employees cannot exceed the maximum capacity of the event.");
            
            
    }
}

