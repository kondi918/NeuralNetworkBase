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
            for (int i = 0; i < weights.Length; i++)
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
        public void setWeights(double[] weights)
        {
            this.weights.Clear();
            this.weights.AddRange(weights);
        }
        public void RemoveWeights()
        {
            this.weights.Clear();
        }
    }
}
