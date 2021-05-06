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
            var upsertCourseDto = (UpsertCourseDto)validationContext.ObjectInstance;

            if (upsertCourseDto.Title == upsertCourseDto.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(UpsertCourseDto) });
            }

            return ValidationResult.Success;
        }
    }
}
