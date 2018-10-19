using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Retyped.dom;

namespace SQLite
{
    public static class SQLiteStorageModes
    {
        private static Dictionary<string, string> _innerStorage = new Dictionary<string, string>();

        public static void SetItem(string key, string value, StorageMode mode)
        {
            switch(mode)
            {
                case StorageMode.LocalStorage:
                    window.localStorage.setItem(key, value);
                    break;
                case StorageMode.SessionStorage:
                    window.sessionStorage.setItem(key, value);
                    break;
                case StorageMode.DictinaryStorage:

                    if(_innerStorage.ContainsKey(key))
                    {
                        _innerStorage[key] = value;
                    }else
                    {
                        _innerStorage.Add(key, value);
                    }
                    break;
            }
        }

        public static string GetItem(string key, StorageMode mode)
        {
            switch(mode)
            {
                case StorageMode.LocalStorage:
                    return window.localStorage.getItem(key);                    
                case StorageMode.SessionStorage:
                    return window.sessionStorage.getItem(key);
                case StorageMode.DictinaryStorage:
                    if(_innerStorage.ContainsKey(key))
                    {
                        return _innerStorage[key];
                    }                    
                    break;
            }
            return null;
        }
    }
}
