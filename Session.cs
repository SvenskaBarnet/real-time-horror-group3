using Npgsql;
using System.Globalization;
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
                            UPDATE entry_point
                            SET time = CURRENT_TIMESTAMP;
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

    public async Task<bool> EntryPointTimer()
    {
        bool gameOver = false;
        await using var cmd = db.CreateCommand(@"
        SELECT to_char(""time"", 'HH24:MI:SS')
        FROM public.entry_point;
        WHERE time is not null;
        ");

        var reader = await cmd.ExecuteReaderAsync();
        DateTime currentTime = DateTime.Now;
        DateTime sessionStart = new();

        while (await reader.ReadAsync())
        {



            sessionStart = reader.GetDateTime(0);


            TimeSpan timeElapsed = currentTime - sessionStart;
            if ((timeElapsed.TotalSeconds > 60))
            {
                gameOver = true;
                break;
            }
            else
            {
                gameOver = false;

            }

        }
        await Console.Out.WriteLineAsync(currentTime.ToLongTimeString());
        return gameOver;
    }
}
