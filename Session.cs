﻿using Npgsql;
using System.Net;

namespace real_time_horror_group3;

public class Session(NpgsqlDataSource db)
{
    public async Task<string> Start(HttpListenerResponse response)
    {
        string message = string.Empty;
        await using var select = db.CreateCommand(@"
                        SELECT COUNT(id)
                        FROM public.session
                        ");

        var reader = await select.ExecuteReaderAsync();
        int count = 1;
        if (await reader.ReadAsync())
        {
            count = reader.GetInt32(0);
        }
        if (count is 0)
        {
            await using var insert = db.CreateCommand(@"
                            INSERT INTO public.session(
	                        time)
	                        VALUES (
                            CURRENT_TIMESTAMP);
                            ");
            await insert.ExecuteNonQueryAsync();

            message = $"Session started at: {DateTime.Now.ToLongTimeString()}";
            response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            message = "Session already in progress";
            response.StatusCode = (int)HttpStatusCode.OK;
        }
        return message;
    }
   
    public async Task<string> EntryPointTimer (HttpListenerResponse response)
    {
        string message = string.Empty;

        await using var select = db.CreateCommand(@"
    UPDATE entry_point
    SET time = CURRENT_TIMESTAMP
    WHERE name = $1 AND room_id = $2 AND type = $3;
");

        return message;
    }
}