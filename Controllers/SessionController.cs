using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpPost("create")]
        public async Task<IActionResult> createSession([FromForm] SessionDto sessionDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            // Create and save the sequance
            var session = new Session
            {
                SessionName = sessionDto.SessionName,
                SessionDescription = sessionDto.SessionDescription,
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime,
                SessionPlace=sessionDto.SessionPlace,
                TimeLimit = sessionDto.TimeLimit
            };
            if (sessionDto.NameOfSequance != null)
            {
                var p1 = await _context.Sequances.FirstOrDefaultAsync(p => p.SequanceName == sessionDto.NameOfSequance);
                if (p1 == null)
                {
                    return BadRequest();
                }
                else
                {
                    session.Sequance_Id = p1.SequanceId;
                    if (sessionDto.FacesFolder != null)
                    {
                        using var stream1 = new MemoryStream();
                        await sessionDto.FacesFolder.CopyToAsync(stream1);
                        session.FacesFolder = stream1.ToArray();
                    }
                    else
                    {
                        session.FacesFolder = p1.FacesFolder;
                    }
                    if (sessionDto.VoicesFolder != null)
                    {
                        using var stream2 = new MemoryStream();
                        await sessionDto.VoicesFolder.CopyToAsync(stream2);
                        session.VoicesFolder = stream2.ToArray();
                    }
                    else
                    {
                        session.VoicesFolder = p1.VoicesFolder;

                    }
                    if (sessionDto.Sheet != null)
                    {
                        using var stream3 = new MemoryStream();
                        await sessionDto.Sheet.CopyToAsync(stream3);
                        session.Sheet = stream3.ToArray();
                    }
                    else
                    {
                        session.Sheet = p1.Sheet;
                    }

                }

            }
            else
            {
                session.Sequance_Id = null;
                if (sessionDto.FacesFolder != null)
                {
                    using var stream1 = new MemoryStream();
                    await sessionDto.FacesFolder.CopyToAsync(stream1);
                    session.FacesFolder = stream1.ToArray();
                }
                if (sessionDto.VoicesFolder != null)
                {
                    using var stream2 = new MemoryStream();
                    await sessionDto.VoicesFolder.CopyToAsync(stream2);
                    session.VoicesFolder = stream2.ToArray();
                }
                if (sessionDto.Sheet != null)
                {
                    using var stream3 = new MemoryStream();
                    await sessionDto.Sheet.CopyToAsync(stream3);
                    session.Sheet = stream3.ToArray();
                }
                else
                {
                    return BadRequest();
                }
            }

            var sessionExists = await _context.Sessions
           .FirstOrDefaultAsync(p => p.SessionName == sessionDto.SessionName);

            if (sessionExists != null)
            {
                return BadRequest(new { message = "A session with this name already exists." });
            }

            session.User_Id = userId;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("All_Sessions")]
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

        [HttpGet("userSessions")]
        public async Task<IActionResult> GetUserSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _context.Sessions
                .Where(p => p.User_Id == userId)
                .Select(p => new { p.SessionName })
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("Session_Data/{id}")]
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
                    TimeLimit = p.TimeLimit,
                    creator = p.User.UserName,
                    ExcelSheetUrl = Url.Action(nameof(GetExcelSheet), new { id }),
                    FacesFolderUrl = Url.Action(nameof(GetFacesFolder), new { id }),
                    VoicesFolderUrl = Url.Action(nameof(GetVoicesFolder), new { id })
                })
                .FirstOrDefaultAsync();

            return Ok(data);
        }

        [HttpPut("Edit_Session/{id}")]
        public async Task<IActionResult> EditSession(int id, [FromForm] EditSessionDto editSessionDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            if (session.User_Id != userId)
            {
                return Unauthorized();
            }

            session.SessionName = editSessionDto.SessionName;
            session.SessionDescription = editSessionDto.SessionDescription;
            session.StartTime = editSessionDto.StartTime;
            session.EndTime = editSessionDto.EndTime;
            session.TimeLimit = editSessionDto.TimeLimit;

            if (editSessionDto.FacesFolder != null)
            {
                using var stream1 = new MemoryStream();
                await editSessionDto.FacesFolder.CopyToAsync(stream1);
                session.FacesFolder = stream1.ToArray();
            }

            if (editSessionDto.VoicesFolder != null)
            {
                using var stream2 = new MemoryStream();
                await editSessionDto.VoicesFolder.CopyToAsync(stream2);
                session.VoicesFolder = stream2.ToArray();
            }

            if (editSessionDto.Sheet != null)
            {
                using var stream3 = new MemoryStream();
                await editSessionDto.Sheet.CopyToAsync(stream3);
                session.Sheet = stream3.ToArray();
            }
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();      
            return Ok(new { message = "Package updated successfully." });
        }

        [HttpDelete("Delete_Session/{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            if (session.User_Id != userId)
            {
                return Unauthorized();
            }

            var recordList = await _context.AttendanceRecords
                 .Where(r => r.Session_Id == session.SessionId)
                 .ToListAsync();
            _context.AttendanceRecords.RemoveRange(recordList);
            await _context.SaveChangesAsync();
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return Ok(new { message = "session deleted successfully." });
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
