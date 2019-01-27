using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface IFileProvider {

        Task<IFileInfo> WriteTempFile(Stream stream, CancellationToken cancellationToken = default(CancellationToken));
        Task<IFileInfo> MoveTempFile(string tempFileName, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken));
        IDirectoryContents GetTempFolderContents();
        void DeleteTempFile(string tempFileName, CancellationToken cancellationToken = default(CancellationToken));

        Task<IFileInfo> WriteFile(Stream stream, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken));

        Task<IFileInfo> GetFile(string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken));

        Task<IFileInfo> MoveFile(IFileInfo source, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteFile(IFileInfo file, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ContentRootFileProvider : IFileProvider {
        private readonly string contentRootFilePath;
        private readonly string assetsFolderPath;
        private readonly string tempFolderPath;

        public ContentRootFileProvider(IHostingEnvironment hostingEnvironment) {
            contentRootFilePath = hostingEnvironment.ContentRootPath;
            assetsFolderPath = Path.Combine(contentRootFilePath, "Assets");
            if (!Directory.Exists(assetsFolderPath))
                Directory.CreateDirectory(assetsFolderPath);

            tempFolderPath = Path.Combine(assetsFolderPath, "Temp");
            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);
        }

        public async Task<IFileInfo> WriteTempFile(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) {

            var randomFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(tempFolderPath, $"{randomFileName}.tmp");
            return await WriteFile(stream, tempFilePath, cancellationToken);
        }

        public Task<IFileInfo> MoveTempFile(string tempFileName, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken)) {
            ThrowIfContainsSlashes(tempFileName, nameof(tempFileName));

            var tempFilePath = Path.Combine(tempFolderPath, tempFileName);
            var tempFileInfo = new PhysicalFileInfo(new FileInfo(tempFilePath));
            return MoveFile(tempFileInfo, relativeFolderPath, fileName, extension, cancellationToken);
        }

        public async Task<IFileInfo> WriteFile(Stream stream, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken)) {
            ThrowIfContainsSlashes(fileName, nameof(fileName));
            ThrowIfContainsSlashes(extension, nameof(extension));

            var directoryPath = GetRelativeDirectory(ref relativeFolderPath);

            if (!directoryPath.Exists)
                directoryPath.Create();

            extension = extension.TrimStart('.');
            var filePath = Path.Combine(directoryPath.FullName, $"{fileName}.{extension}");

            return await WriteFile(stream, filePath, cancellationToken);
        }

        public Task<IFileInfo> GetFile(string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken)) {
            ThrowIfContainsSlashes(fileName, nameof(fileName));
            ThrowIfContainsSlashes(extension, nameof(extension));

            var directoryPath = GetRelativeDirectory(ref relativeFolderPath);

            extension = extension.TrimStart('.');
            var filePath = Path.Combine(directoryPath.FullName, $"{fileName}.{extension}");
            return Task.FromResult<IFileInfo>(new PhysicalFileInfo(new FileInfo(filePath)));
        }

        public IDirectoryContents GetTempFolderContents() {
            return new PhysicalDirectoryContents(tempFolderPath);
        }

        private async Task<IFileInfo> WriteFile(Stream stream, string filePath, CancellationToken cancellationToken) {
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write)) {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            return new PhysicalFileInfo(new FileInfo(filePath));
        }

        public async Task<IFileInfo> MoveFile(IFileInfo source, string relativeFolderPath, string fileName, string extension, CancellationToken cancellationToken = default(CancellationToken)) {

            if (source.IsDirectory)
                throw new InvalidOperationException("You cannot move a folder.");

            if (!source.Exists)
                throw new InvalidOperationException("The source file does not exist.");

            IFileInfo destination;
            using (var sourceStream = source.CreateReadStream()) {
                destination = await WriteFile(sourceStream, relativeFolderPath, fileName, extension, cancellationToken);
            }

            var sourceInfo = new FileInfo(source.PhysicalPath);
            sourceInfo.Delete();

            return destination;
        }

        public Task DeleteFile(IFileInfo file, CancellationToken cancellationToken = default(CancellationToken)) {
            if (file.IsDirectory)
                throw new InvalidOperationException("You cannot delete a folder.");

            if (!file.Exists)
                return Task.CompletedTask;

            var sourceInfo = new FileInfo(file.PhysicalPath);
            sourceInfo.Delete();

            return Task.CompletedTask;
        }

        public void DeleteTempFile(string tempFileName, CancellationToken cancellationToken = default(CancellationToken)) {
            ThrowIfContainsSlashes(tempFileName, nameof(tempFileName));

            var tempFilePath = Path.Combine(tempFolderPath, tempFileName);
            var fileInfo = new FileInfo(tempFilePath);

            if (fileInfo.Exists)
                fileInfo.Delete();
        }

        private static void ThrowIfContainsSlashes(string value, string paramName) {
            if (value.Any(c => char.Equals(c, '/') || char.Equals(c, '\\')))
                throw new ArgumentException($"{paramName} cannot contain / or \\ characters.", paramName);
        }

        private DirectoryInfo GetRelativeDirectory(ref string relativeFolderPath) {
            relativeFolderPath = relativeFolderPath.TrimStart('/', '\\');
            var directoryPath = new DirectoryInfo(Path.Combine(assetsFolderPath, relativeFolderPath));
            if (!directoryPath.FullName.StartsWith(assetsFolderPath))
                throw new InvalidOperationException("Cannot read outside of the Assets folder.");
            return directoryPath;
        }

    }
}
