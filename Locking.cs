using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace real_time_horror_group3;

public class Locking
{
    public static async Task WindowA()
    {
        string dbUri = "Host =localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
        await using var db = NpgsqlDataSource.Create(dbUri);

        await using var cmd = db.CreateCommand(@"
                UPDATE entry_point
                SET is_locked = True
                WHERE name = 'Window A';
            ");

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Window A is now locked.");
    }

    public static async Task WindowB()
    {
        string dbUri = "Host =localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
        await using var db = NpgsqlDataSource.Create(dbUri);

        await using var cmd = db.CreateCommand(@"
                UPDATE entry_point
                SET is_locked = True
                WHERE name = 'Window B';
            ");

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Window B is now locked.");
    }

    public static async Task DoorA()
    {
        string dbUri = "Host =localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
        await using var db = NpgsqlDataSource.Create(dbUri);

        await using var cmd = db.CreateCommand(@"
                UPDATE entry_point
                SET is_locked = True
                WHERE name = 'Door A';
            ");

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Door A is now locked.");
    }
}
