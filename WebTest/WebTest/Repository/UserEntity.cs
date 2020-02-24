using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebTest.Repository
{
    public class UserEntity
    {
        public int id { get; set;}
        public string name { get; set; }
        public byte[] avator { get; set; }
    }
}
