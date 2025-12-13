using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Dimension
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CriterionName { get; set; }
        public int Weight { get; set; }
        public double MinimumScore { get; set; }

    }
}
