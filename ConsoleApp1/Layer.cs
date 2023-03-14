using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Layer
    {
        public Neuron[] mNeurons;
        public Layer(Neuron[] neurons)
        {
            this.mNeurons = neurons;
        }
    }
}
