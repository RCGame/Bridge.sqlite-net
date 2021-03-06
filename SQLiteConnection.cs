﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Retyped.dom;

namespace SQLite
{
    public class PrimaryKey : Attribute
    {
    }

    public class AutoIncrement : Attribute
    {
    }

    public enum StorageMode
    {
        LocalStorage,
        SessionStorage,
        /// <summary>
        /// single process only
        /// </summary>
        DictinaryStorage
    }

    //Html5 Local Storage implementation of SQLite
    public class SQLiteConnection
    {
        internal string prefix;
        internal StorageMode StorageMode;
        public SQLiteConnection(string path = "", StorageMode storageMode = StorageMode.LocalStorage)
        {
            prefix = string.IsNullOrEmpty(path) ? "" : path + ".";
            StorageMode = storageMode;
        }

        public void CreateTable<T>() where T : new()
        {            
            var item = getTable<T>();
            var meta = SQLiteTableMeta.GetMeta<T>(this);

            if(item == null)
            {
                setTable<T>(new List<T>());                
            }
        }

        public int Insert<T>(T row)
        {
            int count = 0;
            var item = getTable<T>();
            if(item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var meta = SQLiteTableMeta.GetMeta<T>(this);
                if(meta.HasAutoIncrement())
                {
                    row[meta.AutoIncrementName] = meta.NextAutoIncrement();
                }
                rows.Add(row);
                count++;

                setTable(rows);
            }
            return count;
        }

        public int Update<T>(T row)
        {
            var item = getTable<T>();
            if(item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var meta = SQLiteTableMeta.GetMeta<T>(this);
                if(meta.HasPrimaryKeyName())
                {
                    int length = rows.Count;
                    string primaryKey = meta.PrimaryKeyName;
                    for(int i = 0; i < length; i++)
                    {
                        if(rows[i][primaryKey] == row[primaryKey])
                        {
                            rows[i] = row;

                            setTable(rows);

                            return 1;
                        }
                    }
                }
            }
            return 0;
        }

        public int UpdateAll<T>(IEnumerable<T> updates)
        {
            int count = 0;
            var item = getTable<T>();
            if(item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item);
                var meta = SQLiteTableMeta.GetMeta<T>(this);
                if(meta.HasPrimaryKeyName())
                {
                    string primaryKey = meta.PrimaryKeyName;
                    var order = updates.OrderByDescending(b => b[meta.PrimaryKeyName]).ToList();
                    for(int i = rows.Count - 1; i > 0 && order.Count > 0; i--)
                    {
                        for(int x = order.Count - 1; x > 0; x--)
                        {
                            var orderItem = order[x];
                            if(rows[i][primaryKey] == orderItem[primaryKey])
                            {
                                rows[i] = orderItem;
                                order.RemoveAt(x);

                                count++;
                                break;
                            }
                        }
                    }

                    if(count > 0)
                        setTable(rows);
                }
            }
            return count;
        }

        public int InsertAll<T>(IEnumerable<T> inserts)
        {
            int count = 0;
            var item = getTable<T>();
            if(item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var meta = SQLiteTableMeta.GetMeta<T>(this);
                bool increment = meta.HasAutoIncrement();

                foreach(var row in inserts)
                {
                    if(increment)
                    {
                        row[meta.AutoIncrementName] = meta.NextAutoIncrement(false);
                    }
                    rows.Add(row);
                    count++;
                }
                meta.Save();
                setTable(rows);
            }
            return count;
        }

        public int Delete<T>(T row)
        {
            var item = getTable<T>();
            if(item != null)
            {
                List<T> rows = JsonConvert.DeserializeObject<List<T>>(item.ToString());
                var meta = SQLiteTableMeta.GetMeta<T>(this);

                if(meta.HasPrimaryKeyName())
                {
                    int length = rows.Count;
                    string primaryKey = meta.PrimaryKeyName;
                    for(int i = 0; i < length; i++)
                    {
                        if(rows[i][primaryKey] == row[primaryKey])
                        {
                            rows.RemoveAt(i);

                            setTable(rows);

                            return 1;
                        }
                    }
                }
            }
            return 0;
        }

        public int DeleteAll<T>()
        {
            var item = getTable<T>();
            if(item != null)
            {
                var rows = JsonConvert.DeserializeObject<List<T>>(item);
                setTable(new List<T>());
                return rows.Count();
            }
            return 0;
        }

        private string getTable<T>()
        {
            return SQLiteStorageModes.GetItem(prefix + typeof(T).Name, StorageMode);            
        }

        private void setTable<T>(List<T> rows)
        {
            SQLiteStorageModes.SetItem(prefix + typeof(T).Name, JsonConvert.SerializeObject(rows), StorageMode);            
        }

        public List<T> Table<T>()
        {
            var item = getTable<T>();
            if(item != null)
            {
                var json = item.ToString();
                var rows = JsonConvert.DeserializeObject<List<T>>(json);
                return rows;
            }
            throw new Exception("Table " + typeof(T).Name + " is empty.");
        }
    }
}
