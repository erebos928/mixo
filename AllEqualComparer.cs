using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    class AllEqualComparer : IEqualityComparer<Int32>
    {
        public bool Equals(int x, int y)
        {
            return false;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
}
