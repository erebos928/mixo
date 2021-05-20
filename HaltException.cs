using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class HaltException:Exception
    {
        public HaltException(String message):base(message)
        {

        }
    }
}
