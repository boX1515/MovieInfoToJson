using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFA_MovieList.Database
{
    public class Database
    {
        MovieDatabaseEntities database = new MovieDatabaseEntities();

        public List<Data> getDataFromDB()
        {
            return (from m in database.Data select m).ToList();
        }

        public async Task<string> checkMovieInDatabase(MovieData item)
        {
            JSON js = new JSON();
            try
            {
                database.Database.Connection.Open();
                var dbData = database.Data.Where(x => x.DBTitle == item.DBTitle).FirstOrDefault();
                var id = database.Data.Count();
                if (dbData == null)
                {
                    database.Data.Add(js.convertFromMovieData(item,id));
                    var status = await database.SaveChangesAsync();
                    CloseDBConn(database);
                    return "Item is added!";
                }
                CloseDBConn(database);
                return "Item exists!";
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        CloseDBConn(database);
                        return "Error on: " + validationError.PropertyName +"\nwith ErrorMessage: "+ validationError.ErrorMessage;

                    }
                }
                CloseDBConn(database);
                return "Error";
            }


        }

        private void CloseDBConn(MovieDatabaseEntities db)
        {
            db.Database.Connection.Close();
        }
        public List<string> Errors = new List<string>();
        public async void DatabaseDataCheck(JSON list)
        {
            for(int i = 0; i < list.data.Count;i++)
            {
                var status = await checkMovieInDatabase(list.data[i]);
                if(status != "Item is added!" && status != "Item exists!")
                {
                    Errors.Add(list.data[i].DBTitle);
                }
            }
            if(Errors.Count != 0)
            {
                string err = "";
                foreach (var item in Errors)
                {
                    err += item + "\n";
                }
                MessageBox.Show("There were errors!\nCheck the ERRORS bellow:\n" + err);
            }
            
        }
        public Tuple<string,Data> DatabaseCheckName(string name)
        {
            var data = database.Data.Where(x => x.Name == name).FirstOrDefault();
            if(data != null)
            {
                return new Tuple<string, Data>("Item is in database!",data);
            }
            return new Tuple<string, Data>("Item is not in database", new Data());
        }

        
    }
}
