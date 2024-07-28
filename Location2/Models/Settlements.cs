using System.ComponentModel.DataAnnotations;

namespace Location2.Models
{
    public class Settlement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        //public bool IsEditing { get; set; }

    }
}


