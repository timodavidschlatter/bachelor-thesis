using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUDSharedCore.Persistence.Model.Entities
{
    [Table("CODETABLE")]
    public class Codetable
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("CODETYP")]
        [MaxLength(100)]
        public string CodeType { get; set; }
        [Column("ABBREVIATION")]
        [MaxLength(100)]
        public string Abbreviation { get; set; }
        [Column("MEANING")]
        [MaxLength(255)]
        public string Meaning { get; set; }
        [Column("INTERNAL")]
        [MaxLength(255)]
        public string Internal { get; set; }
        [Column("SORTORDER")]
        public long? SortOrder { get; set; }
        [Column("STATE")]
        public long? State { get; set; }

        [NotMapped]
        public bool IsDefaultValue
        {
            get { return State == (int)CodetableStateValues.defaultSelected; }
            set { State = (int)CodetableStateValues.defaultSelected; }
        }
    }

    public enum CodetableStateValues
    {
        active = 0,
        defaultSelected = 10,
        inaktiv = 99
    }
}
