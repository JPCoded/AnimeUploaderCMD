using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDAnimeUploader
{
    public class UpdateAnime
    {
        public int ID { get; set; }
        public int? Status { get; set; }
        public int? Episodes { get; set; }
        public string Aired { get; set; }
        public string Duration { get; set; }
        public int? Rating { get; set; }
        public int? SequelID { get; set; }
        public int? PrequelID { get; set; }
    }
}
