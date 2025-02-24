using System;
using System.Linq;
using System.Text.RegularExpressions;

using Bank.Application.Domain;
using Bank.Application.Extensions;
using Bank.Application.Requests;

using FluentValidation;

namespace Bank.Application.Validators;

//TODO: Role and Gender move to this service, and check values to match integers
public static class UserValidator
{
    public class Login : AbstractValidator<UserLoginRequest>
    {
        public Login()
        {
            RuleFor(userRequest => userRequest.Email)
            .NotEmpty()
            .WithMessage("Email is required.") //todo: make constant
            .EmailAddress()
            .WithMessage(""); //todo: make constant
        }
    }

    public class PasswordReset : AbstractValidator<UserPasswordResetRequest>
    {
        public PasswordReset()
        {
            RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password is required.") //todo: make constant
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.") //todo: make constant 
            .MaximumLength(32)
            .WithMessage("Password must be at most 32 characters long.")
            .Must(ContainAtLeastTwoDigits)
            .WithMessage("Password must contain at least two digits.") //todo: make constant
            .Must(ContainAtLeastOneLowercaseCharacter)
            .WithMessage("Password must contain at least one lowercase character.") //todo: make constant 
            .Must(ContainAtLeastOneUppercaseCharacter)
            .WithMessage("Password must contain at least one uppercase character."); //todo: make constant
        }

        private static bool ContainAtLeastTwoDigits(string password)
        {
            return password?.Count(char.IsDigit) >= 2;
        }

        private static bool ContainAtLeastOneLowercaseCharacter(string password)
        {
            return password?.Any(char.IsLower) == true;
        }

        private static bool ContainAtLeastOneUppercaseCharacter(string password)
        {
            return password?.Any(char.IsUpper) == true;
        }
    }

    public class Register : AbstractValidator<UserRegisterRequest>
    {
        private DateOnly m_UINDate;
        private Gender   m_Gender;

        public Register()
        {
            RuleFor(userRequest => userRequest.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.") //todo: make constant
            .MaximumLength(32)
            .WithMessage("Password must be at most 32 characters long.")
            .Must(ValidateName)
            .WithMessage("First name is not valid."); //todo: make constant

            RuleFor(userRequest => userRequest.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.") //todo: make constant
            .Must(ValidateName)
            .WithMessage("Last name is not valid.");

            RuleFor(userRequest => userRequest.DateOfBirth)
            .NotEqual(DateOnly.MinValue)
            .WithMessage("Date of birth is required.")
            .Must(ValidateDateOfBirth)
            .WithMessage("Date of birth is not valid.");

            RuleFor(userRequest => userRequest.Gender)
            .NotEqual(Gender.Invalid)
            .WithMessage("Gender is required.");

            RuleFor(userRequest => userRequest.UniqueIdentificationNumber)
            .Must(MatchUniqueIdentificationNumberRule)
            .WithMessage("Unique identification number must be 13 digits.")
            .Must(ValidateUniqueIdentificationNumberDate)
            .WithMessage("Unique identification number does not contain a valid date.")
            .Must(ValidateGender)
            .WithMessage("Unique identification number does not contain a valid gender.")
            .Must(ValidateControlNumber)
            .WithMessage("Unique identification number does not have a valid control digit.");

            RuleFor(userRequest => userRequest.Email)
            .EmailAddress()
            .WithMessage("Email is not a valid email address."); //todo: make constant

            RuleFor(userRequest => userRequest.Username)
            .Must(ValidateUsername)
            .WithMessage("Invalid username.");

            RuleFor(userRequest => userRequest.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Must(ValidatePhoneNumber)
            .WithMessage("Phone number is not valid.")
            .MinimumLength(12)
            .WithMessage("Phone number does not have enough digits.")
            .MaximumLength(13)
            .WithMessage("Phone number has more than 13 digits.");

            RuleFor(userRequest => userRequest.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .Must(ValidateNameWithNumbers)
            .WithMessage("Address is not valid.");

            RuleFor(userRequest => userRequest.Role)
            .NotEqual(Role.Invalid)
            .WithMessage("Role is required.");

            RuleFor(userRequest => userRequest.Department) // todo klijent ne treba ovo da ima
            .Must(ValidateNameWithNumbers)
            .WithMessage("Department name is not valid.");
        }
        
        private bool ValidateName(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, "^[A-Za-zčćđšžČĆĐŠŽ]+( [A-Za-zčćđšžČĆĐŠŽ]+)*$");
        }
        
        private bool ValidateUsername(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^(?!.*\.\.)(?!.*\.$)[a-zA-Z0-9._]{3,32}$");
        }

        private bool ValidateNameWithNumbers(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, "^[A-Za-zčćđšžČĆĐŠŽ]+( [0-9A-Za-zčćđšžČĆĐŠŽ]+)*$");
        }

        private bool ValidatePhoneNumber(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^\+\d+$");
        }

        private bool MatchUniqueIdentificationNumberRule(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^\d{13}$");
        }

        private bool ValidateUniqueIdentificationNumberDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var result = value.Substring(0, 7)
                              .TryParseUINToDate(out var date);

            m_UINDate = date;

            return result;
        }

        private bool ValidateDateOfBirth(DateOnly value)
        {
            return value < DateOnly.FromDateTime(DateTime.Today);
        }

        private bool DatesMatch(DateOnly dateOfBirth)
        {
            return m_UINDate == dateOfBirth;
        }

        private bool ValidateGender(string value)
        {
            return !(value[9] - '0' < 5 ^ m_Gender == 0);
        }

        private static bool ValidateControlNumber(string value)
        {
            int sum = 0;

            for (int i = 0; i < 12; i++)
                sum += (value[i] - '0') * (7 - i % 6);

            int controlNumber = 11 - sum % 11;

            if (controlNumber > 9)
                controlNumber = 0;

            return controlNumber == value[12] - '0';
        }
    }
}
