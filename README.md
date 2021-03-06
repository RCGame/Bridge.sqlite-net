# Bridge.sqlite-net [![Built with Bridge.NET](https://img.shields.io/badge/built%20with-Bridge.NET-blue.svg)](http://bridge.net/)
Bridge.Net implementation of the popular ORM tool sqlite-net using HTML5 localStorage.
This is a cut-down version. What has been cut? (Although development of these missing pieces is in the roadmap, contribution is also welcomed) 
1. Running SQL query is not expected. Only the ORM part is supported, then you can run LINQ against all entities.
2. Asychronous is not supported, javascript is single-threaded.

# Example Time!

Please refer to the original sqlite-net project, [sqlite-net](https://github.com/praeclarum/sqlite-net).

```csharp
using SQLite;

public class Stock
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Symbol { get; set; }
}

public class Valuation
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public int StockId { get; set; }
	public DateTime Time { get; set; }
	public decimal Price { get; set; }
}

var conn = new SQLiteConnection(databasePath);
conn.CreateTable<Stock>();
conn.CreateTable<Valuation>();

conn.Insert(new Stock() {		
	Symbol = "Awesome stock"		
    });	
conn.Insert(new Stock() {		
        Symbol = "Bluechip"		
    });
    
var query = conn.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));
```

In Html5 localStorage, there is no such a concept of databasePath. You can use databasePath as an item key prefix.
So if your databasePath = "MyStudio.MyGame", above Stock entity will be stored as window.localStorage.getItem("MyStudio.MyGame.Stock");
