using System.Net;
using System.Text;

namespace real_time_horror_group3;

public class StartMenu
{
    public void ShowStartMenu(HttpListenerResponse res)
    {
        string introMessage = "Welcome to NotSoHomeAlone! \n" +
                              "The following are instructions on how to navigate in the game and menus \n" +
                              "To get started with the game you need to send a GET request to /start \n" +
                              "To check a room you need to send a GET request to /room/check \n" +
                              "To check a door you need to send a GET request to /room/door/check \n" +
                              "To check a window you need to send a GET request to /room/door/check \n" +
                              "And finally if you need help or forget how to play you can send a request to /help";

        byte[] converter = Encoding.UTF8.GetBytes(introMessage);
        res.ContentType = "text/plain";
        res.StatusCode = (int)HttpStatusCode.OK;

        res.OutputStream.Write(converter, 0, converter.Length);
        res.OutputStream.Close();
    }
    
}