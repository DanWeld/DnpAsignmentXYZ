using Microsoft.Data.Sqlite;
using System.Text;

var dbPath = Path.Combine("..","..","Server","WebAPI","app.db");
if (!File.Exists(dbPath))
{
    Console.WriteLine($"DB not found at {Path.GetFullPath(dbPath)}");
    return;
}

using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

string[] tables = { "Users", "Posts", "Comments" };
foreach (var table in tables)
{
    Console.WriteLine($"--- {table} ---");
    using var cmd = conn.CreateCommand();
    cmd.CommandText = $"SELECT * FROM {table} LIMIT 100;";
    using var reader = cmd.ExecuteReader();
    var hasRows = false;
    while (reader.Read())
    {
        hasRows = true;
        var sb = new StringBuilder();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            sb.AppendFormat("{0}={1}; ", reader.GetName(i), reader.GetValue(i));
        }
        Console.WriteLine(sb.ToString());
    }
    if (!hasRows) Console.WriteLine("(no rows)");
}

conn.Close();

