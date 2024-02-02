using Npgsql;
using System.Net;
using System.Text;
namespace real_time_horror_group3;

public class Check(NpgsqlDataSource db)
{
    public async Task Door(HttpListenerResponse response)
    {
        string message = "Here is a door";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.ContentType = "text/plain";
        response.StatusCode = (int)HttpStatusCode.OK;

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public async Task Window(HttpListenerResponse response)
    {
        string message = "Here is a window";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.ContentType = "text/plain";
        response.StatusCode = (int)HttpStatusCode.OK;

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }


    async Task<string> GetStatus(int roomId, string type)
    {
        
    }

}