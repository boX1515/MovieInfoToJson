using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WFA_MovieList
{
    public class JSON
    {
        public List<MovieData> data { get; set; }

        public Data convertFromMovieData (MovieData item,int id)
        {
            Data convItem = new Data()
            {
                ID = id,
                Name = item.Name,
                FileName = item.FileName,
                ServerLocation_ = item.ServerLocation,
                DBid = Convert.ToString(item.DBid),
                DBTitle = item.DBTitle,
                DBposter = item.DBposter
            };
            string genres = "";
            for(int i = 0; i < item.DBgenres.Count;i++)
            {
                if(i == item.DBgenres.Count - 1)
                {
                    genres += item.DBgenres[i];
                }
                else
                {
                    genres += item.DBgenres[i] + ";";
                }
            }
            convItem.DBgenres = genres;
            if (item.Info != null) { convItem.Info = convertFromMovieInfo(item.Info, convItem.ID); } else { convItem.Info = new Info(); }
            return convItem;
        }

        public Info convertFromMovieInfo(MovieInfo item, int id)
        {
            Info convItem = new Info()
            {
                ID = id,
                adult = item.adult,
                backdrop_path = item.backdrop_path,
                budget = Convert.ToString(item.budget),
                homepage = item.homepage,
                imdb_id = item.imdb_id,
                original_title = item.original_title,
                overview = item.overview,
                popularity = item.popularity,
                poster_path = item.poster_path,
                release_date = item.release_date,
                revenue = Convert.ToString(item.revenue),
                runtime = Convert.ToString(item.runtime),
                status = item.status,
                tagline = item.tagline,
                title = item.title,
                vote_average = Convert.ToString(item.vote_average),
                vote_count = Convert.ToString(item.vote_count)
            };
            return convItem;
        }

        public MovieData ConvertingDataToMovieData(Data item)
        {
            MovieData data = new MovieData()
            {
                ID = item.ID,
                Name = item.Name,
                FileName = item.FileName,
                ServerLocation = item.ServerLocation_,
                DBid = Convert.ToInt32(item.DBid),
                DBTitle = item.DBTitle,
                DBposter = item.DBposter,
                DBgenres = new List<int>(),
            };

            var genres = item.DBgenres.Split(';').ToList();
            foreach (var g in genres) { data.DBgenres.Add(Convert.ToInt32(g)); }
            data.Info = ConvertingInfoToMovieInfo(item.Info);
            return data;
        }

        public MovieInfo ConvertingInfoToMovieInfo(Info item)
        {
             MovieInfo mi = new MovieInfo()
            {
                backdrop_path = item.backdrop_path,
                budget = Convert.ToInt32(item.budget),
                homepage = item.homepage,
                imdb_id = item.imdb_id,
                original_title = item.original_title,
                overview = item.overview,
                popularity = item.popularity,
                poster_path = item.poster_path,
                release_date = item.release_date,
                revenue = Convert.ToInt32(item.revenue),
                runtime = Convert.ToInt32(item.runtime),
                status = item.status,
                tagline = item.tagline,
                title = item.title,
                vote_average = Convert.ToDouble(item.vote_average),
                vote_count = Convert.ToInt32(item.vote_count)
            };
            bool value;
            Boolean.TryParse(item.adult.ToString(), out value);
            mi.adult = value;
            return mi;
        }
    }
    public class MovieData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ServerLocation { get; set; }
        public List<int> DBgenres { get; set; }
        public int DBid { get; set; }
        public string DBTitle { get; set; }
        public string DBposter { get; set; }
        public MovieInfo Info { get; set; }
    }

    public class MovieInfo
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public int budget { get; set; } 
        public string homepage { get; set; }
        public string imdb_id { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string popularity { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public int revenue { get; set; }
        public int runtime { get; set; }
        public string status { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public double vote_average { get; set; }
        public double vote_count { get; set; }
}
    /// <summary>
    /// API Class for transforming JSON string to OBJECT
    /// </summary>
    public class DataAPI
    {
        public List<results> results { get; set; }
    }
    public class results
    {
        public int id { get; set; }
        public string title { get; set; }
        public string release_date { get; set; }
        public List<int> genre_ids { get; set; }
        public string poster_path { get; set; }
    }

    public class DataGenre
    {
        public List<GenresList> genres { get; set; }
    }
    public class GenresList
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
