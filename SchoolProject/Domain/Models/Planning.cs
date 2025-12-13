using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Planning
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
    }
}
