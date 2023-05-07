using NeuralNetworkBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkBase
{
    internal class Layer
    {
        public List<Neuron> mNeurons = new List<Neuron>();
        public void AddNeuron(Neuron neuron)
        {
            mNeurons.Add(neuron);
        }
        public void RemoveNeuron(int neuronNumber)
        {
            mNeurons.RemoveAt(neuronNumber);
        }
        public Layer(Neuron[] neurons)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                this.mNeurons.Add(neurons[i]);
            }
        }
        public Layer(List<Neuron> neurons)
        {
            for (int i = 0; i < neurons.Count; i++)
            {
                this.mNeurons.Add(neurons[i]);
            }
        }
        public Layer()
        {

        }
    }
}
