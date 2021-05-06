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
            var handleCourseDto = (HandleCourseDto)validationContext.ObjectInstance;

            if (handleCourseDto.Title == handleCourseDto.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(HandleCourseDto) });
            }

            return ValidationResult.Success;
        }
    }
}
