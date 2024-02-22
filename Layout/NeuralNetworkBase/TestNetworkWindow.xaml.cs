using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeuralNetworkBase
{
    /// <summary>
    /// Interaction logic for TestNetworkWindow.xaml
    /// </summary>
    public partial class TestNetworkWindow : Window
    {
        StreamWriter savingFile = null;
        private bool isDrawing = false;
        private Point startPoint;

        public TestNetworkWindow()
        {
            InitializeComponent();
        }
        public void Drawing(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point currentPoint = e.GetPosition(mCanvas);
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 10;
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = currentPoint.X;
                line.Y2 = currentPoint.Y;
                startPoint = currentPoint;
                mCanvas.Children.Add(line);
            }
        }
        private List<int> ReadCanvasPixels()
        {
            int width = (int)mCanvas.ActualWidth;
            int height = (int)mCanvas.ActualHeight;

            // Pobierz kontekst pikseli dla elementu Canvas
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(mCanvas);

            int stride = width * 4; // Każdy piksel zajmuje 4 bajty
            byte[] pixels = new byte[height * stride];
            rtb.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            List<int> pixelList = new List<int>();
            List<byte> scaledPixelList = new List<byte>();
            int newWidth = 28;
            int newHeight = 28;

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int originalX = x * width / newWidth;
                    int originalY = y * height / newHeight;
                    int originalIndex = (originalY * width + originalX) * 4;

                    for (int j = 0; j < 4; j++)
                    {
                        scaledPixelList.Add(pixels[originalIndex + j]);
                    }
                }
            }

            // Przejście przez piksele i interpretacja jako czarne lub białe
            for (int i = 0; i < scaledPixelList.Count; i += 4)
            {
                if (scaledPixelList[i] != 255 && scaledPixelList[i+1] != 255 && scaledPixelList[i+2] != 255)
                {
                    pixelList.Add(1); // pokolorowany piksel
                }
                else
                {
                    pixelList.Add(0); // Biały piksel
                }
            }

            return pixelList;
        }
        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(mCanvas);
        }
        public void EndDraw(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }
        public void EndDraw(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }
        private string SelectTxtFile(string typeOfSelectingFile)
        {
            string path = null;
            try
            {
                OpenFileDialog selectingTxtFile = new OpenFileDialog();
                selectingTxtFile.InitialDirectory = Directory.GetCurrentDirectory();
                selectingTxtFile.Filter = "Text files (*.txt)|*.txt";
                selectingTxtFile.Title = typeOfSelectingFile;

                if (selectingTxtFile.ShowDialog() == true)
                {
                    path = selectingTxtFile.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return path;
        }
        private void ChooseTrainingData_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectTxtFile("Plik zapisu do sieci");
            if (path != null)
            {
                savingFile = new StreamWriter(path);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<int> pixels = ReadCanvasPixels();
            int black = 0;
            int empty = 0;
            foreach(var element in pixels)
            {
                if(element == 0 )
                {
                    empty++;
                }
                else
                {
                    black++;
                }
            }
            TestTextBox.Text = "Empty: " + empty + "\n Black: " + black;
        }
    }
}
