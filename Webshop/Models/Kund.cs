//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Webshop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Kund
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kund()
        {
            this.Bestallnings = new HashSet<Bestallning>();
        }
    
        public int KundID { get; set; }

        [Required(ErrorMessage = "Title is required (max 50 signs)")]
        [StringLength(50, ErrorMessage = "50 sign MAX")]
        [MinLength(2)]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Title is required (max 50 signs)")]
        [StringLength(50, ErrorMessage = "50 sign MAX")]
        [MinLength(5)]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Title is required (max 15 signs)")]
        [StringLength(50, ErrorMessage = "15 sign MAX")]
        [MinLength(4)]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Title is required (max 50 signs)")]
        [StringLength(50, ErrorMessage = "50 sign MAX")]
        [MinLength(3)]
        public string Postort { get; set; }

        public string Email { get; set; }
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Title is required (max 50 signs)")]
        [StringLength(50, ErrorMessage = "50 sign MAX")]
        [MinLength(3)]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Title is required (max 50 signs)")]
        [StringLength(50, ErrorMessage = "50 sign MAX")]
        [MinLength(4)]
        public string Losenord { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bestallning> Bestallnings { get; set; }
    }
}
