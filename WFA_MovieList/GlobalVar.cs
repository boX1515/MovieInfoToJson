using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_MovieList
{
    class GlobalVar
    {
        static Count _globalApiCall;
        public static Count GlobalApiCall
        {
            get
            {
                return _globalApiCall;
            }
            set
            {
                _globalApiCall = value;
            }
        }

        static JSON _globalJsonObject;
        public static JSON GlobalJsonObject
        {
            get
            {
                return _globalJsonObject;
            }
            set
            {
                _globalJsonObject = value;
            }
        }
       /*static List<JSON> _globalJsonList;
        public static List<JSON> GlobalJsonList
        {
            get
            {
                return _globalJsonList;
            }
            set
            {
                _globalJsonList = value;
            }
        }*/

    }
    public class Count
    {
        public int Counter { get; set; }
    }
}
