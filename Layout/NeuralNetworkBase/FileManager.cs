using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace NeuralNetworkBase
{
    internal class FileManager
    {
        public NeuralNetworkInputData neuralInputData;
        public int numberOfDirectiory = 0;
        public int totalNumberOfDirectories = 0;
        public bool isReadingComplete = false;
        public bool isGettingFilesDone { get; set; } = false;
        [assembly: InternalsVisibleTo("NeuralNetworkUnitTests")]
        public List<double[]> NormalizeData(List<double[]> trainingData)
        {
            try
            {
                double min = trainingData[0].First();
                double max = trainingData[0].First();
                foreach (var element in trainingData)
                {
                    double actualMin = element.Min();
                    double actualMax = element.Max();
                    if (actualMin < min)
                    {
                        min = element.Min();
                    }
                    if (actualMax > max)
                    {
                        max = element.Max();
                    }
                }
                foreach (var element in trainingData)
                {
                    for (int i = 0; i < element.Length; i++)
                    {
                        element[i] = ((2 * (element[i] - min)) / (max - min)) - 1;
                    }
                }
            }
            catch(Exception e)
            {
                throw new ArgumentNullException(e.Message);
            }
            return trainingData;
        }
        private void ReadFromTxtFile(string path, List<double[]> trainingData, List<int> trainingResults)
        {
            List<double> data = new List<double>();
            if (path != null)
            {
                StreamReader sr = new StreamReader(path);
                string line = sr.ReadLine();
                line = sr.ReadLine();
                while (line != null)
                {
                    string[] dataString = line.Split(';');
                    for (int i = 0; i < dataString.Length - 1; i++)
                    {
                        data.Add(double.Parse(dataString[i]));
                    }
                    trainingData.Add(data.ToArray());
                    trainingResults.Add(Int32.Parse(dataString[dataString.Length - 1]));
                    data.Clear();
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }
        private NeuralNetworkInputData ReadFromJSON(string path)
        {
            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            NeuralNetworkInputData inputDataList = JsonConvert.DeserializeObject<NeuralNetworkInputData>(json);
            inputDataList.inputData = NormalizeData(inputDataList.inputData);
            sr.Close();
            return inputDataList;
        }
        private NeuralNetworkInputData MixData(List<SingleImageData> imageData)
        {
            List<double[]> mixedData = new List<double[]>(imageData.Count);
            List<int> mixedResults = new List<int>(imageData.Count);
            Random rnd = new Random();
            while (imageData.Count > 0)
            {
                int randomIndex = rnd.Next(0, imageData.Count);
                mixedData.Add(imageData[randomIndex].imageData.ToArray());
                mixedResults.Add(imageData[randomIndex].result);
                imageData.RemoveAt(randomIndex);
            }
            return new NeuralNetworkInputData(mixedData, mixedResults);
        }
        async Task<List<SingleImageData>> AddImagesToListAsync(string[] files, int resultNumber)
        {
            List<SingleImageData> imageData = new List<SingleImageData>();
            object listLock = new object();

            var tasks = files.Select(async file =>
            {
                List<double> singleData = new List<double>();
                using (Bitmap bitmap = new Bitmap(file))
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            Color color = bitmap.GetPixel(i, j);
                            if (color.A == 255 && color.G == 255 && color.B == 255)
                            {
                                singleData.Add(0);
                            }
                            else
                            {
                                singleData.Add(1);
                            }
                        }
                    }
                }
                lock (listLock)
                {
                    imageData.Add(new SingleImageData(singleData, resultNumber));
                }
            });
            await Task.WhenAll(tasks);
            return imageData;
        }
        private async Task ReadImagesFromDirectories(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            totalNumberOfDirectories = directories.Length;
            List<Task<List<SingleImageData>>> tasks = new List<Task<List<SingleImageData>>>();
            int numberOfFiles = 0;

            foreach (var directory in directories)
            {
                numberOfDirectiory++;
                string[] files = Directory.GetFiles(directory, "*.png");
                tasks.Add(AddImagesToListAsync(files, numberOfFiles));
                numberOfFiles++;
            }

            var results = await Task.WhenAll(tasks);

            // Połącz wyniki z wszystkich katalogów
            List<SingleImageData> allImageData = results.SelectMany(x => x).ToList();

            neuralInputData = MixData(allImageData);
        }

        public  NeuralNetworkInputData GetInputData(string path)
        {
            List<double[]> trainingData = new List<double[]>();
            List<int> trainingResults = new List<int>();
            if (System.IO.Path.GetExtension(path) == ".txt")
            {
                ReadFromTxtFile(path, trainingData, trainingResults);
            }
            else if(System.IO.Path.GetExtension(path) == ".json")
            {
                return ReadFromJSON(path);
            }
            return new NeuralNetworkInputData(NormalizeData(trainingData), trainingResults);
        }
        public async Task GetInputDataFromFiles(string path)
        {
            if (Directory.GetDirectories(path).Length > 0)
            {
                await ReadImagesFromDirectories(path);
                isReadingComplete = true;
            }
        }
        public void AddJSONTrainingData(string path, NeuralNetworkInputData trainingData)
        {
            StreamWriter savingFile = new StreamWriter(path, true);
            string json = JsonConvert.SerializeObject(trainingData, Formatting.Indented);
            savingFile.Write(json);
            savingFile.Close();
        }
    }
}
