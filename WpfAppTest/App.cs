using System.Windows;
using Calendar;

namespace WpfAppTest;

public class App : Application
{
    protected override void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var window = new VickyWindow();
        window.Show();
        // var obj = LoadComponent(new Uri("/Calendar;component/Themes/Styles/SmartDate.xaml", UriKind.Relative)) as ResourceDictionary;
        // var style = obj!["SmartDateStyle"] as Style;
        // window.Container.Children.Add(new SmartDate()
        // {
        //     Style = style,
        //     Margin = new Thickness(20)
        // });
    }
}
