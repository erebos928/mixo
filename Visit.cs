using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class Visit
    {
        public int Visitor1 { get; set; }
        public int Visitor2 { get; set; }
        public Visit(int v1,int v2,bool checking)
        {
            if (v1 == v2 && checking)
                throw new Exception("Identical persons");
            if (v1 < v2)
            {
                Visitor1 = v1;
                Visitor2 = v2;
            }
            else
            {
                Visitor1 = v2;
                Visitor2 = v1;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is Visit)
            {
                Visit otherVisit = (Visit)obj;
                return (Visitor1 == otherVisit.Visitor1 && Visitor2 == otherVisit.Visitor2);
            }
            return false;
        }
        public override string ToString()
        {
            return Visitor1 + "---" + Visitor2;
        }
        public override int GetHashCode()
        {
            return Visitor1 + Visitor2;
        }
    }
}
