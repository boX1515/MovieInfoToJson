using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFA_MovieList.Resources;

namespace WFA_MovieList
{
    public partial class Edit : Form
    {
        public Data item = new Data();
        public Edit()
        {
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            //DATA
            ImageDownload imageDL = new ImageDownload();
            var image = await imageDL.ImageDownloader("https://image.tmdb.org/t/p/w300/" + item.DBposter);
            pictureBox1.Width = 200; pictureBox1.Height = 300;
            pictureBox1.Image = image;
            label9.Text = item.DBTitle;
            id_textbox.Text = item.ID.ToString();
            name_textbox.Text = item.Name;
            filename_textbox.Text = item.FileName;
            serverLocation_textbox.Text = item.ServerLocation;
            string combobox = "";
            for(int i = 0; i < item.DBgenres.Count;i++)
            {
                if(i != item.DBgenres.Count - 1) combobox += item.DBgenres[i] + "\n";
                else combobox += item.DBgenres[i];
            }
            genres_comboBox.Text = combobox;
            dbid_textbox.Text = item.DBid.ToString();
            dbtitle_textbox.Text = item.DBTitle;
            dbposter_textbox.Text = item.DBposter;
            linkLabel1.Text = "https://www.themoviedb.org/movie/" + item.DBid;

            //INFO
            adult_comboBox1.SelectedIndex = adult_comboBox1.FindStringExact(item.Info.adult.ToString());
            backdrop_path_textbox.Text = item.Info.backdrop_path;
            homepage_textbox.Text = item.Info.homepage;
            release_date_textbox.Text = item.Info.release_date;
            imdb_id_textbox.Text = item.Info.imdb_id;
            original_title_textbox.Text = item.Info.original_title;
            posterpath_textbox.Text = item.Info.poster_path;
            popularity_textbox.Text = item.Info.popularity;
            revenue_textbox.Text = item.Info.revenue.ToString();
            status_textbox.Text = item.Info.status;
            tagline_textbox.Text = item.Info.tagline;
            title_textbox.Text = item.Info.title;
            overview_textbox.Text = item.Info.overview;
            vote_average_textbox.Text = item.Info.vote_average.ToString();
            vote_count_textbox.Text = item.Info.vote_count.ToString();
            budget_textbox.Text = item.Info.budget.ToString();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
            
        }
    }
}
