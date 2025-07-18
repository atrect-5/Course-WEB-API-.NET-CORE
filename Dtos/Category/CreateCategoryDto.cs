using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El tipo de la categoría es obligatorio.")]
        public required string Type { get; set; }

        public int? UserId { get; set; } = null;
    }
}
