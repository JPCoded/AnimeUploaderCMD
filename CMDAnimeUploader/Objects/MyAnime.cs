using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDAnimeUploader
{
    public class MyAnime
    {
        public int ID { get; set; }
        public string Status { get; set; }
        public int WatchedEpisodes { get; set; }
        public int Score { get; set; }
        public int AnimeID { get; set; }

        public int GetStatus(string status)
        {
            var returnValue = 0;
            switch (status)
            {
                case "Completed":
                case "2":
                    returnValue = 1;
                    break;
                case "Watching":
                case "1":
                    returnValue = 2;
                    break;
                case "Plan to Watch":
                case "6":
                    returnValue = 3;
                    break;
                case "On-Hold":
                case "3":
                    returnValue = 4;
                    break;
            }
            return returnValue;
        }
    }

    public class GetAnime
    {

        public int AnimeID { get; set; }
    }
}
