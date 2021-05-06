using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
                                 IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            [FromQuery] AuthorsParameters authorsParameters)
        {
            var authors = _courseLibraryRepository.GetAuthors(authorsParameters);
            var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsDto);
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public ActionResult<AuthorDto> GetAuthors(Guid authorId)
        {
            var authors = _courseLibraryRepository.GetAuthor(authorId);

            if (authors == null)
            {
                return NotFound();
            }

            return _mapper.Map<AuthorDto>(authors);
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            var newAuthor = _mapper.Map<Author>(createAuthorDto);
            _courseLibraryRepository.AddAuthor(newAuthor);
            _courseLibraryRepository.Save();

            var authorDto = _mapper.Map<AuthorDto>(newAuthor);

            return CreatedAtRoute("GetAuthor", new { authorId = authorDto.Id }, authorDto);
        }

        [HttpGet("collection/({ids})", Name = "GetAuthorsCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorsCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBind))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authors = _courseLibraryRepository.GetAuthors(ids);

            if (ids.Count() != authors.Count())
            {
                return NotFound();
            }

            var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsDto);
        }

        [HttpPost("collection")]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
            IEnumerable<CreateAuthorDto> authorCollection)
        {
            var authors = _mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach(var author in authors)
            {
                _courseLibraryRepository.AddAuthor(author);
            }

            _courseLibraryRepository.Save();

            var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);
            var idsAsString = string.Join(",", authorsDto.Select(item => item.Id));

            return CreatedAtRoute("GetAuthorsCollection",
                new { ids = idsAsString },
                authorsDto);
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var author = _courseLibraryRepository.GetAuthor(authorId);

            _courseLibraryRepository.DeleteAuthor(author);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,DELETE");

            return Ok();
        }
    }
}
