using Npgsql;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;

namespace real_time_horror_group3; 
public class GameEvent(NpgsqlDataSource db)
{
    public async Task UnlockEntry()
    {
        await using var entryCount = db.CreateCommand(@"
            SELECT COUNT(id)
            FROM public.entry_point;
            ");
        var reader1 = await entryCount.ExecuteReaderAsync();
        int totalEntry = 0;
        if (reader1.Read()) 
        {
            totalEntry = reader1.GetInt32(0); 
        }

        Random random = new Random();
        int randomEntry = random.Next(1, totalEntry);

        await using var lockEntry = db.CreateCommand(@"
            UPDATE public.entry_point
            SET is_locked = false 
            WHERE id = $1;
            ");
        lockEntry.Parameters.AddWithValue(randomEntry);
        await lockEntry.ExecuteNonQueryAsync();
    }

    public async void RandomTrigger(Session session,GameEvent gameEvent)
    {
        TimeSpan timeElapsed = await session.ElapsedTime();

        double baseProbability = 0.1;
        double exponentialRate = 0.05;
        double timeInterval = timeElapsed.TotalMinutes;

        double probability = baseProbability * Math.Exp(exponentialRate * timeInterval);

        Random random = new Random();
        double randomValue = random.NextDouble();

        if (randomValue <= probability)
        {
            await gameEvent.UnlockEntry();
        }
    }
}