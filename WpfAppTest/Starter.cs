namespace WpfAppTest;

public class Starter
{
    [STAThread]
    private static void Main(
        string[] args)
    {
        var app = new App();
        app.Run();
    }
}
