using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CourseDto>();

            CreateMap<CreateCourseDto, Course>();

            CreateMap<UpdateCourseDto, Course>();

            CreateMap<Course, UpdateCourseDto>();
        }
    }
}
