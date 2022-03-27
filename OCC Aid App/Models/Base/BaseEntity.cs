using System.ComponentModel.DataAnnotations;

namespace OCC_Aid_App.Models.Base
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
