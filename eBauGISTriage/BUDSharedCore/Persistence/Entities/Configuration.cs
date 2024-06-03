using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUDSharedCore.Persistence.Model.Entities
{
    [Table("CONFIGURATION")]
    public class Configuration
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("KEY")]
        [MaxLength(512)]
        public string Key { get; set; }
        [Column("VALUESTR")]
        [MaxLength(2000)]
        public string ValueStr { get; set; }
        [Column("VALUEDATA", TypeName = "BLOB")]
        public byte[] ValueData { get; set; }
    }
}
