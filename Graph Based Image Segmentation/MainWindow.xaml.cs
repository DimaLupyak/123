using ImageSegmentation.ViewModel;
using System.Windows;

namespace ImageSegmentation.View
{
    public enum ExampleEnum { FooBar, BarFoo }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
