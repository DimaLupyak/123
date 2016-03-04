using ImageSegmentation.ViewModel;
using ImageSegmentationModel;
using System;
using System.Windows;

namespace ImageSegmentation.View
{
    public enum ExampleEnum { FooBar, BarFoo }
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            segmentationMethods.ItemsSource = Enum.GetValues(typeof(SegmentationMethod));
            connectingMethods.ItemsSource = Enum.GetValues(typeof(ConnectingMethod));
            difType.ItemsSource = Enum.GetValues(typeof(ColorDifference));
        }
    }
}
