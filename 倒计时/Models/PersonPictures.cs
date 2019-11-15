using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace 倒计时.Models
{
    class PersonPictures
    {
        [PrimaryKey]
        public string pictureName { get; set; }
        public byte[] picture { get; set; }
    }
}
