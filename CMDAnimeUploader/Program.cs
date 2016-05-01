using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CMDAnimeUploader
{
    class Program
    {
        static void Main(string[] args)
        {

            var dbcontrol = new DatabaseControl();
            var animes = dbcontrol.GetAnime();
            var anime = animes.ToList();
            var urllist = anime.Select(id => string.Format("http://myanimelist.net/anime/{0}", id.ID)).ToList();
            Console.WriteLine("Start: " + GetTime());
            RunAsyncAnime(urllist);
            // RunSynchAnime();

            Console.WriteLine("DONE: " + GetTime());

        }

        private static IEnumerable<XElement> GetElements(string url)
        {
            var animedoc = XElement.Load(url);
            return animedoc.Elements("anime");
        }

        private static void AnimeFunction(int animeId, DatabaseControl dbControl)
        {
            while (true)
            {
                var animeObject = PageScraper.GetAnimeInfo(animeId);
                if (dbControl.AnimeExists(animeId, false))
                {
                    dbControl.InsertGenre(animeId, animeObject.Genre);
                    dbControl.InsertAnime(animeObject);
                    if (animeObject.SequelID != 0)
                    {
                        if (animeObject.SequelID != null && dbControl.AnimeExists((int)animeObject.SequelID, false))
                        {
                            animeId = animeObject.SequelID.Value;
                            continue;
                        }
                        if (animeObject.PrequelID != 0)
                        {
                            if (animeObject.PrequelID != null && dbControl.AnimeExists((int)animeObject.PrequelID, false))
                            {
                                animeId = animeObject.PrequelID.Value;
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    var oldAnime = dbControl.GetAnimeById(animeId);
                    var updateAnime = new UpdateAnime
                    {
                        Aired = animeObject.Aired == oldAnime.Aired ? null : animeObject.Aired,
                        Episodes = animeObject.Episodes == oldAnime.Episodes ? null : animeObject.Episodes,
                        Status = animeObject.Status == oldAnime.Status ? null : animeObject.Status,
                        Duration = animeObject.Duration == oldAnime.Duration ? null : animeObject.Duration,
                        Rating = animeObject.Rating == oldAnime.Rating ? null : animeObject.Rating,
                        PrequelID = animeObject.PrequelID == oldAnime.PrequelID ? null : animeObject.PrequelID,
                        SequelID = animeObject.SequelID == oldAnime.SequelID ? null : animeObject.SequelID,
                        ID = animeId
                    };
                   
                    if (updateAnime.Status != null || updateAnime.Aired != null || updateAnime.Duration != null ||
                        updateAnime.Rating != null || updateAnime.PrequelID != null || updateAnime.SequelID != null ||
                        updateAnime.Episodes != null)
                    {
                        Console.WriteLine("\nAnimeID: " + animeId);
                        if (updateAnime.Status != null)
                        {
                            Console.WriteLine("Status: " + oldAnime.Status + " -> " + updateAnime.Status);
                        }
                        if (updateAnime.Aired != null)
                        {
                            Console.WriteLine("Aired: " + oldAnime.Aired + " -> " + updateAnime.Aired);
                        }
                        if (updateAnime.Duration != null)
                        {
                            Console.WriteLine("Duration: " + oldAnime.Duration + " -> " + updateAnime.Duration);
                        }
                        if (updateAnime.Rating != null)
                        {
                            Console.WriteLine("Rating: " + oldAnime.Rating + " -> " + updateAnime.Rating);
                        }
                        if (updateAnime.PrequelID != null)
                        {
                            Console.WriteLine("Prequel: " + oldAnime.PrequelID + " -> " + updateAnime.PrequelID);
                        }
                        if (updateAnime.SequelID != null)
                        {
                            Console.WriteLine("Sequel: " + oldAnime.SequelID + " -> " + updateAnime.SequelID);
                        }
                        if (updateAnime.Episodes != null)
                        {
                            Console.WriteLine("Episodes: " + oldAnime.Episodes + " -> " + updateAnime.Episodes);
                        }
                        dbControl.UpdateAnime(updateAnime);
                    }
                }
                break;
            }
        }

        private static void RunSynchAnime ()
        {  var dbControl = new DatabaseControl();
            var animeElements = GetElements("http://myanimelist.net/malappinfo.php?u=CWarlord87&status=all&type=anime");

            foreach (var anime in animeElements)
            {
                var myanimeObject = new MyAnime();

                var status = anime.Element("my_status").Value;
                var episodes = Convert.ToInt32(anime.Element("my_watched_episodes").Value);
                var score = Convert.ToInt32(anime.Element("my_score").Value);
                var animeId = Convert.ToInt32(anime.Element("series_animedb_id").Value);

                AnimeFunction(animeId, dbControl);

                if (dbControl.AnimeExists(animeId, true))
                {
                    myanimeObject.AnimeID = animeId;
                    myanimeObject.WatchedEpisodes = episodes;
                    myanimeObject.Score = score;
                    myanimeObject.Status = status;
                    dbControl.InsertAnime(myanimeObject);
                }
                else
                {
                    var myanime = dbControl.GetMyAnimeById(animeId);
                    var myStatus = myanime.GetStatus(status);
                    //change to just update specific items
                    if (Convert.ToInt32(myanime.Status) != myanime.GetStatus(status) ||
                        score != Convert.ToInt32(myanime.Score) || episodes != Convert.ToInt32(myanime.WatchedEpisodes))
                        dbControl.UpdateAnime(animeId, score, episodes, myStatus);
                }
            }

            
        }

        private static void RunAsyncAnime(IList<string> animeUrlList)
        {
            var dbControl = new DatabaseControl();
            var check = new UrlChecker();
            var AnimeList =  check.Check(animeUrlList);
           
            var enumerable = AnimeList as IList<Anime> ?? AnimeList.ToList();
            var counter = -1;
            while (enumerable.Count < animeUrlList.Count)
            {
                if (counter != enumerable.Count)
                {
                    Console.WriteLine(enumerable.Count + "/" + animeUrlList.Count);
                    counter = enumerable.Count;
                }
            }

            foreach (var anime in enumerable)
            {
                if (dbControl.AnimeExists(anime.ID, false))
                {
                    dbControl.InsertGenre(anime.ID, anime.Genre);
                    dbControl.InsertAnime(anime);
                }
                else
                {
                    var oldAnime = dbControl.GetAnimeById(anime.ID);
                    var updateAnime = new UpdateAnime
                    {
                        Aired = anime.Aired == oldAnime.Aired ? null : anime.Aired,
                        Episodes = anime.Episodes == oldAnime.Episodes ? null : anime.Episodes,
                        Status = anime.Status == oldAnime.Status ? null : anime.Status,
                        Duration = anime.Duration == oldAnime.Duration ? null : anime.Duration,
                        Rating = anime.Rating == oldAnime.Rating ? null : anime.Rating,
                        PrequelID = anime.PrequelID == oldAnime.PrequelID ? null : anime.PrequelID,
                        SequelID = anime.SequelID == oldAnime.SequelID ? null : anime.SequelID,
                        ID = anime.ID
                    };
                    if (updateAnime.Status != null || updateAnime.Aired != null || updateAnime.Duration != null ||
                        updateAnime.Rating != null || updateAnime.PrequelID != null || updateAnime.SequelID != null ||
                        updateAnime.Episodes != null)
                    {
                        Console.WriteLine("\nAnimeID: " + anime.ID);
                        if (updateAnime.Status != null)
                        {
                            Console.WriteLine("Status: " + oldAnime.Status + " -> " + updateAnime.Status);
                        }
                        if (updateAnime.Aired != null)
                        {
                            Console.WriteLine("Aired: " + oldAnime.Aired + " -> " + updateAnime.Aired);
                        }
                        if (updateAnime.Duration != null)
                        {
                            Console.WriteLine("Duration: " + oldAnime.Duration + " -> " + updateAnime.Duration);
                        }
                        if (updateAnime.Rating != null)
                        {
                            Console.WriteLine("Rating: " + oldAnime.Rating + " -> " + updateAnime.Rating);
                        }
                        if (updateAnime.PrequelID != null)
                        {
                            Console.WriteLine("Prequel: " + oldAnime.PrequelID + " -> " + updateAnime.PrequelID);
                        }
                        if (updateAnime.SequelID != null)
                        {
                            Console.WriteLine("Sequel: " + oldAnime.SequelID + " -> " + updateAnime.SequelID);
                        }
                        if (updateAnime.Episodes != null)
                        {
                            Console.WriteLine("Episodes: " + oldAnime.Episodes + " -> " + updateAnime.Episodes);
                        }
                        dbControl.UpdateAnime(updateAnime);
                    }
                }
            }
            Console.ReadLine();
        }

        private static string GetTime()
        {
            return string.Format("{0}:{1}:{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
    }
}
