using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class ConflictException: Exception
    {
        public ConflictException(String message):base(message)
        {

        }
    }
}
