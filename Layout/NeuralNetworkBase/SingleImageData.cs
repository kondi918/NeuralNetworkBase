using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkBase
{
    internal class SingleImageData
    {
        public List<double> imageData { get; set; }
        public int result { get; set; }

        public SingleImageData(List<double> imageData, int result)
        {
            this.imageData = imageData;
            this.result = result;
        }
    }
}
