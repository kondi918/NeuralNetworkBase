using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
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
        System.Windows.Point startPoint;
        List<List<System.Windows.Point>> points = new List<List<System.Windows.Point>>();
        NeuralNetworkInputData inputData;
        NeuralNetwork myNetwork;

        public TestNetworkWindow()
        {
            InitializeComponent();
            ReadTestData();
            GetNetwork();
            clonnedCanvas.Visibility = Visibility.Visible;
            clonnedCanvas.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));
            clonnedCanvas.Arrange(new Rect(0, 0, clonnedCanvas.ActualWidth, clonnedCanvas.ActualHeight));
            clonnedCanvas.Visibility = Visibility.Hidden;
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
                StreamReader sr = new StreamReader("NetworkTest/256-50-10LEARNEDVERSION2.json");
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
                List<System.Windows.Point> currentPoints = new List<System.Windows.Point>();
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
    
                    currentPoints.Add(startPoint);
                    currentPoints.Add(currentPoint);
                    points.Add(currentPoints);

                
                startPoint = currentPoint;

                RecognizeAndDrawCharacter();

            }

        }
        private void ClonningCanvas()
        {
            clonnedCanvas.Visibility = Visibility.Visible;
            clonnedCanvas.Children.Clear();

            foreach (UIElement element in secondCanvas.Children)
            {
                if (element is Line line)
                {
                    Line clonedLine = new Line
                    {
                        X1 = line.X1,
                        Y1 = line.Y1,
                        X2 = line.X2,
                        Y2 = line.Y2,
                        Stroke = line.Stroke,
                        StrokeThickness = line.StrokeThickness,
                        StrokeDashArray = new DoubleCollection(line.StrokeDashArray),
                        StrokeStartLineCap = line.StrokeStartLineCap,
                        StrokeEndLineCap = line.StrokeEndLineCap,
                        StrokeLineJoin = line.StrokeLineJoin
                    };

                    clonnedCanvas.Children.Add(clonedLine);
                }
            }
            clonnedCanvas.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));
            clonnedCanvas.Arrange(new Rect(0, 0, clonnedCanvas.ActualWidth, clonnedCanvas.ActualHeight));
        }
        private List<double> ReadCanvasPixels()
        {
            ClonningCanvas();
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)clonnedCanvas.ActualWidth, (int)clonnedCanvas.ActualHeight, 96, 96, PixelFormats.Default);
            renderBitmap.Render(clonnedCanvas);

            // Utwórz obraz z renderBitmap jako Image control
            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
            imageControl.Source = renderBitmap;
            
            // Convert the render target bitmap to a Bitmap
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
            Bitmap testMap = new Bitmap(16, 16);
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                Bitmap bitmap = new Bitmap(ms);
                for (int y=0; y<bitmap.Height; y++)
                {
                    for(int x=0; x<bitmap.Width;x++)
                    {
                        {
                           testMap.SetPixel(x,y,bitmap.GetPixel(x,y));
                           colorList.Add(bitmap.GetPixel(x, y));

                        }
                    }
                }

            }
            testMap.Save("myTestMap.png", ImageFormat.Png);
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
            clonnedCanvas.Visibility = Visibility.Hidden;
            return pixelList;
        }
        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(mCanvas);
        }
        private void ShowNetworkResult()
        {
            //RecognizeAndDrawCharacter();
            List<double> pixels = ReadCanvasPixels();
            FileManager fm = new FileManager();
            pixels = fm.NormalizeData(new List<double[]> { pixels.ToArray() }).First().ToList();
            var result = myNetwork.CalculateSmallNetworkResult(pixels.ToArray());
            networkResultText.Text = result.result.ToString();
            string allResultsText = "";
            for(int i=0; i<result.resultList.Count; i++)
            {
                allResultsText += $"[{i}] : {result.resultList.ElementAt(i)}\n";
            }
            AllResultsTB.Text = allResultsText;
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
        private bool PixelsNotEmpty(List<double> pixels)
        {
            foreach(var element in pixels)
            {
                if(element == 1)
                {
                    return true;
                }
            }
            return false;
        }
        private void AddDataBtnClick(object sender, RoutedEventArgs e)
        {
            
            List<double> pixels = ReadCanvasPixels();
            if (pixels != null && pixels.Count == 256 && PixelsNotEmpty(pixels))
            {
                try
                {
                    int expectedResult = Int32.Parse(expectedResultText.Text);
                    inputData.inputData.Add(pixels.ToArray());
                    inputData.trainingResults.Add(expectedResult);
                    SaveTestData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Couldn't add testData");
            }
        }
        private void RecognizeAndDrawCharacter()
        {
            // Usuń wszystkie obecne linie z secondCanvas
            secondCanvas.Children.Clear();

            // Następnie rysuj nowy kształt na secondCanvas
            secondCanvas.Width = 16;
            secondCanvas.Height = 16;
            clonnedCanvas.Height = 16;
            clonnedCanvas.Width = 16;
            // List<System.Windows.Point> characterPoints = GetCharacterPoints(); // Pobierz punkty, które reprezentują kształt

            // Jeśli punkty reprezentują kształt, narysuj go na secondCanvas
            if (points.Count > 0)
            {
                double minX = points[0].Min(p => p.X);
                double maxX = points[0].Max(p => p.X);
                double minY = points[0].Min(p => p.Y);
                double maxY = points[0].Max(p => p.Y);
                // Znajdź minimalne i maksymalne współrzędne punktów, aby obliczyć rozmiar kształtu
                foreach (var point in points)
                {
                    if(point.Min(p => p.X) < minX)
                    {
                        minX = point.Min(p => p.X);
                    }
                    if(point.Max(p => p.X) > maxX)
                    {
                        maxX = point.Max(p => p.X);
                    }
                    if(point.Min(p => p.Y) < minY)
                    {
                        minY = point.Min(p => p.Y);
                    }
                    if (point.Max(p => p.Y) > maxY)
                    {
                        maxY = point.Max(p => p.Y);
                    }
                }
                // Oblicz wymiary kształtu
                double shapeWidth = maxX - minX;
                double shapeHeight = maxY - minY;

                // Przelicz współczynniki skalowania, aby przeskalować kształt do wymiaru 10x7 pikseli
                double scaleX = 14 / shapeWidth;
                double scaleY = 14 / shapeHeight;

                // Wybierz mniejszy współczynnik skalowania, aby zachować proporcje
                double scale = Math.Min(scaleX, scaleY);

                // Oblicz przesunięcie, aby wyśrodkować kształt na kanwie
                double offsetX = (16 - shapeWidth * scale) / 2;
                double offsetY = (16 - shapeHeight * scale) / 2;
                foreach (var point in points)
                {
                    // Narysuj przeskalowany kształt
                    for (int i = 0; i < point.Count - 1; i++)
                    {
                        Line line = new Line();
                        line.Stroke = System.Windows.Media.Brushes.Black;
                        line.StrokeThickness = 4; // Grubość linii może być dostosowana
                        line.X1 = (point[i].X - minX) * scale + offsetX;
                        line.Y1 = (point[i].Y - minY) * scale + offsetY;
                        line.X2 = (point[i + 1].X - minX) * scale + offsetX;
                        line.Y2 = (point[i + 1].Y - minY) * scale + offsetY;

                        secondCanvas.Children.Add(line);

                    }
                }
            }
            secondCanvas.UpdateLayout();
        }
        private void CopyAndScaleCanvasContent()
        {
            int targetWidth = 16;
            int targetHeight = 16;

            // Aktualizacja układu sourceCanvas, aby uzyskać rzeczywiste wymiary
            mCanvas.Measure(new System.Windows.Size(mCanvas.ActualWidth, mCanvas.ActualHeight));
            mCanvas.Arrange(new Rect(new System.Windows.Size(mCanvas.ActualWidth, mCanvas.ActualHeight)));

            // Renderowanie zawartości sourceCanvas jako obrazu
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)mCanvas.ActualWidth, (int)mCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(mCanvas);

            // Tworzenie obrazu ze źródłowego RenderTargetBitmap
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = rtb;
            image.Width = mCanvas.ActualWidth;
            image.Height = mCanvas.ActualHeight;

            // Skalowanie obrazu
            ScaleTransform scaleTransform = new ScaleTransform((double)targetWidth / mCanvas.ActualWidth, (double)targetHeight / mCanvas.ActualHeight);
            image.RenderTransform = scaleTransform;
            image.RenderTransformOrigin = new System.Windows.Point(0, 0);

            // Dodanie przeskalowanego obrazu do targetCanvas
            secondCanvas.Children.Clear(); // Wyczyść docelowy Canvas, jeśli jest taka potrzeba
            secondCanvas.Children.Add(image);

            // Aktualizacja układu targetCanvas
            secondCanvas.Width = targetWidth;
            secondCanvas.Height = targetHeight;
            secondCanvas.UpdateLayout();
        }
        private void ClearFields()
        {
            secondCanvas.Children.Clear();
            clonnedCanvas.Children.Clear();
            mCanvas.Children.Clear();
            points.Clear();
        }
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }

}
