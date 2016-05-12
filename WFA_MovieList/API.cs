using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace WFA_MovieList
{
    public class API
    {
        private string movieSearchURL = "http://api.themoviedb.org/3/search/movie?api_key=";
        private string GenreURL = "http://api.themoviedb.org/3/genre/movie/list?api_key=";
        private DateTime date;
        public async Task<Tuple<results,MovieInfo>> RetrieveInfo(string data)
        {

            date = new DateTime(int.Parse("0001"), 1, 1);
            string Searcheditem = "";
            HttpClient client = new HttpClient();
            if(data.Contains("."))
            {
              Searcheditem =  data.Replace('.', ' ');
            }
            else
            {
                Searcheditem = data;
            }
            string[] dates = new string[] {
                "(1980)","(1981)","(1982)","(1983)","(1984)","(1985)","(1986)","(1987)","(1988)","(1989)",
                "(1990)","(1991)","(1992)","(1993)","(1994)","(1995)","(1996)","(1997)","(1998)","(1999)",
                "(2000)","(2001)","(2002)","(2003)","(2004)","(2005)","(2006)","(2007)","(2008)","(2009)",
                "(2010)","(2011)","(2012)","(2013)","(2014)","(2015)","(2016)","(2017)",
                "1980","1981","1982","1983","1984","1985","1986","1987","1988","1989",
                "1990","1991","1992","1993","1994","1995","1996","1997","1998","1999",
                "2000","2001","2002","2003","2004","2005","2006","2007","2008","2009",
                "2010","2011", "2012", "2013", "2014", "2015", "2016", "2017"
            };

            string[] specialStrings = new string[]
            {
                "SLOSubs","COMPLETE"
            };

            foreach(var item in dates)
            {
                if(Searcheditem.Contains(item))
                {
                    var position = Searcheditem.IndexOf(item);
                    var editedInfo = Searcheditem.Remove(position);
                    Searcheditem = editedInfo;
                    Searcheditem = Searcheditem.TrimEnd();
                    var datum = item;
                    if (item.Contains("(") || item.Contains(")"))
                    {
                        
                        datum = datum.Replace('(', ' ');
                        datum = datum.Replace(')', ' ');
                        datum.Trim();
                        DateTime datum_ = new DateTime(int.Parse(datum), 1, 1);
                        date = new DateTime();
                        date = datum_;
                    }
                    else
                    {
                        DateTime datum_ = new DateTime(int.Parse(datum), 1, 1);
                        date = new DateTime();
                        date = datum_;
                    }
                    break;
                }
            }
            foreach(var item in specialStrings)
            {
                if(Searcheditem.Contains(item))
                {
                    var position = Searcheditem.IndexOf(item);
                    var editedInfo = Searcheditem.Remove(position);
                    Searcheditem = editedInfo;
                    Searcheditem = Searcheditem.TrimEnd();
                }
            }
            //Sestavljanje URL-ja za klic na API z api-keyem ter search parametrom
            string url_API = "";
            if(Properties.Settings.Default.APIKey.Length != 0  && Properties.Settings.Default.APIKey != null)
            {
                url_API = movieSearchURL + Properties.Settings.Default.APIKey + "&query=" + Searcheditem;
                try
                {
                    GlobalVar.GlobalApiCall.Counter++;
                    var response = await client.GetStringAsync(url_API);
                    var jsonData = JsonConvert.DeserializeObject<DataAPI>(response);
                    results apiResult = new results();
                    for (int i = 0; i < jsonData.results.Count; i++)
                    {
                        DateTime jsonDate = Convert.ToDateTime(jsonData.results[i].release_date);
                        if (date.Year != 1 && date != null)
                        {
                                if (jsonDate >= date || jsonData.results[i].title == Searcheditem)
                                {
                                    apiResult.id = jsonData.results[i].id;
                                    apiResult.title = jsonData.results[i].title;
                                    apiResult.genre_ids = jsonData.results[i].genre_ids;
                                    apiResult.poster_path = jsonData.results[i].poster_path;
                                    break;
                                }
                        }
                    }
                    GlobalVar.GlobalApiCall.Counter++;
                    var GetMovieInfoUrl = "http://api.themoviedb.org/3/movie/"+ apiResult.id +"?api_key=" + Properties.Settings.Default.APIKey;
                    var responseInfo = await client.GetStringAsync(GetMovieInfoUrl);
                    var movieInfoJsonObject = JsonConvert.DeserializeObject<MovieInfo>(responseInfo);
                    return new Tuple<results, MovieInfo>(apiResult, movieInfoJsonObject);
                }
                catch(Exception e)
                {
                    Form1 frm = new Form1();
                    frm.label2.Text = "Error calling API!";
                    Console.Write(e.ToString() + " | "+ e.Message);
                }
                
            }
            else
            {
                return new Tuple<results, MovieInfo>(null,null);
                
            }
            return new Tuple<results, MovieInfo>(null, null);
        }
        public async Task<DataGenre> RetriveGenres()
        {
            HttpClient client = new HttpClient();
            try
            {
                var data = await client.GetStringAsync(GenreURL + Properties.Settings.Default.APIKey);
                var jsonData = JsonConvert.DeserializeObject<DataGenre>(data);
                return jsonData;
            }
            catch(Exception e)
            {
                Form1 frm = new Form1();
                frm.label2.Text = "Error retrieving Genres from API!";
                Console.Write(e.ToString() + " | " + e.Message);
                
            }
            return new DataGenre();
            
        }
    }
}
