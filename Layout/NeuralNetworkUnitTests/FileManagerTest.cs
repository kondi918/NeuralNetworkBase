using NeuralNetworkBase;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkUnitTests
{
    internal class FileManagerTest:FileManagerTestData
    {
        [Test]
        public void ShouldReturnNormalizedDataWhenAnyNumbersAreGiven()
        {
            //arrange
            var fileManager = new FileManager();

            //act
            var result = fileManager.NormalizeData(GetTestData());
            List<double> resultNumbers = new List<double>();
            foreach(var element in result)
            {
                foreach(var number in element)
                {
                    resultNumbers.Add(number);
                }
            }

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.AreEqual(-1, resultNumbers.Min());
                Assert.AreEqual(1, resultNumbers.Max());
            });

        }
        [Test]
        public void ShouldReturnEmptyObjectWhenNullDataIsGiven()
        {
            //arrange
            var fileManager = new FileManager();

            //act
            List<double[]> data = new List<double[]>();

            //assert
            Assert.Throws<ArgumentNullException>(() => fileManager.NormalizeData(data));
        }
    }
}
