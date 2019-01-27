using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using PortalTeme.API.Helpers;
using PortalTeme.API.Models;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileUploadController : ControllerBase {

        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly IFileService fileService;

        public FileUploadController(IFileService fileService) {
            this.fileService = fileService;
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<List<UploadedTempFile>>> Upload() {

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            var filesAndFormData = await ParseMultipartRequest();

            return filesAndFormData.Files;
        }

        private async Task<FilesAndFormData> ParseMultipartRequest() {
            var filesAndFormData = new FilesAndFormData {
                Files = new List<UploadedTempFile>()
            };
            var formAccumulator = new KeyValueAccumulator();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary.Value, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null) {

                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition)) {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition)) {
                        var tempFileName = await fileService.CreateTempFile(section.Body);
                        filesAndFormData.Files.Add(new UploadedTempFile {
                            OriginalName = contentDisposition.FileName.Value,
                            TempFileName = tempFileName
                        });
                    } else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                        var (key, value) = await ParseFormDataSection(formAccumulator, section, contentDisposition);
                        formAccumulator.Append(key, value);

                        if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");

                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            filesAndFormData.FormData = new FormCollection(formAccumulator.GetResults());
            return filesAndFormData;
        }

        private async Task<(string key, string value)> ParseFormDataSection(KeyValueAccumulator formAccumulator, MultipartSection section, ContentDispositionHeaderValue contentDisposition) {
            // Content-Disposition: form-data; name="key"
            //
            // value

            // Do not limit the key name length here because the 
            // multipart headers length limit is already in effect.
            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
            var encoding = GetEncoding(section);
            using (var streamReader = new StreamReader(section.Body, encoding, true, 1024, leaveOpen: true)) {
                // The value length limit is enforced by MultipartBodyLengthLimit
                var value = await streamReader.ReadToEndAsync();
                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                    value = string.Empty;

                return (key.Value, value);
            }
        }

        private Encoding GetEncoding(MultipartSection section) {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding)) {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter {
        public void OnResourceExecuting(ResourceExecutingContext context) {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }

    public struct FilesAndFormData {
        public List<UploadedTempFile> Files { get; set; }

        public FormCollection FormData { get; set; }
    }

}
