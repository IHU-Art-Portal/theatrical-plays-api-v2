using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int? PersonId { get; set; }
    }
}
