using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    //a triple of service, first person and second person (spp)
    public struct SppStruct
    {
        public SppStruct(int ss, int pp1, int pp2)
        {
            s = ss;
            p1 = pp1;
            p2 = pp2;
        }
        public int s;
        public int p1;
        public int p2;
    }
}
