using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBEngine
{
    public class DBMap
    {
        public Hashtable dbHasTable { get { return dbmap; } }


        Hashtable dbmap = new Hashtable();

        public DBMap()
        { 
            
        }

        public void initMap(string mapini_Path)
        {
            StreamReader sr = new StreamReader(mapini_Path, Encoding.UTF8);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                string[] ReadText = s.Split(';');
                dbmap.Add(ReadText[0], ReadText[1]);
            }
        }
    }
}
