﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

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

        [HttpPost("create")]
        public async Task<IActionResult> createPackage([FromForm] PackageDto packageDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            package.User_Id = userId;
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            //var packageID = package.PackageId;
            return Ok(new { message = "Please create a list of sequences with + button."/*, packageID */});
        }


        [HttpGet("All_Packages")]
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

        [HttpGet("userPackages")]
        public async Task<IActionResult> GetUserPackages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var packages = await _context.Packages
                .Where(p => p.User_Id == userId) 
                .Select(p => new { p.PackageName })
                .ToListAsync();

            return Ok(packages);
        }

        [HttpGet("Package_Data/{id}")]
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

        [HttpPut("Edit_Package/{id}")]
        public async Task<IActionResult> EditPackage(int id, [FromForm] EditPackageDto editPackageDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            if (package.User_Id != userId)
            {
                return Unauthorized();
            }

            package.PackageName = editPackageDto.PackageName;
            package.PackageDescription = editPackageDto.PackageDescription;
            package.StartTime = editPackageDto.StartTime;
            package.EndTime = editPackageDto.EndTime;

            if (editPackageDto.FacesFolder != null)
            {
                using var stream1 = new MemoryStream();
                await editPackageDto.FacesFolder.CopyToAsync(stream1);
                package.FacesFolder = stream1.ToArray();
            }

            if (editPackageDto.VoicesFolder != null)
            {
                using var stream2 = new MemoryStream();
                await editPackageDto.VoicesFolder.CopyToAsync(stream2);
                package.VoicesFolder = stream2.ToArray();
            }

            if (editPackageDto.Sheet != null)
            {
                using var stream3 = new MemoryStream();
                await editPackageDto.Sheet.CopyToAsync(stream3);
                package.Sheet = stream3.ToArray();
            }

            _context.Packages.Update(package);
            await _context.SaveChangesAsync();

            var sequanceList = await _context.Sequances
               .Where(s => s.Package_Id == package.PackageId)
               .ToListAsync();

            foreach (var s in sequanceList)
            {
                if (s.Package_Id == package.PackageId)
                {
                    if (package.Sheet != s.Sheet)
                    {
                        s.Sheet = package.Sheet;
                    }
                    if (package.FacesFolder != s.FacesFolder)
                    {
                        s.FacesFolder = package.FacesFolder;
                    }
                    if (package.VoicesFolder != s.VoicesFolder)
                    {
                        s.VoicesFolder = package.VoicesFolder;
                    }
                }
            }
            // Update the sequences
            _context.Sequances.UpdateRange(sequanceList);
            await _context.SaveChangesAsync();

            foreach (var s in sequanceList)
            {
                var sessionList = await _context.Sessions
               .Where(se => se.Sequance_Id == s.SequanceId)
               .ToListAsync();

                foreach (var session in sessionList)
                {
                    if (session.Sequance_Id == s.SequanceId)
                    {
                        if (session.Sheet != s.Sheet)
                        {
                            session.Sheet = s.Sheet;
                        }
                        if (session.FacesFolder != s.FacesFolder)
                        {
                            session.FacesFolder = s.FacesFolder;
                        }
                        if (session.VoicesFolder != s.VoicesFolder)
                        {
                            session.VoicesFolder = s.VoicesFolder;
                        }
                    }
                }

                // Update the sessions
                _context.Sessions.UpdateRange(sessionList);
                await _context.SaveChangesAsync();
            }


            return Ok(new { message = "Package updated successfully." });
        }

        [HttpDelete("Delete_Package/{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            if (package.User_Id != userId)
            {
                return Unauthorized();
            }
            

            var sequanceList = await _context.Sequances
               .Where(s => s.Package_Id == package.PackageId)
               .ToListAsync();
            foreach (var sequance in sequanceList)
            {
                var sessionList = await _context.Sessions
                .Where(se => se.Sequance_Id == sequance.SequanceId)
                .ToListAsync();
                foreach (var session in sessionList)
                {
                    var recordList = await _context.AttendanceRecords
                    .Where(r => r.SessionId == session.SessionId)
                    .ToListAsync();
                    _context.AttendanceRecords.RemoveRange(recordList);
                    await _context.SaveChangesAsync();
                    break;
                }
                _context.Sessions.RemoveRange(sessionList);
                await _context.SaveChangesAsync();
                break;
            }
            _context.Sequances.RemoveRange(sequanceList);
            await _context.SaveChangesAsync();

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();


            return Ok(new { message = "Package deleted successfully." });
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