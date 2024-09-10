using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace AttendanceAPI3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly AttendanceContext _context;
        public SessionController(AttendanceContext context)
        {
            _context = context;
        }
        [HttpPost("create/{id}")]
        public async Task<IActionResult> createSessionWithoutSequance([FromForm] SessionDto sessionDto, [FromRoute] string id)
        {
            //Response.Headers.Add("Cache-Control", "no-cache,no-store,must-revalidate");
            //Response.Headers.Add("Pragma", "no-cache");
            //var name = HttpContext.Session.GetString("Email");
            //var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == HttpContext.Session.GetString("Email"));
            //if (string.IsNullOrEmpty(name))
            //{
            //    return Unauthorized(new { message = "You are not authenticated. Please log in." });
            //}
            //if (user.UserRole != "Instructor")
            //{
            //    return Unauthorized(new { message = "You are not authenticated. Please log in with Instructor role." });
            //}
            using var stream1 = new MemoryStream();
            await sessionDto.FacesFolder.CopyToAsync(stream1);

            using var stream2 = new MemoryStream();
            await sessionDto.VoicesFolder.CopyToAsync(stream2);

            using var stream3 = new MemoryStream();
            await sessionDto.Sheet.CopyToAsync(stream3);


            // Create and save the session
            var session = new Session
            {
                SessionName = sessionDto.SessionName,
                SessionPlace= sessionDto.SessionPlace,
                SessionDescription = sessionDto.SessionDescription,
                FacesFolder = stream1.ToArray(),
                VoicesFolder = stream2.ToArray(),
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime,
                Sheet = stream3.ToArray() // Store the sheet data as a byte array in the database
            };

            var sessionExists = await _context.Sessions
         .FirstOrDefaultAsync(p => p.SessionName == sessionDto.SessionName);

            if (sessionExists != null)
            {
                return BadRequest(new { message = "A Session with this name already exists." });
            }

            //session.User_Id = user.UserId;
            session.User_Id = id;
            session.Sequance_Id = null;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("createWithSequance/{id}")]
        public async Task<IActionResult> CreateSessionWithSequance([FromForm] SessionWithSequanceDto sessionWithSequanceDto, [FromRoute] string id)
        {
            //Response.Headers.Add("Cache-Control", "no-cache,no-store,must-revalidate");
            //Response.Headers.Add("Pragma", "no-cache");
            //var name = HttpContext.Session.GetString("Email");
            //var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == HttpContext.Session.GetString("Email"));
            //if (string.IsNullOrEmpty(name))
            //{
            //    return Unauthorized(new { message = "You are not authenticated. Please log in." });
            //}
            //if (user.UserRole != "Instructor")
            //{
            //    return Unauthorized(new { message = "You are not authenticated. Please log in with Instructor role." });
            //}
            var session = new Session
            {
                SessionName = sessionWithSequanceDto.SessionName,
                SessionDescription = sessionWithSequanceDto.SessionDescription,
                SessionPlace = sessionWithSequanceDto.SessionPlace,
                StartTime = sessionWithSequanceDto.StartTime,
                EndTime = sessionWithSequanceDto.EndTime,
                Sequance_Id = (await _context.Sequances.FirstOrDefaultAsync(p => p.SequanceName == sessionWithSequanceDto.NameOfSequance)).SequanceId
            };
            var p1 = await _context.Sequances.FirstOrDefaultAsync(p => p.SequanceName == sessionWithSequanceDto.NameOfSequance);
            if (p1 == null)
            {
                return BadRequest();
            }

            var sessionExists = await _context.Sessions
         .FirstOrDefaultAsync(p => p.SessionName == sessionWithSequanceDto.SessionName);

            if (sessionExists != null)
            {
                return BadRequest(new { message = "A Session with this name already exists." });
            }

            session.Sheet = p1.Sheet;
            session.FacesFolder = p1.FacesFolder;
            session.VoicesFolder = p1.VoicesFolder;
            //session.User_Id = user.UserId;
            session.User_Id = id;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult<List<SessionListDto>>> GetSessions()
        {
            var sessionSummaries = await _context.Sessions
                .Select(p => new SessionListDto
                {

                    SessionName = p.SessionName,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    creator = p.User.UserName
                })
                .ToListAsync();

            return Ok(sessionSummaries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<SessionDataDto>>> DataOfSession(int id)
        {
            var data = await _context.Sessions
                .Where(p => p.SessionId == id)
                .Select(p => new SessionDataDto
                {

                    SessionName = p.SessionName,
                    SessionPlace = p.SessionPlace,
                    SessionDescription = p.SessionDescription,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    creator = p.User.UserName,
                    ExcelSheetUrl = Url.Action(nameof(GetExcelSheet), new { id }),
                    FacesFolderUrl = Url.Action(nameof(GetFacesFolder), new { id }),
                    VoicesFolderUrl = Url.Action(nameof(GetVoicesFolder), new { id })
                })
                .FirstOrDefaultAsync();

            return Ok(data);
        }

        [HttpGet("download/sheet/{id}")]
        public async Task<IActionResult> GetExcelSheet(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            if (session.Sheet == null)
            {
                return NotFound("Excel sheet not found.");
            }

            return File(session.Sheet, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sheet.xlsx");
        }

        [HttpGet("download/facesfolder/{id}")]
        public async Task<IActionResult> GetFacesFolder(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            if (session.FacesFolder == null)
            {
                return NotFound("Faces folder file not found.");
            }

            return File(session.FacesFolder, "application/x-rar-compressed", "facesfolder.rar");
        }

        [HttpGet("download/voicesfolder/{id}")]
        public async Task<IActionResult> GetVoicesFolder(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            if (session.VoicesFolder == null)
            {
                return NotFound("Voices folder file not found.");
            }

            return File(session.VoicesFolder, "application/x-rar-compressed", "voicesfolder.rar");
        }



    }
}
