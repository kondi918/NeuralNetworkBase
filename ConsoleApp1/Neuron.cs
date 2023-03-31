using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Neuron
    {
        public List<double> weights = new List<double>();       // w0 = bias
        public List<double> inputData = new List<double>();
        public Neuron(double[] weights)
        {
            for(int i=0; i< weights.Length; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public Neuron(List<double> weights)
        {
            for (int i = 0; i < weights.Count; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public void setInputData(List<double> inputData)
        {
            this.inputData.AddRange(inputData);
        }
        public void setInputData(double[] inputData)
        {
            this.inputData.AddRange(inputData);
        }
        public void readWeights()
        {
            Console.WriteLine("Wagi: ");
            for (int i = 0; i < weights.Count; i++)
            {
                Console.Write(weights[i] + ",");
            }
            Console.WriteLine("\n");
        }

    }
}
