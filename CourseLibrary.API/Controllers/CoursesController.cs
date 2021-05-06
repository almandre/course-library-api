using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public ActionResult<IEnumerable<CourseDto>> GetCourses(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courses = _courseLibraryRepository.GetCourses(authorId);
            var coursesDto = _mapper.Map<IEnumerable<CourseDto>>(courses);

            return Ok(coursesDto);
        }

        [HttpGet("{courseId}", Name = "GetCourse")]
        public ActionResult<CourseDto> GetCourse(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }

            return _mapper.Map<CourseDto>(course);
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourse(Guid authorId, CreateCourseDto createCourseDto)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var newCourse = _mapper.Map<Course>(createCourseDto);
            _courseLibraryRepository.AddCourse(authorId, newCourse);
            _courseLibraryRepository.Save();

            var courseDto = _mapper.Map<CourseDto>(newCourse);

            return CreatedAtRoute(
                "GetCourse",
                new {
                    authorId = courseDto.AuthorId,
                    courseId = courseDto.Id
                },
                courseDto
            );
        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourse(Guid authorId, Guid courseId, UpdateCourseDto updateCourseDto)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                var newCourse = _mapper.Map<Course>(updateCourseDto);
                newCourse.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, newCourse);

                _courseLibraryRepository.Save();

                var courseDto = _mapper.Map<CourseDto>(newCourse);

                return CreatedAtRoute(
                    "GetCourse",
                    new { authorId, courseId = courseDto.Id },
                    courseDto
                );
            }

            _mapper.Map(updateCourseDto, course);

            _courseLibraryRepository.UpdateCourse(course);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult UpdatePartially(
            Guid authorId,
            Guid courseId,
            JsonPatchDocument<UpdateCourseDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }

            var updateCourseDto = _mapper.Map<UpdateCourseDto>(course);

            patchDocument.ApplyTo(updateCourseDto, ModelState);

            if (!TryValidateModel(updateCourseDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateCourseDto, course);

            _courseLibraryRepository.UpdateCourse(course);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCoursesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,PATCH");

            return Ok();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
