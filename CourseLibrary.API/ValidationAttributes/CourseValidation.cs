using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var createCourseDto = (CreateCourseDto)validationContext.ObjectInstance;

            if (createCourseDto.Title == createCourseDto.Description)
            {
                return new ValidationResult("The provided description should be different from the title",
                    new[] { nameof(CreateCourseDto) });
            }

            return ValidationResult.Success;
        }
    }
}
