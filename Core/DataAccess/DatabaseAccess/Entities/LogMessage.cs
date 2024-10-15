using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WtSbAssistant.Core.Logger;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities
{
    public class LogMessage
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public LogSeverity Severity { get; set; }

        public string Source { get; set; } = null!;

        [StringLength(30)]
        public string ShortSource { get; set; } = null!;

        public string Message { get; set; } = null!;

        [StringLength(30)] public string ShortMessage { get; set; } = null!;

        public DateTime Time { get; set; }
    }
}
