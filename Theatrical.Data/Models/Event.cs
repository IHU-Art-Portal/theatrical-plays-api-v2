using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public int VenueId { get; set; }
        public string DateEvent { get; set; }
        public string PriceRange { get; set; } = null!;
        public int SystemId { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Production Production { get; set; } = null!;
        public virtual System System { get; set; } = null!;
        public virtual Venue Venue { get; set; } = null!;
    }
}
