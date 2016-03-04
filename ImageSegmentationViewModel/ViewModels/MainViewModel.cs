using ImageSegmentationModel;
using ImageSegmentationModel.Filter;
using ImageSegmentationModel.Segmentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageSegmentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private PerfomanceInfo perfomanceInfo;
        public PerfomanceInfo PerfomanceInfo
        {
            get
            {
                return perfomanceInfo;
            }
            set
            {
                perfomanceInfo  = value;
                RaisePropertyChanged("PerfomanceInfo");
            }
        }

        private int _k = 100;
        public int K
        {
            get
            {
                return _k;
            }
            set
            {
                _k = value;
                RaisePropertyChanged("K");
            }
        }
        private float _sigma = 0.8f;
        public float Sigma
        {
            get
            {
                return _sigma;
            }
            set
            {
                _sigma = value;
                RaisePropertyChanged("Sigma");
            }
        }
        private int _minSize = 10;
        public int MinSize
        {
            get
            {
                return _minSize;
            }
            set
            {
                _minSize = value;
                RaisePropertyChanged("MinSize");
            }
        }
        private DataStructure dataStructure = DataStructure.SimpleGhaph;
        public DataStructure DataStructure
        {
            get
            {
                return dataStructure;
            }
            set
            {
                dataStructure = value;
                RaisePropertyChanged("DataStructure");
            }
        }

        private SortModification sortModification = SortModification.WithSorting;
        public SortModification SortModification
        {
            get
            {
                return sortModification;
            }
            set
            {
                sortModification = value;
                RaisePropertyChanged("SortModification");
            }
        }

        private MargeHeuristic margeHeuristic = MargeHeuristic.K;
        public MargeHeuristic MargeHeuristic
        {
            get
            {
                return margeHeuristic;
            }
            set
            {
                margeHeuristic = value;
                RaisePropertyChanged("MargeHeuristic");
            }
        }

        private ConnectingMethod connection = ConnectingMethod.Connected_8;
        public ConnectingMethod Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = value;
                RaisePropertyChanged("Connection");
            }
        }

        private ColorDifference difType = ColorDifference.RGB_std_deviation;
        public ColorDifference DifType
        {
            get
            {
                return difType;
            }
            set
            {
                difType = value;
                RaisePropertyChanged("DifType");
            }
        }

        public MainViewModel()
        {
        }

        private RelayCommand _openImageCommand;
        public RelayCommand OpenImageCommand
        {
            get { return _openImageCommand ?? (_openImageCommand = new RelayCommand(OpenImage)); }
        }

        private RelayCommand _gaussianImageCommand;
        public RelayCommand GaussianImageCommand
        {
            get { return _gaussianImageCommand ?? (_gaussianImageCommand = new RelayCommand(GaussianImage)); }
        }
        

        private RelayCommand _segmentImageCommand;
        public RelayCommand SegmentImageCommand
        {
            get { return _segmentImageCommand ?? (_segmentImageCommand = new RelayCommand(SegmentImage)); }
        }
        
        private void OpenImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image files (*.gif;*.jpg;*.jpe;*.png;*.bmp;*.dib;*.tif;*.tifF;*.wmf;*.ras;*.eps;*.pcx;*psd;*.tga)|*.gif;*.jpg;*.jpe;*.png;*.bmp;*.dib;*.tif;*.tifF;*.wmf;*.ras;*.eps;*.pcx;*psd;*.tga";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                SegmentedImage = null;
                OriginImage = new ImageViewModel(new Bitmap(dlg.FileName));

                ClearPerfomanceInfo();
                RaisePropertyChanged("PerfomanceInfo");
            }
           
        }


        public void SegmentImage()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                CanNewExecute = false;
                try
                {
                    IFhSegmentation segmentation = SegmentationFactory.Instance.GetFhSegmentation(DataStructure, SortModification, MargeHeuristic);
                    RGB[,] pixels = ImageHelper.GetPixels(OriginImage.Bitmap);
                    if (pixels != null)
                    {
                        GaussianFilter filter = new GaussianFilter();
                        filter.Filter(OriginImage.Bitmap.Width, OriginImage.Bitmap.Height, pixels, Sigma);
                        int[,] segments = segmentation.BuildSegments(OriginImage.Bitmap.Width, OriginImage.Bitmap.Height, pixels, K, MinSize, Connection, DifType, ref perfomanceInfo);                       
                        SegmentedImage = new ImageViewModel(ImageHelper.GetBitmap(segments));
                        RaisePropertyChanged("PerfomanceInfo");
                    }
                }
                catch { }
                finally
                {
                    CanNewExecute = true;
                }
            });
        }

       
        protected void ClearPerfomanceInfo()
        {
            perfomanceInfo.AlgorithmPerfomance = 0;
            perfomanceInfo.BuildingPerfomance = 0;
            perfomanceInfo.SmallSegmentMargingPerfomance = 0;
            perfomanceInfo.SortingPerfomance = 0;
        }


        public void GaussianImage()
        {
            Task task = Task.Factory.StartNew(() =>
            {

                ClearPerfomanceInfo();
                RaisePropertyChanged("PerfomanceInfo");
                CanNewExecute = false;
                try
                {
                    GaussianFilter filter = new GaussianFilter();
                    RGB[,] pixels = ImageHelper.GetPixels(OriginImage.Bitmap);
                    if (pixels != null)
                    {
                        filter.Filter(OriginImage.Bitmap.Width, OriginImage.Bitmap.Height, pixels, Sigma);
                        
                            SegmentedImage = new ImageViewModel(ImageHelper.GetFilterBitmap(pixels));
                    }
                }
                catch { }
                finally
                {
                    CanNewExecute = true;
                }
            });
        }


        private List<RelayCommand> _commands;
        private IList<RelayCommand> Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new List<RelayCommand>();
                    _commands.Add(OpenImageCommand);
                    _commands.Add(SegmentImageCommand);
                    _commands.Add(GaussianImageCommand);
                }
                return _commands.AsReadOnly();
            }
        }


        private bool _canNewExecute = true;
        public bool CanNewExecute
        {
            get { return _canNewExecute; }
            set
            {
                if (value != _canNewExecute)
                {
                    _canNewExecute = value;
                    RaisePropertyChanged("CanNewExecute");
                }
            }
        }

        private ImageViewModel _originImage;
        public ImageViewModel OriginImage
        {
            get { return _originImage; }
            set
            {
                if (value != _originImage)
                {
                    _originImage = value;
                    RaisePropertyChanged("OriginImage");
                }
            }
        }

        private ImageViewModel _segmentedImage;
        public ImageViewModel SegmentedImage
        {
            get { return _segmentedImage; }
            set
            {
                if (value != _segmentedImage)
                {
                    _segmentedImage = value;
                    RaisePropertyChanged("SegmentedImage");
                }
            }
        }

        #region Implement INotyfyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}