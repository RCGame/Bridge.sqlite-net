# Bridge.sqlite-net
Bridge.Net implementation of the popular ORM tool sqlite-net using HTML5 localStorage.
This is a cut-down version. What has been cut?
1. Running SQL query is not expected. Only the ORM part is supported, then you can run LINQ against all entities.
2. Asychronised is not supported, javascript is single-threaded.

# Example Time!

Please refer to the original sqlite-net project, [sqlite-net](https://github.com/praeclarum/sqlite-net).

```csharp
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
```
