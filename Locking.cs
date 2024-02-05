using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace real_time_horror_group3;



public class Locking(NpgsqlDataSource db)
{

    public async Task Lock(string type, int roomId, HttpListenerRequest req, HttpListenerResponse res)
    {
        StreamReader reader = new(req.InputStream, req.ContentEncoding);
        string lockName = reader.ReadToEnd();

       
        await using var cmd = db.CreateCommand(@$"

            UPDATE entry_point
            SET is_locked = True
            WHERE name = $1 AND room_id = $2 AND type = $3");

        cmd.Parameters.AddWithValue(lockName);
        cmd.Parameters.AddWithValue(roomId);
        cmd.Parameters.AddWithValue(type);

        await cmd.ExecuteNonQueryAsync();
 

        string message = $"{lockName} is now locked";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        res.ContentType = "text/plain";
        res.StatusCode = (int)HttpStatusCode.OK;

        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.OutputStream.Close();
    }

}



/*
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




void Lock(HttpListenerResponse response, string itemName)
{
    string message = $"{itemName} is now locked";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}

*/
