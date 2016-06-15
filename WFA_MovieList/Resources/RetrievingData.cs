using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WFA_MovieList.Resources
{
    class ImageDownload
    {
        private HttpClient client = new HttpClient();
        public async Task<Image> ImageDownloader (string URL)
        {
            var response = await client.GetStreamAsync(URL);
            return Image.FromStream(response);
        }

        public async Task<Button> CreatingMovieButtons(MovieData item)
        {
            Button bttn = new Button()
            {
                Height = 300,
                Width = 200,
                Visible = true,
                BackgroundImage = await ImageDownloader("https://image.tmdb.org/t/p/w300/" + item.DBposter),
                BackgroundImageLayout = ImageLayout.Stretch,
                Tag = item.DBid,
            };
            return bttn;
        }
    }
}
