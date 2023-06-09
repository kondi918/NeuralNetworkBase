﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Layer
    {
        public List<Neuron> mNeurons = new List<Neuron>();
        public void addNeuron(Neuron neuron)
        {
            mNeurons.Add(neuron);
        }
        public void removeNeuron(int neuronNumber)
        {
            mNeurons.RemoveAt(neuronNumber);
        }
        public Layer(Neuron[] neurons)
        {
            for(int i=0; i<neurons.Length; i++)
            {
                this.mNeurons.Add(neurons[i]);
            }
        }
        public Layer(List<Neuron> neurons)
        {
            for(int i =0; i< neurons.Count; i++)
            {
                this.mNeurons.Add(neurons[i]);
            }
        }
        public Layer()
        {

        }
    }
}
