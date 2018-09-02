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
            if (string.IsNullOrEmpty(path.Trim()))
            {
                prefix = "";
            }
            else
            {
                prefix = path + ".";
            }
        }

        public void CreateTable<T>() where T : new()
        {
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item == null)
            {
                List<T> rows = new List<T>();
                string json = JsonConvert.SerializeObject(rows);
                Window.LocalStorage.SetItem(prefix + typeof(T).Name, json);
            }
        }

        public int Insert<T>(T row)
        {
            int index = -1, count = 0;
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
                        if (rows.Count() == 0)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = rows.Max(t => (int)t[name]) + 1;
                        }
                        row[name] = index;
                        count++;
                        rows.Add(row);
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));

                    }
                }
            }
            return count;
        }

        public int Update<T>(T row)
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
                        return 1;
                    }
                }
            }
            return 0;
        }

        public int UpdateAll<T>(IEnumerable<T> updates)
        {
            int count = 0;
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
                        foreach (var row in updates)
                        {
                            var target = rows.Where(t => t[name] == row[name]).First();
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                target[prop.Name] = row[prop.Name];
                            }
                            count++;
                        }
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));
                    }
                }
            }
            return count;
        }

        public int InsertAll<T>(IEnumerable<T> inserts)
        {
            int count = 0;
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
                        foreach (var row in inserts)
                        {
                            if (rows.Count() == 0)
                            {
                                row[name] = 0;
                            }
                            else
                            {
                                row[name] = rows.Max(t => (int)t[name]) + 1;
                            }
                            count++;
                            rows.Add(row);
                        }
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));
                    }
                }
            }
            return count;
        }

        public int Delete<T>(T row)
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
                        bool did = rows.Remove(target);
                        Window.LocalStorage.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows));
                        return 1;
                    }
                }
            }
            return 0;
        }

        public int DeleteAll<T>()
        {
            string json = "[]";
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item != null)
            {
                var str = item.ToString();
                var rows = JsonConvert.DeserializeObject<List<T>>(str);
                Window.LocalStorage.SetItem(prefix + typeof(T).Name, json);
                return rows.Count();
            }
            return 0;
        }

        public List<T> Table<T>()
        {
            var item = Window.LocalStorage.GetItem(prefix + typeof(T).Name);
            if (item != null)
            {
                var json = item.ToString();
                var rows = JsonConvert.DeserializeObject<List<T>>(json);
                return rows;
            }
            throw new Exception("Table " + typeof(T).Name + " is empty.");
        }
    }
}
