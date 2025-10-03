using System.ComponentModel.DataAnnotations.Schema;

namespace Habit_Battles.Core.Domain.Entities
{
    public class Auditables
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        private DateTime? _dateModified { get; set; }
        public DateTime? DateModified
        {
            get => _dateModified;
            set => _dateModified = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null;
        }
        [Column(TypeName = "timestamp with time zone")]

        private DateTime? _dateCreated;
        public DateTime? DateCreated
        {
            get => _dateCreated;
            set => _dateCreated = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null;
        }
    }
}
