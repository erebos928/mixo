using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class VisitEqualityComparer : IEqualityComparer<Visit>
    {
        public bool Equals(Visit x, Visit y)
        {
            return (x.Visitor1 == y.Visitor1) && (x.Visitor2 == y.Visitor2);
        }

        public int GetHashCode(Visit obj)
        {
            return obj.GetHashCode();
        }
    }
}
