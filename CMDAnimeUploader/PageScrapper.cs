using System;
using HtmlAgilityPack;

namespace CMDAnimeUploader
{
    internal static class PageScraper
    {
        public static Anime GetAnimeInfo(int animeId)
        {
            var anime = new Anime();

            var document = GetPage(animeId);

           
            var type =
                document.DocumentNode.SelectSingleNode("//*[text()='Type:']/parent::div")
                    .InnerText.Replace("Type:", "")
                    .Trim();
            var episode =
                document.DocumentNode.SelectSingleNode("//*[text()='Episodes:']/parent::div")
                    .InnerText.Replace("Episodes:", "")
                    .Trim();
            var status =
                document.DocumentNode.SelectSingleNode("//*[text()='Status:']/parent::div")
                    .InnerText.Replace("Status:", "")
                    .Trim();
            var aired =
                document.DocumentNode.SelectSingleNode("//*[text()='Aired:']/parent::div")
                    .InnerText.Replace("Aired:", "")
                    .Trim();
            var duration =
                document.DocumentNode.SelectSingleNode("//*[text()='Duration:']/parent::div")
                    .InnerText.Replace("Duration:", "")
                    .Trim();
            var rating =
                document.DocumentNode.SelectSingleNode("//*[text()='Rating:']/parent::div")
                    .InnerText.Replace("Rating:", "")
                    .Trim();
            var synopsis =
                document.DocumentNode.SelectSingleNode("//meta[@property='og:description']")
                    .GetAttributeValue("content", "")
                    .Replace("'", "''")
                    .Trim();
            var genres =
                document.DocumentNode.SelectSingleNode("//*[text()='Genres:']/parent::div")
                    .InnerText.Replace("Genres:", "")
                    .Replace(", ", ",")
                    .Trim();
            var prequelId = document.DocumentNode.SelectSingleNode("//*[text()='Prequel:']/parent::tr/descendant::a");
            var sequelId = document.DocumentNode.SelectSingleNode("//*[text()='Sequel:']/parent::tr/descendant::a");
            var title =
                document.DocumentNode.SelectSingleNode("//meta[@property='og:title']")
                    .GetAttributeValue("content", "")
                    .Replace("'", "''")
                    .Trim();

            string[] newPrequelId = null;
  
            string[] newSequelId = null;
         


            if (prequelId != null)
            {
                newPrequelId = prequelId.GetAttributeValue("href", "").Split('/');
               
            }


            if (sequelId != null)
            {
                newSequelId = sequelId.GetAttributeValue("href", "").Split('/');
             
            }


            anime.ID = animeId;
            anime.Rating = Anime.GetRating(rating);
            anime.Type = Anime.GetType(type);
            anime.Episodes = episode == "Unknown" ? 0 : Convert.ToInt32(episode);
            anime.Duration = duration;
            anime.Aired = aired;
            anime.Description = synopsis;
            anime.Status = Anime.GetStatus(status);
            anime.PrequelID = newPrequelId == null ? 0 : Convert.ToInt32(newPrequelId[2]);
     
            anime.SequelID = newSequelId == null ? 0 : Convert.ToInt32(newSequelId[2]);
          
            anime.Genre = genres;
            anime.Title = title;

            return anime;
        }

        private static HtmlDocument GetPage(int idToFetch)
        {
            var getHtmlWeb = new HtmlWeb();
            return getHtmlWeb.Load(string.Format("http://myanimelist.net/anime/{0}", idToFetch));
        }
    }
}