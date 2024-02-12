using Npgsql;
using System;
using System.Net;

namespace real_time_horror_group3;

public class Highscore(NpgsqlDataSource db)
{
    public async Task HandleGameOver()
    {
        Player player = new(db);
        string playerName = await player.Verify();
        await using var cmd = db.CreateCommand(@"
        UPDATE public.player
        SET is_dead = true
        WHERE name = $1;
    ");
        cmd.Parameters.AddWithValue(playerName);
        await cmd.ExecuteNonQueryAsync();

    }

    public async Task AddGameOverTime(string playerName)
    {
        Session session = new(db);
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