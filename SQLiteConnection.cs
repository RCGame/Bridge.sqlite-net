using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public SQLiteConnection(string path)
        {

        }

        public void CreateTable<T>() where T : new()
        {
            T b = new T();
            string json = JsonConvert.SerializeObject(b);
            var linq = typeof(T).GetProperties().Where(t => t.GetCustomAttributes().Count() > 0);
            if (linq.Count() > 0)
            {
                var attrs = linq.First().GetCustomAttributes();
                Console.WriteLine(linq.First().Name + " " + attrs.Aggregate((c, d) => c.ToString() + "," + d.ToString()));
            }
            Console.WriteLine(json);
        }

        public T Insert<T>(T row)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T row)
        {

        }

        public void UpdateAll<T>(IEnumerable<T> rows)
        {

        }

        public void InsertAll<T>(IEnumerable<T> rows)
        {

        }

        public int DeleteAll<T>()
        {
            return 0;
        }

        public List<T> Table<T>()
        {
            throw new NotImplementedException();
        }
    }
}
