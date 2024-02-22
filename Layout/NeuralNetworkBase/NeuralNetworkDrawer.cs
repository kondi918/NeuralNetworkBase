using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralNetworkBase
{
    class SingleNeuronDrawing
    {
        public int positionx { get; private set; }
        public int positiony { get; private set; }
        public int layerNumber { get; private set; }

        public SingleNeuronDrawing(int positionx, int positiony, int layerNumber)
        {
            this.positionx = positionx;
            this.positiony = positiony;
            this.layerNumber = layerNumber;
        }
    }

    static class NeuralNetworkDrawer
    {
        private static List<int> NeuralNetworkSchemaStructure = new List<int>();
        private static List<SingleNeuronDrawing> NeuronsPositionOnSchema = new List<SingleNeuronDrawing>();

        public static void GenerateSchemaStructure(NeuralNetwork myNeuralNetwork)
        {
            foreach (var element in myNeuralNetwork.mLayers)
            {
                NeuralNetworkSchemaStructure.Add(0);
                foreach (var neuron in element.mNeurons)
                {
                    NeuralNetworkSchemaStructure[NeuralNetworkSchemaStructure.Count - 1]++;
                    foreach (var weight in neuron.getWeights())
                    {

                        //result += weight + ";";
                    }
                }
                //result += "</Layer>\n";
            }
        }

        public static void CreateNeuronsDrawings(Canvas myCanvas)
        {
            int canvasWidth = (int)myCanvas.ActualWidth;
            int canvasHeight = (int)myCanvas.ActualHeight;
            int operationWidth = (int)((double)canvasWidth * 0.8);
            int operationHeight = (int)((double)canvasHeight * 0.8);
            int differenceBetweenLayers = operationWidth / NeuralNetworkSchemaStructure.Count;
            int NeuronsWidthPosition = differenceBetweenLayers / 2;
            int numberOfLayer = 0;

            foreach (var numberOfNeuronsInLayer in NeuralNetworkSchemaStructure)
            {
                int differenceBetweenNeurons = operationHeight / numberOfNeuronsInLayer;
                int NeuronsHeightPosition = differenceBetweenNeurons / 2;
                for (int i = 0; i < numberOfNeuronsInLayer; i++)
                {
                    SingleNeuronDrawing singleNeuron = new SingleNeuronDrawing(NeuronsWidthPosition, NeuronsHeightPosition, numberOfLayer);
                    NeuronsPositionOnSchema.Add(singleNeuron);
                    NeuronsHeightPosition += differenceBetweenNeurons;
                }
                NeuronsWidthPosition += differenceBetweenLayers;
                numberOfLayer++;
            }
        }

        public static void DrawNeuralNetworkSchemaConnections(Canvas myCanvas)
        {
            foreach (var neuronA in NeuronsPositionOnSchema)
            {
                foreach (var neuronB in NeuronsPositionOnSchema)
                {
                    if (neuronA.layerNumber == neuronB.layerNumber + 1)
                    {
                        Line connection = new Line();
                        connection.Stroke = Brushes.Red;
                        connection.X1 = neuronA.positionx + 10;
                        connection.Y1 = neuronA.positiony + 10;
                        connection.X2 = neuronB.positionx + 10;
                        connection.Y2 = neuronB.positiony + 10;
                        myCanvas.Children.Add(connection);
                    }
                }

            }
        }


        public static void DrawNeuralNetworkSchema(Canvas myCanvas)
        {
            myCanvas.Children.Clear();
            foreach (var neuron in NeuronsPositionOnSchema)
            {
                Ellipse circle = new Ellipse();
                circle.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // ARGB color code for red color
                circle.Width = 20;
                circle.Height = 20;
                Canvas.SetLeft(circle, neuron.positionx); // position the circle at (50, 50) on the canvas
                Canvas.SetTop(circle, neuron.positiony);

                myCanvas.Children.Add(circle); // add the circle as a child of the canvas
            }
            DrawNeuralNetworkSchemaConnections(myCanvas);
            NeuralNetworkSchemaStructure.Clear();
            NeuronsPositionOnSchema.Clear();
        }
    }
}
