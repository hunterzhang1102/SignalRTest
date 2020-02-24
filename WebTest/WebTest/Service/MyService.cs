using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Service
{
    public class MyService
    {
        private int _count;
        public int GetLastedCount()
        {
            _count++;
            return _count;
        }
    }
}
