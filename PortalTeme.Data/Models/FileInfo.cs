using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace PortalTeme.Data.Models {
    public class FileInfo {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// The physical relative path of the containing directory
        /// </summary>
        [Required]
        public string RelativeFolderPath { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Extension { get; set; }

        public string RelativeFilePath => Path.Combine(RelativeFolderPath, $"{FileName}.{Extension}");

    }
}
