using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkUnitTests
{
    internal abstract class FileManagerTestData
    {
        public List<double[]> GetTestData()
        {
            double[] inputs1 = { -10, 10, 20 };
            double[] inputs2 = { -1, 1, 2 };
            List<double[]> inputList = new List<double[]>();
            inputList.Add(inputs1);
            inputList.Add(inputs2);
            return inputList;
        }
    }
}
