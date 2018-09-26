
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Retyped.dom;

namespace SQLite
{
    public class SQLiteTableMeta
    {
        private string _metaString;
        private bool _wasNew;
        public long AutoIncrementTotal = 0;
        public string AutoIncrementName;
        public string PrimaryKeyName;
        private SQLiteConnection _innerConnection;
        /// <summary>
        /// The amount to increase 
        /// </summary>
        public long AutoIncrementSet = 1;

        public const string TableMetaExtension = ".meta";       

        public bool HasAutoIncrement()
        {                        
            return !string.IsNullOrWhiteSpace(AutoIncrementName);
        }
        public bool HasPrimaryKeyName()
        {
            return !string.IsNullOrWhiteSpace(PrimaryKeyName);
        }

        public long NextAutoIncrement(bool saveMeta = true)
        {
            AutoIncrementTotal += AutoIncrementSet;

            if(saveMeta)
            {
                Save();
            }            

            return AutoIncrementTotal;
        }

        private static Dictionary<string, SQLiteTableMeta> memoryTableMeta = new Dictionary<string, SQLiteTableMeta>();

        public void Save()
        {
            SQLiteStorageModes.SetItem(_metaString, JsonConvert.SerializeObject(this), this._innerConnection.StorageMode);
        }

        public bool WasNew()
        {
            return _wasNew;
        }

        private static SQLiteTableMeta createMeta<T>(string metaString, SQLiteConnection connection)
        {
            var newMetaTagle = new SQLiteTableMeta();
            newMetaTagle._innerConnection = connection;
            newMetaTagle._wasNew = true;

            var properties = typeof(T).GetProperties().Where(t => t.GetCustomAttributes().Count() > 0).ToArray();
            int length = properties.Length;
            bool foundAuto = false;
            bool foundPrimary = false;
            for(int i = 0; i < length; i++)
            {
                var property = properties[i];
                var attrs = property.GetCustomAttributes();
                if(attrs == null || attrs.Length == 0)
                {
                    continue;
                }

                var name = property.Name;
                bool foundboth = false;
                
                foreach(var attribute in attrs)
                {
                    if(!foundAuto && attribute.ToString() == "SQLite.AutoIncrement")
                    {
                        newMetaTagle.AutoIncrementName = name;
                        foundAuto = true;
                    }

                    if(!foundPrimary && attribute.ToString() == "SQLite.PrimaryKey")
                    {
                        newMetaTagle.PrimaryKeyName = name;
                        foundPrimary = true;
                    }

                    if(foundPrimary && foundAuto)
                    {
                        foundboth = true;
                        break;
                    }
                }

                if(foundboth)
                    break;                
            }

            // store in local storage.
            newMetaTagle.Save();            

            return newMetaTagle;
        }        

        public static SQLiteTableMeta GetMeta<T>(SQLiteConnection connection)
        {
            string metaString = connection.prefix + typeof(T).Name + TableMetaExtension;
            
            var item = SQLiteStorageModes.GetItem(metaString, connection.StorageMode);
            if(item == null)
            {
                var newMeta = createMeta<T>(metaString, connection);
                newMeta._metaString = metaString;

                memoryTableMeta.Add(metaString, newMeta);

                return newMeta;
            }
            else
            {
                SQLiteTableMeta oldMeta = null;
                // if there is an exception...
                if(memoryTableMeta.ContainsKey(metaString))
                {
                    oldMeta = memoryTableMeta[metaString];
                }else
                {
                    oldMeta = JsonConvert.DeserializeObject<SQLiteTableMeta>(item);                   
                }

                oldMeta._innerConnection = connection;
                oldMeta._metaString = metaString;
                return oldMeta;
            }            
        }
    }
}
