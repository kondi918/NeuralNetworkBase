using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralNetworkBase
{
    internal class FileManager
    {
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
        public NeuralNetworkInputData GetInputData(string path)
        {
            List<double[]> trainingData = new List<double[]>();
            List<int> trainingResults = new List<int>();
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
            }
              return new NeuralNetworkInputData(NormalizeData(trainingData), trainingResults);
        }
    }
}
