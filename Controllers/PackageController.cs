using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;

namespace AttendanceAPI3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public PackageController(AttendanceContext context)
        {
            _context = context;
        }

        [HttpPost("create/{id}")]
        public async Task<IActionResult> createPackage([FromForm] PackageDto packageDto, [FromRoute] string id)
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

            // Check if PackageName already exists

            using var stream1 = new MemoryStream();
            await packageDto.FacesFolder.CopyToAsync(stream1);

            using var stream2 = new MemoryStream();
            await packageDto.VoicesFolder.CopyToAsync(stream2);

            using var stream3 = new MemoryStream();
            await packageDto.Sheet.CopyToAsync(stream3);


            // Create and save the package
            var package = new Package
            {
                PackageName = packageDto.PackageName,
                PackageDescription = packageDto.PackageDescription,
                FacesFolder = stream1.ToArray(),
                VoicesFolder = stream2.ToArray(),
                StartTime = packageDto.StartTime,
                EndTime = packageDto.EndTime,
                Sheet = stream3.ToArray() // Store the sheet data as a byte array in the database
            };

            var packageExists = await _context.Packages
               .FirstOrDefaultAsync(p => p.PackageName == packageDto.PackageName);

            if (packageExists != null)
            {
                return BadRequest(new { message = "A package with this name already exists." });
            }

            //package.User_Id=user.UserId;
            package.User_Id = id;
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // Retrieve the ID of the newly created package
            //var packageId = package.PackageId;

            //return RedirectToAction("GetPackages");
            return Ok(new { message = "Please create a list of sequences with + button."/*, packageId */});
        }


        [HttpGet]
        public async Task<ActionResult<List<PackegeListDto>>> GetPackages()
        {
            var packageSummaries = await _context.Packages
                .Select(p => new PackegeListDto
                {
                  
                    PackageName = p.PackageName,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    creator = p.User.UserName
                })
                .ToListAsync();

            return Ok(packageSummaries);
        }

        [HttpGet("userPackages/{userId}")]
        public async Task<IActionResult> GetUserPackages(string userId)
        {
            var packages = await _context.Packages
                .Where(p => p.User_Id == userId) 
                .Select(p => new { p.PackageName })
                .ToListAsync();

            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<PackageDataDto>>> DataOfPackage(int id)
        {
            var data = await _context.Packages
                .Where(p => p.PackageId == id)
                .Select(p => new PackageDataDto
                {

                    PackageName = p.PackageName,
                    PackageDescription=p.PackageDescription,
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
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            if (package.Sheet == null)
            {
                return NotFound("Excel sheet not found.");
            }

            return File(package.Sheet, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sheet.xlsx");
        }

        [HttpGet("download/facesfolder/{id}")]
        public async Task<IActionResult> GetFacesFolder(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            if (package.FacesFolder == null)
            {
                return NotFound("Faces folder file not found.");
            }

            return File(package.FacesFolder, "application/x-rar-compressed", "facesfolder.rar");
        }

        [HttpGet("download/voicesfolder/{id}")]
        public async Task<IActionResult> GetVoicesFolder(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            if (package.VoicesFolder == null)
            {
                return NotFound("Voices folder file not found.");
            }

            return File(package.VoicesFolder, "application/x-rar-compressed", "voicesfolder.rar");
        }
    }

}