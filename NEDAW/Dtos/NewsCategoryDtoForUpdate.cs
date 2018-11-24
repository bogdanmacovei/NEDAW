using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NEDAW.Dtos
{
    public class NewsCategoryDtoForUpdate
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}