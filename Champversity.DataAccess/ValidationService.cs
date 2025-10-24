using System.Text.RegularExpressions;
using Champversity.DataAccess.Models;

namespace Champversity.DataAccess
{
    public class ValidationService
    {
        private readonly ApplicationDbContext _dbContext;

    public ValidationService(ApplicationDbContext dbContext)
        {
     _dbContext = dbContext;
        }

 public async Task<List<ValidationResult>> ValidateStudentAsync(Student student)
        {
    var results = new List<ValidationResult>();
            var rules = _dbContext.ValidationRules.Where(r => r.IsActive).ToList();

          // Personal Information Validation
            results.AddRange(ValidateField(nameof(student.FirstName), student.FirstName, rules));
            results.AddRange(ValidateField(nameof(student.LastName), student.LastName, rules));
     results.AddRange(ValidateField(nameof(student.Email), student.Email, rules));
     results.AddRange(ValidateField(nameof(student.PhoneNumber), student.PhoneNumber, rules));
     results.AddRange(ValidateField(nameof(student.Nationality), student.Nationality, rules));

  // Academic Background Validation
 results.AddRange(ValidateField(nameof(student.HighestQualification), student.HighestQualification, rules));
  results.AddRange(ValidateField(nameof(student.IELTSScore), student.IELTSScore, rules));

            // Program Details Validation
       results.AddRange(ValidateField(nameof(student.University), student.University, rules));
            results.AddRange(ValidateField(nameof(student.IntendedProgramMajor), student.IntendedProgramMajor, rules));

    // Custom Business Logic Validation
     results.AddRange(await ValidateBusinessRulesAsync(student));

   return results.Where(r => !r.IsValid).ToList();
        }

  private List<ValidationResult> ValidateField(string fieldName, string value, List<ValidationRule> rules)
     {
      var results = new List<ValidationResult>();
  var fieldRules = rules.Where(r => r.FieldName == fieldName);

foreach (var rule in fieldRules)
  {
        var result = new ValidationResult { FieldName = fieldName };
      
      switch (rule.RuleType.ToLower())
 {
     case "required":
      result.IsValid = !string.IsNullOrWhiteSpace(value);
        result.ErrorMessage = result.IsValid ? "" : rule.ErrorMessage;
     break;
       
    case "minlength":
 if (int.TryParse(rule.RuleValue, out int minLength))
              {
        result.IsValid = string.IsNullOrEmpty(value) || value.Length >= minLength;
    result.ErrorMessage = result.IsValid ? "" : rule.ErrorMessage;
            }
         break;
       
    case "maxlength":
   if (int.TryParse(rule.RuleValue, out int maxLength))
  {
     result.IsValid = string.IsNullOrEmpty(value) || value.Length <= maxLength;
  result.ErrorMessage = result.IsValid ? "" : rule.ErrorMessage;
 }
          break;
   
        case "regex":
              if (!string.IsNullOrEmpty(value))
            {
    try
           {
    var regex = new Regex(rule.RuleValue);
            result.IsValid = regex.IsMatch(value);
   result.ErrorMessage = result.IsValid ? "" : rule.ErrorMessage;
          }
  catch
  {
       result.IsValid = false;
           result.ErrorMessage = "Invalid validation pattern";
    }
         }
   else
    {
      result.IsValid = true; // Allow empty for non-required fields
   }
            break;
      
 case "email":
         result.IsValid = string.IsNullOrEmpty(value) || IsValidEmail(value);
            result.ErrorMessage = result.IsValid ? "" : rule.ErrorMessage;
        break;
    
       default:
        result.IsValid = true;
            break;
  }
    
  results.Add(result);
         }
     
      return results;
        }

    private async Task<List<ValidationResult>> ValidateBusinessRulesAsync(Student student)
  {
         var results = new List<ValidationResult>();

    // Age validation (must be at least 16 years old)
   if (student.DateOfBirth != DateTime.MinValue)
          {
                var age = DateTime.Now.Year - student.DateOfBirth.Year;
     if (student.DateOfBirth > DateTime.Now.AddYears(-age)) age--;
       
      if (age < 16)
         {
  results.Add(new ValidationResult
      {
  IsValid = false,
      FieldName = nameof(student.DateOfBirth),
             ErrorMessage = "Applicant must be at least 16 years old"
     });
           }
  }

            // IELTS Score validation
            if (!string.IsNullOrEmpty(student.IELTSScore))
            {
     if (decimal.TryParse(student.IELTSScore, out decimal ieltsScore))
    {
    if (ieltsScore < 0 || ieltsScore > 9)
           {
        results.Add(new ValidationResult
         {
            IsValid = false,
   FieldName = nameof(student.IELTSScore),
       ErrorMessage = "IELTS Score must be between 0 and 9"
});
              }
       }
                else
       {
         results.Add(new ValidationResult
 {
      IsValid = false,
FieldName = nameof(student.IELTSScore),
   ErrorMessage = "IELTS Score must be a valid number"
        });
       }
       }

            return results;
        }

     private bool IsValidEmail(string email)
        {
         try
  {
         var addr = new System.Net.Mail.MailAddress(email);
    return addr.Address == email;
    }
  catch
   {
      return false;
            }
        }

        public async Task InitializeDefaultValidationRulesAsync()
        {
      if (!_dbContext.ValidationRules.Any())
    {
    var defaultRules = new List<ValidationRule>
 {
       new ValidationRule { FieldName = nameof(Student.FirstName), RuleType = "Required", RuleValue = "", ErrorMessage = "First Name is required", IsActive = true },
       new ValidationRule { FieldName = nameof(Student.LastName), RuleType = "Required", RuleValue = "", ErrorMessage = "Last Name is required", IsActive = true },
        new ValidationRule { FieldName = nameof(Student.Email), RuleType = "Required", RuleValue = "", ErrorMessage = "Email is required", IsActive = true },
    new ValidationRule { FieldName = nameof(Student.Email), RuleType = "Email", RuleValue = "", ErrorMessage = "Please enter a valid email address", IsActive = true },
 new ValidationRule { FieldName = nameof(Student.PhoneNumber), RuleType = "Required", RuleValue = "", ErrorMessage = "Phone Number is required", IsActive = true },
        new ValidationRule { FieldName = nameof(Student.Nationality), RuleType = "Required", RuleValue = "", ErrorMessage = "Nationality is required", IsActive = true },
     new ValidationRule { FieldName = nameof(Student.University), RuleType = "Required", RuleValue = "", ErrorMessage = "University is required", IsActive = true },
        new ValidationRule { FieldName = nameof(Student.IntendedProgramMajor), RuleType = "Required", RuleValue = "", ErrorMessage = "Program/Major is required", IsActive = true },
    new ValidationRule { FieldName = nameof(Student.FirstName), RuleType = "MaxLength", RuleValue = "50", ErrorMessage = "First Name cannot exceed 50 characters", IsActive = true },
     new ValidationRule { FieldName = nameof(Student.LastName), RuleType = "MaxLength", RuleValue = "50", ErrorMessage = "Last Name cannot exceed 50 characters", IsActive = true }
    };

   _dbContext.ValidationRules.AddRange(defaultRules);
     await _dbContext.SaveChangesAsync();
}
        }
    }
}