using ImageSegmentation.ViewModel;
using ImageSegmentationModel;
using System;
using System.Windows;

namespace ImageSegmentation.View
{
    public enum ExampleEnum { FooBar, BarFoo }
    public partial class MainWindow : ThemedWindow
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            dataStructure.ItemsSource = Enum.GetValues(typeof(DataStructure));
            sortModification.ItemsSource = Enum.GetValues(typeof(SortModification));
            margeHeuristic.ItemsSource = Enum.GetValues(typeof(MargeHeuristic));
            connectingMethods.ItemsSource = Enum.GetValues(typeof(ConnectingMethod));
            difType.ItemsSource = Enum.GetValues(typeof(ColorDifference));
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                viewModel.OpenImage(files[0]);
            }
        }

        private void Image_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.OpenImage();
        }
    }
}
