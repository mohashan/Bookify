using Bookify.Application.Abstractions.Messaging;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookify.Application.Users.RegisterUser;
public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password):ICommand<Guid>;


internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c=>c.Email).EmailAddress();

        RuleFor(c=>c.FirstName).NotEmpty();
        RuleFor(c=>c.LastName).NotEmpty();

        RuleFor(c=>c.Password).NotEmpty().MinimumLength(5);
    }
}