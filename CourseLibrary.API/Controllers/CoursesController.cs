using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors/{authorId}/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository,
                                 IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesOfAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesOfAuthorFromRepo = _courseLibraryRepository.GetCourses(authorId);

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesOfAuthorFromRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourseOfAuthor")]
        public ActionResult<CourseDto> GetCourseOfAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseOfAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseOfAuthorFromRepo == null)
            {
                return NotFound();
            }

            return _mapper.Map<CourseDto>(courseOfAuthorFromRepo);
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseOfAuthor(Guid authorId, CourseForCreationDto courseDto)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var newCourse = _mapper.Map<Course>(courseDto);
            _courseLibraryRepository.AddCourse(authorId, newCourse);
            _courseLibraryRepository.Save();

            var course = _mapper.Map<CourseDto>(newCourse);

            return CreatedAtRoute(
                "GetCourseOfAuthor",
                new {
                    authorId = course.AuthorId,
                    courseId = course.Id
                },
                course
            );
        }
    }
}
