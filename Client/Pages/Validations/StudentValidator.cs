using FluentValidation;
using ICSS.Shared;

namespace ICSS.Client.Pages.Validations
{
    public class StudentModelValidator : AbstractValidator<StudentModel>
    {
        public StudentModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");

            RuleFor(x => x.IdNumber)
                .NotEmpty().WithMessage("ID Number is required.")
                .Length(4, 20).WithMessage("ID Number must be between 4 and 20 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.SystemId)
                .NotEmpty().WithMessage("SystemId is required.")
                .Length(5, 50).WithMessage("SystemId must be between 5 and 50 characters.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required.");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<StudentModel>.CreateWithOptions((StudentModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

}
