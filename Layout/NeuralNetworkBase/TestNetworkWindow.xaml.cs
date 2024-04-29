using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private bool isDrawing = false;
        private System.Windows.Point startPoint;
        NeuralNetworkInputData inputData;
        NeuralNetwork myNetwork;

        public TestNetworkWindow()
        {
            InitializeComponent();
            ReadTestData();
            GetNetwork();
        }
        private void ReadTestData()
        {
            try
            {
                StreamReader sr = new StreamReader("NetworkTest/testData.json");
                string json = sr.ReadToEnd();
                inputData = JsonConvert.DeserializeObject<NeuralNetworkInputData>(json);
                sr.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error while reading inputData");
            }
        }
        private void GetNetwork()
        {
            try
            {
                StreamReader sr = new StreamReader("NetworkTest/54-20-10FirstNetwork.json");
                string json = sr.ReadToEnd();
                myNetwork = JsonConvert.DeserializeObject<NeuralNetwork>(json);
                sr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to set Network");
            }
        }
        public void Drawing(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                System.Windows.Point currentPoint = e.GetPosition(mCanvas);

                // Pierwsza linia
                double[] thickness = { 1, 2, 3, 4, 5, 6, 7, 7.2,7.4,7.6,7.8,8, 9,9.1, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7,9.8, 9.9, 10 };
                foreach(var thick in thickness)
                {
                    Line line1 = new Line();
                    line1.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                    line1.StrokeThickness = thick;
                    line1.X1 = startPoint.X;
                    line1.Y1 = startPoint.Y;
                    line1.X2 = currentPoint.X;
                    line1.Y2 = currentPoint.Y;
                    mCanvas.Children.Add(line1);
                }
                startPoint = currentPoint;


            }
        }
        private List<double> ReadCanvasPixels()
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)secondCanvas.ActualWidth, (int)secondCanvas.ActualHeight, 96, 96, PixelFormats.Default);
            renderBitmap.Render(secondCanvas);

            // Utwórz obraz z renderBitmap jako Image control
            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
            imageControl.Source = renderBitmap;
            
            // Convert the render target bitmap to a Bitmap
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                Bitmap bitmap = new Bitmap(ms);
                for (int y=1; y<bitmap.Height; y++)
                {
                    for(int x=1; x<bitmap.Width;x++)
                    {
                        {

                           colorList.Add(bitmap.GetPixel(x, y));

                        }
                    }
                }

            }
            List<double> pixelList = new List<double>();
            foreach(var element in colorList)
            {
                if (element.ToArgb() == System.Drawing.Color.White.ToArgb())
                {
                    pixelList.Add(0);       // 0 to bialy kolor
                }
                else
                {
                    pixelList.Add(1);       // 1 to inny kolor
                }
            }
            
            return pixelList;
        }
        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(mCanvas);
        }
        private void ShowNetworkResult()
        {
            RecognizeAndDrawCharacter();
            List<double> pixels = ReadCanvasPixels();
            networkResultText.Text = myNetwork.CalculateSmallNetworkResult(pixels.ToArray()).ToString();
        }
        public void EndDraw(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            ShowNetworkResult();
        }
        void SaveTestData()
        {
            StreamWriter savingFile = new StreamWriter("NetworkTest/testData.json");
            string json = JsonConvert.SerializeObject(inputData,Formatting.Indented);
            savingFile.Write(json);
            savingFile.Close();
        }
        private void AddDataBtnClick(object sender, RoutedEventArgs e)
        {
            RecognizeAndDrawCharacter();
            List<double> pixels = ReadCanvasPixels();
            if (pixels != null && pixels.Count == 54)
            {
                try
                {
                    int expectedResult = Int32.Parse(expectedResultText.Text);
                    inputData.inputData.Add(pixels.ToArray());
                    inputData.trainingResults.Add(expectedResult);
                    SaveTestData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void RecognizeAndDrawCharacter()
        {
            secondCanvas.Children.Clear();
            secondCanvas.Width = 10;
            secondCanvas.Height = 7;
            List<System.Windows.Point> characterPoints = GetCharacterPoints(); // Pobierz punkty, które reprezentują kształt

            // Jeśli punkty reprezentują kształt, narysuj go na secondCanvas
            if (characterPoints.Count > 0)
            {
                // Znajdź minimalne i maksymalne współrzędne punktów, aby obliczyć rozmiar kształtu
                double minX = characterPoints.Min(p => p.X);
                double maxX = characterPoints.Max(p => p.X);
                double minY = characterPoints.Min(p => p.Y);
                double maxY = characterPoints.Max(p => p.Y);

                // Oblicz wymiary kształtu
                double shapeWidth = maxX - minX;
                double shapeHeight = maxY - minY;

                // Przelicz współczynniki skalowania, aby przeskalować kształt do wymiaru 10x7 pikseli
                double scaleX = 10.0 / shapeWidth;
                double scaleY = 7.0 / shapeHeight;

                // Wybierz mniejszy współczynnik skalowania, aby zachować proporcje
                double scale = Math.Min(scaleX, scaleY);

                // Oblicz przesunięcie, aby wyśrodkować kształt na kanwie
                double offsetX = (10 - shapeWidth * scale) / 2;
                double offsetY = (7 - shapeHeight * scale) / 2;

                // Narysuj przeskalowany kształt
                for (int i = 0; i < characterPoints.Count - 1; i++)
                {
                    Line line = new Line();
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.StrokeThickness = 1; // Grubość linii może być dostosowana
                    line.X1 = (characterPoints[i].X - minX) * scale + offsetX;
                    line.Y1 = (characterPoints[i].Y - minY) * scale + offsetY;
                    line.X2 = (characterPoints[i + 1].X - minX) * scale + offsetX;
                    line.Y2 = (characterPoints[i + 1].Y - minY) * scale + offsetY;

                    secondCanvas.Children.Add(line);
                }
            }
            secondCanvas.UpdateLayout();
        }

        private List<System.Windows.Point> GetCharacterPoints()
        {
            List<System.Windows.Point> points = new List<System.Windows.Point>();

            foreach (var child in mCanvas.Children)
            {
                if (child is Line)
                {
                    Line line = (Line)child;
                    points.Add(new System.Windows.Point(line.X1, line.Y1));
                    points.Add(new System.Windows.Point(line.X2, line.Y2));
                }
            }

            return points;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            mCanvas.Children.Clear();
            secondCanvas.Children.Clear();
        }
    }

}
