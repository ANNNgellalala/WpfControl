using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfAppTest;

[TemplatePart(Name = "Container", Type = typeof(UniformGrid))]
public class VickyWindow : Window
{
    public UniformGrid Container { get; set; }
    
    static VickyWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VickyWindow), new FrameworkPropertyMetadata(typeof(VickyWindow)));
    }
    
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Container = (GetTemplateChild("Container") as UniformGrid)!;
    }
}
