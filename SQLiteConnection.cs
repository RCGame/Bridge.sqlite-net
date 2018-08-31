using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bridge.Html5;

namespace SQLite
{
    public class PrimaryKey : Attribute
    {
    }

    public class AutoIncrement : Attribute
    {
    }

    //Html5 Local Storage implementation of SQLite
    public class SQLiteConnection
    {
        private string prefix;

        public SQLiteConnection(string path)
        {
            prefix = path + ".";
        }

        public void CreateTable<T>() where T : new()
        {
            T row = new T();
            List<T> rows = new List<T>();
            rows.Add(row);
            string json = JsonConvert.SerializeObject(rows);
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item == null)
            {
                Window.LocalStorage.SetItem(prefix + typeof(T).Name, json);
            }
        }

        public T Insert<T>(T row)
        {
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var linq = typeof(T).GetProperties().Where(t => t.GetCustomAttributes().Count() > 0);
                foreach (var property in linq)
                {
                    var name = property.Name;
                    var attrs = property.GetCustomAttributes();
                    if (attrs.Where(t => t.ToString() == "SQLite.AutoIncrement").Count() > 0)
                    {
                        row[name] = rows.Max(t => (int)t[name]) + 1;
                        rows.Add(row);
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));
                    }
                }              
            }
            return row;
        }

        public void Update<T>(T row)
        {
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var linq = typeof(T).GetProperties().Where(t => t.GetCustomAttributes().Count() > 0);
                foreach (var property in linq)
                {
                    var name = property.Name;
                    var attrs = property.GetCustomAttributes();
                    if (attrs.Where(t => t.ToString() == "SQLite.PrimaryKey").Count() > 0)
                    {
                        var target = rows.Where(t => t[name] == row[name]).First();
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            target[prop.Name] = row[prop.Name];
                        }
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));
                    }
                }
            }
        }

        public void UpdateAll<T>(IEnumerable<T> rows)
        {
            foreach (var row in rows)
            {
                Update(row);
            }
        }

        public void InsertAll<T>(IEnumerable<T> rows)
        {
            foreach (var row in rows)
            {
                Insert(row);
            }
        }

        public int DeleteAll<T>()
        {
            string json = "[]";
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item == null)
            {
                var tmp = Window.LocalStorage.GetItem(prefix + typeof(T).Name).ToString();
                var rows = JsonConvert.DeserializeObject<List<T>>(tmp);
                Window.LocalStorage.SetItem(prefix + typeof(T).Name, json);
                return rows.Count();
            }
            return 0;
        }

        public List<T> Table<T>()
        {
            throw new NotImplementedException();
        }
    }
}
