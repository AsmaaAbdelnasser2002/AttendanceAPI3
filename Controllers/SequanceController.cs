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
    public class SequanceController : ControllerBase
    {

        private readonly AttendanceContext _context;

        public SequanceController(AttendanceContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> createSequance([FromForm] SequanceDto sequanceDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Create and save the sequance
            var sequance = new Sequance
            {
                SequanceName = sequanceDto.SequanceName,
                SequanceDescription = sequanceDto.SequanceDescription,
                StartTime = sequanceDto.StartTime,
                EndTime = sequanceDto.EndTime,
            };
            if (sequanceDto.NameOfPackage != null)
            {
                var p1 = await _context.Packages.FirstOrDefaultAsync(p => p.PackageName == sequanceDto.NameOfPackage);
                if (p1 == null)
                {
                    return BadRequest();
                }
                else
                {
                    sequance.Package_Id = p1.PackageId;
                    if (sequanceDto.FacesFolder != null)
                    {
                        using var stream1 = new MemoryStream();
                        await sequanceDto.FacesFolder.CopyToAsync(stream1);
                        sequance.FacesFolder = stream1.ToArray();
                    }
                    else
                    {
                        sequance.FacesFolder = p1.FacesFolder ;
                    }
                    if (sequanceDto.VoicesFolder != null)
                    {
                        using var stream2 = new MemoryStream();
                        await sequanceDto.VoicesFolder.CopyToAsync(stream2);
                        sequance.VoicesFolder = stream2.ToArray();
                    }
                    else
                    {
                        sequance.VoicesFolder = p1.VoicesFolder ;

                    }
                    if (sequanceDto.Sheet != null)
                    {
                        using var stream3 = new MemoryStream();
                        await sequanceDto.Sheet.CopyToAsync(stream3);
                        sequance.Sheet = stream3.ToArray();
                    }
                    else
                    {
                        sequance.Sheet = p1.Sheet ;
                    }
                    
                }
               
            }
            else
            {
                sequance.Package_Id = null;
                if (sequanceDto.FacesFolder != null)
                {
                    using var stream1 = new MemoryStream();
                    await sequanceDto.FacesFolder.CopyToAsync(stream1);
                    sequance.FacesFolder = stream1.ToArray();
                }
                if (sequanceDto.VoicesFolder != null)
                {
                    using var stream2 = new MemoryStream();
                    await sequanceDto.VoicesFolder.CopyToAsync(stream2);
                    sequance.VoicesFolder = stream2.ToArray();
                }
                if (sequanceDto.Sheet != null)
                {
                    using var stream3 = new MemoryStream();
                    await sequanceDto.Sheet.CopyToAsync(stream3);
                    sequance.Sheet = stream3.ToArray();
                }
                else
                {
                    return BadRequest();
                }
            }

            var sequanceExists = await _context.Sequances
           .FirstOrDefaultAsync(p => p.SequanceName == sequanceDto.SequanceName);

            if (sequanceExists != null)
            {
                return BadRequest(new { message = "A Sequance with this name already exists." });
            }

            sequance.User_Id = userId;
            _context.Sequances.Add(sequance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Please create a list of sessions with + button." });
        }

        [HttpGet("All_Sequances")]
        public async Task<ActionResult<List<SequanceListDto>>> GetSequances()
        {
            var sequanceSummaries = await _context.Sequances
                .Select(p => new SequanceListDto
                {

                    SequanceName = p.SequanceName,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    creator = p.User.UserName
                })
                .ToListAsync();

            return Ok(sequanceSummaries);
        }

        [HttpGet("userSequances")]
        public async Task<IActionResult> GetUserSequances()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequances = await _context.Sequances
                .Where(p => p.User_Id == userId)
                .Select(p => new { p.SequanceName })
                .ToListAsync();

            return Ok(sequances);
        }


        [HttpGet("Sequance_Data/{id}")]
        public async Task<ActionResult<List<SequanceDataDto>>> DataOfSequance(int id)
        {
            var data = await _context.Sequances
                .Where(p => p.SequanceId == id)
                .Select(p => new SequanceDataDto
                {

                    SequanceName = p.SequanceName,
                    SequanceDescription = p.SequanceDescription,
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

        [HttpPut("Edit_Sequance/{id}")]
        public async Task<IActionResult> EditSequance(int id, [FromForm] EditSequanceDto editSequanceDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequance = await _context.Sequances.FindAsync(id);

            if (sequance == null)
            {
                return NotFound();
            }

            if (sequance.User_Id != userId)
            {
                return Unauthorized();
            }

            sequance.SequanceName = editSequanceDto.SequanceName;
            sequance.SequanceDescription = editSequanceDto.SequanceDescription;
            sequance.StartTime = editSequanceDto.StartTime;
            sequance.EndTime = editSequanceDto.EndTime;

            if (editSequanceDto.FacesFolder != null)
            {
                using var stream1 = new MemoryStream();
                await editSequanceDto.FacesFolder.CopyToAsync(stream1);
                sequance.FacesFolder = stream1.ToArray();
            }

            if (editSequanceDto.VoicesFolder != null)
            {
                using var stream2 = new MemoryStream();
                await editSequanceDto.VoicesFolder.CopyToAsync(stream2);
                sequance.VoicesFolder = stream2.ToArray();
            }

            if (editSequanceDto.Sheet != null)
            {
                using var stream3 = new MemoryStream();
                await editSequanceDto.Sheet.CopyToAsync(stream3);
                sequance.Sheet = stream3.ToArray();
            }
            _context.Sequances.Update(sequance);
            await _context.SaveChangesAsync();

            var sessionList = await _context.Sessions
              .Where(se => se.Sequance_Id == sequance.SequanceId)
              .ToListAsync();

            foreach (var session in sessionList)
            {
                if (session.Sequance_Id == sequance.SequanceId)
                {
                    if (session.Sheet != sequance.Sheet)
                    {
                        session.Sheet = sequance.Sheet;
                    }
                    if (session.FacesFolder != sequance.FacesFolder)
                    {
                        session.FacesFolder = sequance.FacesFolder;
                    }
                    if (session.VoicesFolder != sequance.VoicesFolder)
                    {
                        session.VoicesFolder = sequance.VoicesFolder;
                    }
                }
            }
            // Update the sessions
            _context.Sessions.UpdateRange(sessionList);
            await _context.SaveChangesAsync();
            return Ok(new { message = "sequance updated successfully." });
        }

        [HttpDelete("Delete_Sequance/{id}")]
        public async Task<IActionResult> DeleteSequance(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequance = await _context.Sequances.FindAsync(id);

            if (sequance == null)
            {
                return NotFound();
            }

            if (sequance.User_Id != userId)
            {
                return Unauthorized();
            }

            var sessionList = await _context.Sessions
                .Where(se => se.Sequance_Id == sequance.SequanceId)
                .ToListAsync();
            foreach (var session in sessionList)
            {
                var recordList = await _context.AttendanceRecords
                .Where(r => r.Session_Id == session.SessionId)
                .ToListAsync();
                _context.AttendanceRecords.RemoveRange(recordList);
                await _context.SaveChangesAsync();
                break;
            }
            _context.Sessions.RemoveRange(sessionList);
            await _context.SaveChangesAsync();

            _context.Sequances.Remove(sequance);
            await _context.SaveChangesAsync();

            

            return Ok(new { message = "sequance deleted successfully." });
        }

        [HttpGet("download/sheet/{id}")]
        public async Task<IActionResult> GetExcelSheet(int id)
        {
            var sequance = await _context.Sequances.FindAsync(id);
            if (sequance == null)
            {
                return NotFound();
            }

            if (sequance.Sheet == null)
            {
                return NotFound("Excel sheet not found.");
            }

            return File(sequance.Sheet, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sheet.xlsx");
        }

        [HttpGet("download/facesfolder/{id}")]
        public async Task<IActionResult> GetFacesFolder(int id)
        {
            var sequance = await _context.Sequances.FindAsync(id);
            if (sequance == null)
            {
                return NotFound();
            }

            if (sequance.FacesFolder == null)
            {
                return NotFound("Faces folder file not found.");
            }

            return File(sequance.FacesFolder, "application/x-rar-compressed", "facesfolder.rar");
        }

        [HttpGet("download/voicesfolder/{id}")]
        public async Task<IActionResult> GetVoicesFolder(int id)
        {
            var sequance = await _context.Sequances.FindAsync(id);
            if (sequance == null)
            {
                return NotFound();
            }

            if (sequance.VoicesFolder == null)
            {
                return NotFound("Voices folder file not found.");
            }

            return File(sequance.VoicesFolder, "application/x-rar-compressed", "voicesfolder.rar");
        }


    }
}
