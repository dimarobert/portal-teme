using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Models.Tasks;
using PortalTeme.Data;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using PortalTeme.Helpers;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubmissionsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly UserManager<User> userManager;
        private readonly IFileService fileService;

        public SubmissionsController(PortalTemeContext context, UserManager<User> userManager, IFileService fileService) {
            _context = context;
            this.userManager = userManager;
            this.fileService = fileService;
        }


        // POST: api/Submissions/
        [HttpPost]
        public async Task<IActionResult> PostSubmitTask(CreateTaskSubmissionRequest request) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentTask = await _context.StudentAssignedTasks
                .FirstOrDefaultAsync(st => st.Id == request.StudentTaskId);

            if (studentTask is null)
                return NotFound();

            if (studentTask.StudentId != userManager.GetUserId(User))
                return Forbid();

            var submission = new TaskSubmission {
                AssignedTaskId = studentTask.Id,
                State = TaskSubmissionState.Submitted,
                DateAdded = DateTime.Now,
                Files = new List<TaskSubmissionFile>(),
                Description = request.Description
            };

            _context.TaskSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            foreach (var uploadedFile in request.UploadedFiles) {

                var submissionFolder = $"StudentSubmissions/{studentTask.StudentId}/{studentTask.Id}/{submission.Id}";

                // TODO: validate file extension.
                var (fileName, extension) = FilesHelpers.GetFileNameAndExtension(uploadedFile.OriginalName);

                var file = await fileService.MoveTempFile(uploadedFile.TempFileName, submissionFolder, uploadedFile.OriginalName);

                submission.Files.Add(new TaskSubmissionFile {
                    FileId = file.Id,
                    FileType = TaskSubmissionFileType.SourceCode,
                    TaskSubmission = submission
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Submissions/5/Grade
        [HttpPost("{submissionId}/Grade")]
        public async Task<IActionResult> PostGradeTask(Guid submissionId, ReviewTaskSubmissionRequest request) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var submission = await _context.TaskSubmissions
               .FirstOrDefaultAsync(ts => ts.Id == submissionId);

            if (submission is null)
                return NotFound();

            //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
            //    if (!authorization.Succeeded)
            //        return Forbid();

            submission.Review = request.Review;
            submission.State = TaskSubmissionState.Reviewed;

            if (request.Grade.HasValue) {
                submission.Grading = request.Grade;
                submission.State = TaskSubmissionState.Graded;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}