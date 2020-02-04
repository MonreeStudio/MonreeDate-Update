using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTasks
{
    class PersonPictures
    {
        [PrimaryKey]
        public string pictureName { get; set; }
        public byte[] picture { get; set; }
    }
}
