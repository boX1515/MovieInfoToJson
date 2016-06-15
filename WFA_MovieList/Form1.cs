using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using WFA_MovieList.Resources;
using WFA_MovieList.Database;


namespace WFA_MovieList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static List<string> LocationList;
        public static Dictionary<string,List<string>> Files;
        public static Dictionary<string,string> LocationDictionary;
        public List<string> Data;

        private JSON dataManipulation = new JSON();
        public API APICall = new API();

        public string Title = "MovieDataToJson";
        public string APICallTitle = "Processing ... ";

        private Database.Database db = new Database.Database();

        private void Form1_Load(object sender, EventArgs e)
        {
            var data = db.getDataFromDB();
            textBox2.Text = Properties.Settings.Default.APIKey;
            if (LocationList == null)
            {
                LocationList = new List<string>();
            }
            if(LocationDictionary == null)
            {
                LocationDictionary = new Dictionary<string, string>();
            }
            if(Files == null)
            {
                Files = new Dictionary<string,List<string>>();
            }
        }

        private void setFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                if (!LocationList.Contains(folderBrowserDialog1.SelectedPath))
                {
                    LocationDictionary.Add(LocationDictionary.Count + 1.ToString(), folderBrowserDialog1.SelectedPath);
                    //LocationList.Add(folderBrowserDialog1.SelectedPath);
                }
            }
            if (!LocationList.Contains(folderBrowserDialog1.SelectedPath))
            {
                LocationList.Add(folderBrowserDialog1.SelectedPath);
                listBox2.DataSource = null;
            }
            listBox2.DataSource = LocationList;
            listBox2.Refresh();
            
        }
        private void updateMainFolders()
        {
            var list = new List<string>();
            var movieList = new List<string>();
            Data = new List<string>();
            int dirCount = 0;
            if(LocationDictionary != null)
            {
                foreach (string key in LocationDictionary.Keys)
                {
                    var dir = LocationDictionary[key];

                    list = Directory.GetDirectories(dir).ToList();
                    foreach (var item in list)
                    {
                        foreach (var dirFiles in Directory.GetFiles(item))
                        {
                            if (dirFiles.EndsWith(".mp4"))
                            {
                                movieList.Add(item);
                            }
                        }

                    }
                    if(Files != null)
                    {
                        if (Files.ContainsKey(dir))
                        {
                            Files.Remove(dir);
                        }
                        Files.Add(dirCount.ToString(), movieList);
                        dirCount++;
                    }
                    

                }
            }

            if(Files != null)
            {
                foreach (var key in Files.Keys)
                {
                    var data_dir = Files[key].ToArray();
                    for (int i = 0; i < data_dir.Length; i++)
                    {
                        if (!Data.Contains(data_dir[i]))
                        {
                            Data.Add(data_dir[i]);
                        }

                    }
                }
            }

            if(Data != null)
            {
                if (listBox1.DataSource != null) { listBox1.DataSource = null; listBox1.DataSource = Data; }
                else { listBox1.DataSource = Data; }
                listBox1.Update();

                progressBar1.Maximum = Data.Count;
                label5.Text = Data.Count.ToString();
            }
            
        }
        
        private void updateFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateMainFolders();
        }
        //private int countCall = 0;

        //Event za gumb v flow layout panelu 
        private void button_movie_Click(object sender, EventArgs e)
        {
            var item = ((Button)sender).Tag;
            MovieData newItem = new MovieData();
            newItem = GlobalVar.GlobalJsonObject.data.Where(x => x.DBid.ToString() == item.ToString()).First();
            Edit formLoad = new Edit();
            formLoad.item = newItem;
            if(formLoad.ShowDialog() == DialogResult.OK)
            {
                if (formLoad.item != newItem)
                {
                    newItem = formLoad.item;
                    //GlobalVar.GlobalJsonObject.data.Remove()
                }
            }

        }
       
        private async void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            
            var objectToSerialize = new JSON();
            objectToSerialize.data = new List<MovieData>();
            bool isItemInImportedJson = false;
            GlobalVar.GlobalApiCall = new Count();
            for (int i = 0; i < Data.Count;i++)
            {
                progressBar1.Value = progressBar1.Value + 1;

                flowLayoutPanel1.Enabled = false;

                MovieData item = new MovieData();
                item.Name = new DirectoryInfo(Data[i]).Name;
                label2.Text = "Checking Movie Info in Database ... ";
                var status = db.DatabaseCheckName(item.Name);
                if(status.Item1 != "Item is in database!") // preverba v bazi
                {
                    
                    //PREGLEJ DA BO HITREJŠE!!
                    if (GlobalVar.GlobalJsonObject != null)
                    {
                        isItemInImportedJson = true;
                        var isInGlobal = GlobalVar.GlobalJsonObject.data.Where<MovieData>(x => x.Name == item.Name).FirstOrDefault();
                        if (isInGlobal == null)
                        {

                            item.ID = GlobalVar.GlobalJsonObject.data.Count + 1;
                            item = await RetrievingMovieData(item, i);
                            objectToSerialize.data.Add(item);
                            GlobalVar.GlobalJsonObject.data.Add(item);
                        }
                    }
                    else
                    {
                        label2.Text = "Movie is not in Database, retrieving from API ... ";
                        item.ID = i;
                        item = await RetrievingMovieData(item, i);
                        objectToSerialize.data.Add(item);

                    }
                }
                else
                {
                    item = dataManipulation.ConvertingDataToMovieData(status.Item2);
                    objectToSerialize.data.Add(item);
                    //creating buttons in flow layout panel for easy view
                    ImageDownload imageProcessing = new ImageDownload();
                    var responseBttn = await imageProcessing.CreatingMovieButtons(item);
                    responseBttn.Click += new EventHandler(button_movie_Click);
                    flowLayoutPanel1.Controls.Add(responseBttn);
                }

                

                //check in local database to see if movie is already in
            }
            if (isItemInImportedJson) 
            {
                //Sorting objects inside list to display items with descending date
                GlobalVar.GlobalJsonObject.data.Sort(delegate (MovieData x, MovieData y)
                {
                    DateTime item1 = Convert.ToDateTime(x.Info.release_date);
                    DateTime item2 = Convert.ToDateTime(y.Info.release_date);
                    if (item1 == item2) return 0;
                    else if (item1 > item2) return -1;
                    else if (item2 > item1) return 1;
                    else return item1.CompareTo(item2);
                });
                //Serializing
                var json = JsonConvert.SerializeObject(GlobalVar.GlobalJsonObject, Formatting.Indented);
                textBox1.Text = json;
            }
            else
            {
                //Sorting objects inside list to display items with descending date
                objectToSerialize.data.Sort(delegate (MovieData x, MovieData y)
                {
                    DateTime item1 = Convert.ToDateTime(x.Info.release_date);
                    DateTime item2 = Convert.ToDateTime(y.Info.release_date);
                    if (item1 == item2) return 0;
                    else if (item1 > item2) return -1;
                    else if (item2 > item1) return 1;
                    else return item1.CompareTo(item2);
                });
                //Serializing && setting global variable for JSON object as list
                GlobalVar.GlobalJsonObject = objectToSerialize;
                //var json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
                //textBox1.Text = json;
            }
            label2.Text = "Checking database for changes...";
            
            db.DatabaseDataCheck(GlobalVar.GlobalJsonObject);
            label2.Text = "Completed";
            this.Text = Title;
            flowLayoutPanel1.Enabled = true;
        }
        
        

        private async Task<MovieData> RetrievingMovieData(MovieData item,int i)
        {
            
            foreach (var key in Directory.GetFiles(Data[i]))
            {
                if (key.EndsWith("mp4", false, null))
                {
                    var movieDataInDir = Path.GetFileName(key);
                    item.FileName = movieDataInDir;
                    break;
                }
            }

            var driveLetter = Path.GetPathRoot(Data[i]);
            item.ServerLocation = "/Movies_" + driveLetter[0] + "/" + item.Name + "/" + item.FileName;
            //countCall++;
            if (GlobalVar.GlobalApiCall.Counter >= 38)
            {
                GlobalVar.GlobalApiCall.Counter = 1;
                label2.Text = "Waiting 5s before calling API...";
                await Task.Delay(5000);


            }
            else
            {
                this.Text = APICallTitle + item.Name;
                label2.Text = "Retrieving Movie Info From API...";
            }

            var response = await APICall.RetrieveInfo(item.Name);

            if (response.Item1 != null)
            {
                item.DBid = response.Item1.id;
                item.DBTitle = response.Item1.title;
                item.DBgenres = response.Item1.genre_ids;
                item.DBposter = response.Item1.poster_path;
            }
            else
            {
                item.DBid = 000000;
                item.DBTitle = "Unknown";
                item.DBgenres = new List<int>();
                item.DBposter = "";
            }
            if (response.Item2 != null)
            {
                item.Info = response.Item2;
            }
            return item;
        }

        private void jSONExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(GlobalVar.GlobalJsonObject !=  null)
            {
                var json = JsonConvert.SerializeObject(GlobalVar.GlobalJsonObject, Formatting.Indented);
                folderBrowserDialog1.ShowNewFolderButton = true;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if(result == DialogResult.OK)
                {
                    File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\data.json", json);
                    MessageBox.Show("File creation completed!");
                }
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(textBox1.Text);
        }

        private async void getGenreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var data = await APICall.RetriveGenres();
                if(data.genres != null)
                {
                    File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\genres.json", JsonConvert.SerializeObject(data));
                    MessageBox.Show("File creation completed!");
                }
                
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.APIKey = textBox2.Text;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void jSONFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "JSON files(*.json)|*.json";
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var stringJson = "";
                    using (StreamReader sr = new StreamReader(fileDialog.FileName))
                    {
                        // Read the stream to a string, and write the string to the console.
                        stringJson = sr.ReadToEnd();
                    }
                    var jsonObject = JsonConvert.DeserializeObject<JSON>(stringJson);
                    GlobalVar.GlobalJsonObject = new JSON();
                    GlobalVar.GlobalJsonObject = jsonObject;
                    textBox3.Text = stringJson;

                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message,"Error");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if(MessageBox.Show("Are you sure you want to remove this folder?", "Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    //Error at listbox deleting a selected item
                    var list = listBox2.Items;
                    List<string> items = new List<string>();
                    Dictionary<string, string> zac = new Dictionary<string, string>();
                    for (int i = 0; i < listBox2.Items.Count; i++)
                    {
                        if (listBox2.SelectedItem != listBox2.Items[i])
                        {
                            items.Add(listBox2.Items[i].ToString());
                            zac.Add(i.ToString(), listBox2.Items[i].ToString());
                        }
                    }
                    if(items != null)
                    {
                        listBox2.DataSource = null;
                        listBox2.DataSource = items;
                        LocationDictionary.Clear();
                        LocationDictionary = zac;
                        if(LocationDictionary.Keys.Count != 0)
                        {
                            Files = new Dictionary<string, List<string>>(); //shranjevanje lokacij vseh map
                            updateMainFolders();
                        }
                        else
                        {
                            ClearAllData();
                        }
                        
                    }
                    
                    /*for(int i = 0; i < list.Count;i++)
                    {
                        listBox2.Items.Insert(i, list[i]);
                    }*/
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Erro" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void ClearAllData()
        {
            LocationDictionary = null;
            Data = null;
            Files = null;
            listBox1.DataSource = null;
            listBox2.DataSource = null;
        }

        private void refresh_button_Click(object sender, EventArgs e)
        {
            updateMainFolders();
        }

        private void refresh_button_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(refresh_button, "Refresh the main folder to get sub-folders.");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(button3, "Remove selected main folder from list.");
        }

    }
}
