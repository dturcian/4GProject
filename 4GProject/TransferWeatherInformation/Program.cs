using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferWeatherInformation
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Process();
        }
    }
}
