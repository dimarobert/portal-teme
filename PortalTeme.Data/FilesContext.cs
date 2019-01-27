using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Models;

namespace PortalTeme.Data {
    public class FilesContext : DbContext {
        public FilesContext(DbContextOptions<FilesContext> options) : base(options) { }


        public DbSet<FileInfo> Files { get; set; }

    }
}
