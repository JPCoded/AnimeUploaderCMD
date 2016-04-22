using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace CMDAnimeUploader
{
    internal class DatabaseControl : IDisposable
    {
        //move to own settings file
        private readonly SqlConnection _connection =
            new SqlConnection(
                "Data Source=DESKTOP-AQTJ6NL\\ANIMELIST;Initial Catalog=MyAnimeList;Integrated Security=True");

        public void Dispose()
        {
            _connection.Close();
        }

        public bool GenreExist(int animeId, int genreId)
        {
            var genre = _connection.Query("GetGenreByGenreId", new { AnimeId = animeId, GenreId = genreId },
                commandType: CommandType.StoredProcedure);
            return !genre.Any();

        }

        public MyAnime GetMyAnimeById(int animeId)
        {
            return _connection.Query<MyAnime>("GetMyAnimeById", new { ID = animeId },
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<Anime> GetAnime()
        {
            return _connection.Query<Anime>("select * from Anime").ToList();
        }

        public Anime GetAnimeById(int animeId)
        {
            return _connection.Query<Anime>("GetAnimeById", new { ID = animeId },
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public void UpdateAnime(int animeId, int score = -1, int watchedEpisodes = -1, int status = -1)
        {
            _connection.Execute("UpdateMyAnime",
                new { ID = animeId, Score = score, WatchedEpisodes = watchedEpisodes, Status = status },
                commandType: CommandType.StoredProcedure);
        }

        public void InsertAnime(MyAnime myAnime)
        {
            _connection.Execute("InsertMyAnime", myAnime, commandType: CommandType.StoredProcedure);
        }

        public void InsertAnime(Anime anime)
        {
            _connection.Execute("InsertAnime", anime, commandType: CommandType.StoredProcedure);
        }

        public void UpdateAnime(UpdateAnime anime)
        {
            _connection.Execute("UpdateAnime", anime, commandType: CommandType.StoredProcedure);
        }

        public void InsertGenre(int animeId, string genres)
        {
            var genre = genres.Split(',');
            foreach (var g in genre)
            {
                _connection.Execute("InsertGenre", new { AnimeID = animeId, Genre = g },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public List<GetAnime> GetAllMyListId()
        {
            return _connection.Query<GetAnime>("Select AnimeId from MyAnimeList").ToList();
        }

        public bool AnimeExists(int animeDb, bool checkMyList)
        {
            bool doesExist;
            if (checkMyList)
            {
                var list = _connection.Query<MyAnime>("Select ID from MyAnimeList where AnimeID = " + animeDb).ToList();
                doesExist = list.Count == 0;
            }
            else
            {
                var list = _connection.Query<Anime>("Select ID from Anime where ID = " + animeDb).ToList();
                doesExist = list.Count == 0;
            }
            return doesExist;
        }

    }
}
