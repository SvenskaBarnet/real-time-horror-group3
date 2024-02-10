using Npgsql;
using System.Net;
using System.Text;

namespace real_time_horror_group3;

public class Check(NpgsqlDataSource db)
{
    private GameEvent gameEvent = new GameEvent(db);
    private Session session = new Session(db);
    public async Task<string> Windows(HttpListenerRequest request, HttpListenerResponse response, Player player)
    {
        int roomId = await PlayerPosition(request, response, player);

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

        gameEvent.RandomTrigger(session, gameEvent);
        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }
    public async Task<string> Doors(HttpListenerRequest request, HttpListenerResponse response, Player player)
    {
        int roomId = await PlayerPosition(request, response, player);

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
        gameEvent.RandomTrigger(session, gameEvent);
        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }

    public async Task<string> EntryPoints(HttpListenerRequest request, HttpListenerResponse response, string playerName)
    {
        await using var playerPos = db.CreateCommand(@"
            SELECT p.location, r.name
            FROM public.player p
            JOIN public.room r ON r.id = p.location
            WHERE p.name = $1;
            ");

        playerPos.Parameters.AddWithValue(playerName ?? string.Empty);
        var reader1 = await playerPos.ExecuteReaderAsync();

        int roomId = 0;
        string roomName = string.Empty;
        if (await reader1.ReadAsync())
        {
            roomId = reader1.GetInt32(0);
            roomName = reader1.GetString(1);
        }

        int doors = await GetEntries(roomId, "Door");
        int windows = await GetEntries(roomId, "Window");

        string message = $"You are in the {roomName}. \nThere is {doors} door(s) and {windows} window(s).";

        return message;
    }

    async Task<int> GetEntries(int roomId, string type)
    {
        await using var entryPoints = db.CreateCommand(@"
            SELECT COUNT(type) 
            FROM entry_point
            WHERE room_id = $1 AND type = $2;
            ");
        entryPoints.Parameters.AddWithValue(roomId);
        entryPoints.Parameters.AddWithValue(type);
        var reader2 = await entryPoints.ExecuteReaderAsync();

        int count = 0;
        while (await reader2.ReadAsync())
        {
            count = reader2.GetInt32(0);
        }

        return count;
    }
    public async Task<int> PlayerPosition(HttpListenerRequest request, HttpListenerResponse response, Player player)
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

        return roomId;
    }
}