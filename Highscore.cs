using Npgsql;
using System;
using System.Net;

namespace real_time_horror_group3;

public class Highscore(NpgsqlDataSource db)
{
    public async Task<bool> HandleGameOver(HttpListenerRequest request, HttpListenerResponse response)
    {
        Player player = new(db);
        string playerName = await player.Verify(request, response);
        await using var cmd = db.CreateCommand(@"
        SELECT COUNT(*) 
        FROM public.player
        WHERE name = $1 AND is_dead = true;
        ");
        cmd.Parameters.AddWithValue(playerName);

        var reader = await cmd.ExecuteReaderAsync();
        int deadPlayer = 0;
        while (await reader.ReadAsync())
        {
            deadPlayer = reader.GetInt32(0);
        }

        response.StatusCode = (int)HttpStatusCode.OK;
        if (deadPlayer == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task AddGameOverTime(HttpListenerRequest request, HttpListenerResponse response)
    {
        Session session = new(db);
        Player player = new(db);
        string playerName = await player.Verify(request, response);
        var time = session.FormattedTime();
        var highscore = db.CreateCommand(@"
        INSERT INTO public.highscore(player_name, session_time)
        VALUES ($1, $2);
    ");
        highscore.Parameters.AddWithValue(playerName);
        highscore.Parameters.AddWithValue(time);
        await highscore.ExecuteNonQueryAsync();
    }



}