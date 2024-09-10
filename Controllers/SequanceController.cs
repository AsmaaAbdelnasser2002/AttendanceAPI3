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
    public class SequanceController : ControllerBase
    {

        private readonly AttendanceContext _context;

        public SequanceController(AttendanceContext context)
        {
            _context = context;
        }
        [HttpPost("create/{id}")]
        public async Task<IActionResult> createSequanceWithoutPackage([FromForm] SequanceDto sequanceDto, [FromRoute] string id)
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
                await sequanceDto.FacesFolder.CopyToAsync(stream1);

                using var stream2 = new MemoryStream();
                await sequanceDto.VoicesFolder.CopyToAsync(stream2);

                using var stream3 = new MemoryStream();
                await sequanceDto.Sheet.CopyToAsync(stream3);


                // Create and save the sequance
                var sequance = new Sequance
                {
                    SequanceName = sequanceDto.SequanceName,
                    SequanceDescription = sequanceDto.SequanceDescription,
                    FacesFolder = stream1.ToArray(),
                    VoicesFolder = stream2.ToArray(),
                    StartTime = sequanceDto.StartTime,
                    EndTime = sequanceDto.EndTime,
                    Sheet = stream3.ToArray() // Store the sheet data as a byte array in the database
                };



            var sequanceExists = await _context.Sequances
          .FirstOrDefaultAsync(p => p.SequanceName == sequanceDto.SequanceName);

            if (sequanceExists != null)
            {
                return BadRequest(new { message = "A Sequance with this name already exists." });
            }

            //sequance.User_Id = user.UserId;

            sequance.User_Id = id;
                sequance.Package_Id = null;
                _context.Sequances.Add(sequance);
                await _context.SaveChangesAsync();

               // var sequanceId = sequance.SequanceId;
                return Ok(new { message = "Please create a list of sessions with + button."/*,sequanceId*/ });
            
            
        }


        [HttpPost("createWithPackage/{id}")]
        public async Task<IActionResult> CreateSequanceWithPackage([FromForm] SequanceWithPackageDto sequanceWithPackageDto, [FromRoute] string id)
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
            var sequance = new Sequance
            {
                SequanceName = sequanceWithPackageDto.SequanceName,
                SequanceDescription = sequanceWithPackageDto.SequanceDescription,
                StartTime = sequanceWithPackageDto.StartTime,
                EndTime = sequanceWithPackageDto.EndTime,
                Package_Id = (await _context.Packages.FirstOrDefaultAsync(p => p.PackageName == sequanceWithPackageDto.NameOfPackage)).PackageId

            };
            var p1 = await _context.Packages.FirstOrDefaultAsync(p => p.PackageName == sequanceWithPackageDto.NameOfPackage);
            if (p1 == null)
            {
                return BadRequest();
            }

            var sequanceExists = await _context.Sequances
          .FirstOrDefaultAsync(p => p.SequanceName == sequanceWithPackageDto.SequanceName);

            if (sequanceExists != null)
            {
                return BadRequest(new { message = "A Sequance with this name already exists." });
            }

            sequance.Sheet = p1.Sheet;
                sequance.FacesFolder = p1.FacesFolder;
                sequance.VoicesFolder = p1.VoicesFolder;
                //sequance.User_Id = user.UserId;
                sequance.User_Id = id;
              
                _context.Sequances.Add(sequance);
                await _context.SaveChangesAsync();

                //var sequanceId = sequance.SequanceId;
                return Ok(new { message = "Please create a list of sessions with + button."/*, sequanceId*/ });
        }


        [HttpGet]
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

        [HttpGet("userSequances/{userId}")]
        public async Task<IActionResult> GetUserSequances(string userId)
        {
            var packages = await _context.Sequances
                .Where(p => p.User_Id == userId)
                .Select(p => new { p.SequanceName })
                .ToListAsync();

            return Ok(packages);
        }


        [HttpGet("{id}")]
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
