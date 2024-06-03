﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUDSharedCore.Persistence.Model.Entities
{
    [Table("AMT")]
    public partial class Amt
    {
        [Key]
        [Column("ID", TypeName = "NUMBER(8)")]
        public int Id { get; set; }
        [Column("AMT_KURZ")]
        [StringLength(4)]
        public string Kuerzel { get; set; }
        [Column("AMT")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("BEREICH")]
        [StringLength(4)]
        public string Bereich { get; set; }

        [InverseProperty("Dienststelle")]
        public virtual ICollection<Mitarbeiter> Mitarbeiter { get; set; }
    }
}