using PortalTeme.Data;
using PortalTeme.Data.Models;
using PortalTeme.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface IFileService {

        Task<Data.Models.FileInfo> CreateFile(Stream stream, string relativeFolderPath, string fileName);
        Task<Data.Models.FileInfo> GetFile(Guid fileId);
        Task<Data.Models.FileInfo> MoveFile(Guid sourceFileId, string relativeFolderPath, string fileName);
        Task DeleteFile(Guid fileId);

        /// <summary>
        /// Writes the contents to a temporary file and returns it's file name and extension.
        /// </summary>
        /// <param name="stream">The content to write to the file.</param>
        /// <returns>The name of the temporary file created.</returns>
        Task<string> CreateTempFile(Stream stream);

        Task<Data.Models.FileInfo> MoveTempFile(string tempFileName, string relativeFolderPath, string fileName);
        Task<long> GetFileSize(Data.Models.FileInfo file);
    }

    public class DbFileService : IFileService {
        private readonly FilesContext context;
        private readonly IFileProvider fileProvider;
        private readonly ITempFilesRepository tempFilesRepository;

        public DbFileService(FilesContext context, IFileProvider fileProvider, ITempFilesRepository tempFilesRepository) {
            this.context = context;
            this.fileProvider = fileProvider;
            this.tempFilesRepository = tempFilesRepository;
        }

        public async Task<Data.Models.FileInfo> CreateFile(Stream stream, string relativeFolderPath, string fileName) {

            var (fileNameWithoutExtension, extension) = FilesHelpers.GetFileNameAndExtension(fileName);

            var fileInfo = await fileProvider.WriteFile(stream, relativeFolderPath, fileNameWithoutExtension, extension);
            var file = await CreateFileInfoToDb(relativeFolderPath, fileNameWithoutExtension, extension);

            return file;
        }

        public async Task<Data.Models.FileInfo> MoveFile(Guid sourceFileId, string relativeFolderPath, string fileName) {

            var (fileNameWithoutExtension, extension) = FilesHelpers.GetFileNameAndExtension(fileName);

            var file = await context.Files.FindAsync(sourceFileId);
            if (file is null)
                throw new InvalidOperationException("No file with the provided sourceFileId exists.");

            var sourceFileInfo = await fileProvider.GetFile(file.RelativeFolderPath, file.FileName, file.Extension);
            var fileInfo = await fileProvider.MoveFile(sourceFileInfo, relativeFolderPath, fileNameWithoutExtension, extension);

            file.RelativeFolderPath = relativeFolderPath;
            file.FileName = fileNameWithoutExtension;
            file.Extension = extension;
            await context.SaveChangesAsync();

            return file;
        }

        public async Task<Data.Models.FileInfo> GetFile(Guid fileId) {
            return await context.Files.FindAsync(fileId);
        }

        public async Task<long> GetFileSize(Data.Models.FileInfo file) {
            var fileInfo = await fileProvider.GetFile(file.RelativeFolderPath, file.FileName, file.Extension);
            return fileInfo.Length;
        }

        public async Task DeleteFile(Guid fileId) {
            var file = await context.Files.FindAsync(fileId);

            if (file is null)
                return;

            var fileInfo = await fileProvider.GetFile(file.RelativeFolderPath, file.FileName, file.Extension);
            await fileProvider.DeleteFile(fileInfo);
        }


        public async Task<string> CreateTempFile(Stream stream) {
            var fileInfo = await fileProvider.WriteTempFile(stream);
            tempFilesRepository.AddTempFile(fileInfo.Name);
            return fileInfo.Name;
        }

        public async Task<Data.Models.FileInfo> MoveTempFile(string tempFileName, string relativeFolderPath, string fileName) {

            var (fileNameWithoutExt, extension) = FilesHelpers.GetFileNameAndExtension(fileName);
            var fileInfo = await fileProvider.MoveTempFile(tempFileName, relativeFolderPath, fileNameWithoutExt, extension);

            var file = await CreateFileInfoToDb(relativeFolderPath, fileNameWithoutExt, extension);
            return file;
        }

        private async Task<Data.Models.FileInfo> CreateFileInfoToDb(string relativeFolderPath, string fileName, string extension) {
            var file = new Data.Models.FileInfo {
                RelativeFolderPath = relativeFolderPath,
                FileName = fileName,
                Extension = extension
            };
            context.Files.Add(file);
            await context.SaveChangesAsync();
            return file;
        }
      
    }
}
