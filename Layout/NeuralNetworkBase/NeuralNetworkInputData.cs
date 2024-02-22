using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkBase
{
    internal class NeuralNetworkInputData
    {
        public List<double[]> inputData { get; set; }
        public List<int> trainingResults { get; set; }
        public NeuralNetworkInputData(List<double[]> inputData, List<int> trainingResults)
        {
            this.inputData = inputData;
            this.trainingResults = trainingResults;
        }
    }
}
