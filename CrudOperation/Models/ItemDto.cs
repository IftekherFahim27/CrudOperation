using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CrudOperation.Models
{
    public class ItemDto
    {
        [Required,MaxLength(150)]
        public string Name { get; set; } = "";
        [Required]
        public int Unit { get; set; }
        [Required]
        public int Quantity { get; set; }

        
        public IFormFile? ImageFile { get; set; } 
    }
}
