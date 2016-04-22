using System.Collections.Generic;


namespace CMDAnimeUploader
{
    public class Anime
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public int? Episodes { get; set; }
        public string Aired { get; set; }
        public string Duration { get; set; }
        public int? Rating { get; set; }
        public string Description { get; set; }
        public int? SequelID { get; set; }
       
        public int? PrequelID { get; set; }
   
        public string Genre { get; set; }

        private static readonly Dictionary<string, int> TypeDictionary = new Dictionary<string, int>()
        {
            {"TV",1 },
            {"OVA",2 },
            {"ONA",3 },
            {"Special",4 },
            {"Movie",5 },
            {"Unknown",6 }
        };

        private static readonly Dictionary<string, int> AiringDictionary = new Dictionary<string, int>()
        {
            {"Not yet aired",1},
            { "Currently Airing",2 },
            { "Finished Airing",3}
        };

        public static int GetRating(string rating)
        {
            int ratingNumber;

            switch (rating)
            {
                case AniRating.G:
                    ratingNumber = 1;
                    break;
                case AniRating.Pg:
                    ratingNumber = 2;
                    break;
                case AniRating.Pg13:
                    ratingNumber = 3;
                    break;
                case AniRating.R:
                    ratingNumber = 4;
                    break;
                case AniRating.X:
                    ratingNumber = 5;
                    break;
                default:
                    ratingNumber = 6;
                    break;
            }
            return ratingNumber;
        }

        public static int GetType(string type)
        {
            return TypeDictionary[type];
        }

        public static int GetStatus(string status)
        {
            return AiringDictionary[status];
        }
    }

    internal static class AniRating
    {
        public const string G = "G - All Ages";
        public const string Pg = "PG - Children";
        public const string Pg13 = "PG-13 - Teens 13 or older";
        public const string R = "R - 17+ (violence &amp; profanity)";
        public const string X = "R+ - Mild Nudity";
    }

}
