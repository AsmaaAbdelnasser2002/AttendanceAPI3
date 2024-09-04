using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;

namespace AttendanceAPI3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public PackageController(AttendanceContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadPackage([FromForm] PackageDto packageDto,[FromRoute]int id)
        { 

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
            package.User_Id = id;
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            //return RedirectToAction("GetPackages");
            return Ok(/*package*/);
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
                    creator = p.User.Username
                })
                .ToListAsync();

            return Ok(packageSummaries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<PackegeListDto>>> DataOfPackage(int id)
        {
            var data = await _context.Packages
                .Where(p => p.PackageId == id)
                .Select(p => new PackageDataDto
                {

                    PackageName = p.PackageName,
                    PackageDescription=p.PackageDescription,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    creator = p.User.Username
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