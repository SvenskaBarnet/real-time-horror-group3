using Npgsql;
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
}