using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class Table:List<int>
    {
        public Table():base()
        {

        }
        public Table(int[] values):base(values)
        {
            
        }
        public List<Visit> ComputeVisits()
        {
            List<Visit> visits = new List<Visit>();
            for (int i = 0; i < this.Count; i++)
                for (int j = i + 1; j < this.Count; j++)
                    visits.Add(new Visit(this[i], this[j],true));
            return visits;
        }
        public override string ToString()
        {
            String result = "";
            for (int i = 0; i < this.Count; i++)
                result += this[i] + ", ";
            if (result.EndsWith(" "))
                return result.TrimEnd(new char[] { ',', ' ' });
            else
                return result;
        }
    }
}
