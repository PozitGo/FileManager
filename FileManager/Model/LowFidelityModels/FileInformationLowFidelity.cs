using System.ComponentModel.DataAnnotations;

namespace FileManager.Model.LowFidelityModels
{
    public class FileInformationLowFidelity
    {
        [Required]
        public required string FileName { get; set; }
        [Required]
        public required string UUID { get; set; }
    }
}
