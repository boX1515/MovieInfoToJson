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
        public List<Data> data { get; set; }
    }
    public class Data
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
