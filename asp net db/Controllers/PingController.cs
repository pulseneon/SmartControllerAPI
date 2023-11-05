using asp_net_db.Models.Dto;
using asp_net_db.Models.Results;
using asp_net_db.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_db.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class PingController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Ping(string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return BadRequest(new ErrorResult(401, error: "Невалидный токен"));

            return Ok();
        }

        /*
         private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public CourseController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Добавить в курс по гуид (ссылке)
        /// </summary>
        [HttpGet("invite/{guid}")]
        public async Task<IActionResult> InviteUser(int userId, string guid)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.InviteHash == guid);

            if (course == null)
            {
                return NotFound("Курс не найден");
            }

            var isIxist = course.StudentsIds.Contains(userId);

            if (isIxist)
            {
                return BadRequest("Оценка по курсу уже выставлена");
            }

            course.StudentsIds.Add(userId);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Получить invite/guid курса для инвайта
        /// </summary>
        [HttpGet("invite")]
        public async Task<IActionResult> GetInvite(int courseId, string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401);

            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(new HashDto(course.InviteHash));
        }

        /// <summary>
        /// Массив всех курсов преподавателя
        /// </summary>
        [HttpGet("AllTeacher")]
        public async Task<IActionResult> AddTrack(int id, string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401);

            return Ok(await _context.Courses.Where(x => x.OwnerId == id).ToListAsync());
        }

        /// <summary>
        /// Получить курс
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCourse(int id, string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401);

            var course = await _context.Courses.Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        /// <summary>
        /// Все курсы пользователя
        /// </summary>
        [HttpGet("userCourses")]
        public async Task<IActionResult> AllUserCourses(int userId, string token)
        {
            var result = TokenUtility.ValidateToken(token);
            if (!result) return StatusCode(401);

            var course = await _context.Courses.Include(x => x.Lessons).Where(x => x.StudentsIds.Any(s => s == userId)).ToListAsync();

            return Ok(course);
        }

        /// <summary>
        /// Добавить курс
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CourseDto dto, string token)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(errors);
            }

            if (!TokenUtility.ValidateToken(token)) return StatusCode(401);

            var course = _mapper.Map<Course>(dto);
            _context.Courses.Add(course);
            _context.SaveChanges();

            return Ok(course);
        }

        /// <summary>
        /// Удалить (мягко) курс
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(int id, string token)
        {
            if (!TokenUtility.ValidateToken(token)) return StatusCode(401);

            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
            foreach(var lesson in course.Lessons)
            {
                lesson.isDeleted = true;
            }

            course.isDeleted = true;

            _context.SaveChanges();
            return Ok();
        }
         */
    }
}
