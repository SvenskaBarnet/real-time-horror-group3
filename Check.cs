using Npgsql;
using System.Net;
using System.Text;

namespace real_time_horror_group3;

public class Check(NpgsqlDataSource db)
{
    public async Task<string> Windows(HttpListenerRequest request, HttpListenerResponse response, Player player)
    {
        await using var playerPos = db.CreateCommand(@"
                        SELECT location
                        FROM public.player
                        WHERE name = $1
                        ");
        playerPos.Parameters.AddWithValue(await player.Verify(request, response));

        var reader = await playerPos.ExecuteReaderAsync();
        int roomId = 0;

        if (await reader.ReadAsync())
        {
            roomId = reader.GetInt32(0);
        }

        await using var windows = db.CreateCommand(@"
                        SELECT name, is_locked
                        FROM entry_point
                        WHERE room_id = $1 AND type = 'Window';
                        ");
        windows.Parameters.AddWithValue(roomId);
        var reader2 = await windows.ExecuteReaderAsync();

        string message = string.Empty;
        while (await reader2.ReadAsync())
        {
            switch (reader2.GetBoolean(1))
            {
                case true:
                    message += $"Window {reader2.GetString(0)} is locked.\n";
                    break;
                case false:
                    message += $"Window {reader2.GetString(0)} is unlocked.\n";
                    break;
            }
        }
        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }
    public async Task<string> Doors(HttpListenerRequest request, HttpListenerResponse response, Player player)
    {
        await using var playerPos = db.CreateCommand(@"
                        SELECT location
                        FROM public.player
                        WHERE name = $1
                        ");
        playerPos.Parameters.AddWithValue(await player.Verify(request, response));

        var reader = await playerPos.ExecuteReaderAsync();
        int roomId = 0;

        if (await reader.ReadAsync())
        {
            roomId = reader.GetInt32(0);
        }

        await using var windows = db.CreateCommand(@"
                        SELECT name, is_locked
                        FROM entry_point
                        WHERE room_id = $1 AND type = 'Door';
                        ");
        windows.Parameters.AddWithValue(roomId);
        var reader2 = await windows.ExecuteReaderAsync();

        string message = string.Empty;
        while (await reader2.ReadAsync())
        {
            switch (reader2.GetBoolean(1))
            {
                case true:
                    message += $"Door {reader2.GetString(0)} is locked.\n";
                    break;
                case false:
                    message += $"Door {reader2.GetString(0)} is unlocked.\n";
                    break;
            }
        }
        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }
}