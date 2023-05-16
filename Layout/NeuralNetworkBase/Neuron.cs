using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkBase
{
    internal class Neuron
    {
        public List<double> weights = new List<double>();       // w0 = bias
        public List<double> inputData = new List<double>();
        public double neuronResult { get; set; }
        public Neuron(double[] weights)
        {
            this.weights.Clear();
            for (int i = 0; i < weights.Length; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public Neuron(List<double> weights)
        {
            this.weights.Clear();
            for (int i = 0; i < weights.Count; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public Neuron()
        {
            
        }
        public void setInputData(List<double> inputData)
        {
            this.inputData.Clear();
            this.inputData.AddRange(inputData);
        }
        public void setInputData(double[] inputData)
        {
            this.inputData.Clear();
            this.inputData.AddRange(inputData);
        }
        public void setWeights(List <double> weights)
        {
            this.weights.Clear();
            foreach(var weight in weights)
            {
                this.weights.Add(weight);
            }
        }
        public void RemoveWeights()
        {
            this.weights.Clear();
        }
    }
}
