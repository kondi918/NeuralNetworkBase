using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeuralNetworkBase
{
    /// <summary>
    /// Interaction logic for CreateNeuralNetwork.xaml
    /// </summary>
    public partial class CreateNeuralNetwork : Window
    {
        StreamWriter savingFile = null;
        NeuralNetwork myNetwork = new NeuralNetwork();
        Random random = new Random();
        public CreateNeuralNetwork()
        {
            InitializeComponent();
            Closing += CreateNeuralNetwork_Closing; //JEBANE DELEGATY KUARW
        }

        private void CreateNeuralNetwork_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(savingFile != null)
            {
                savingFile.Close();
            }
        }
        private bool isNumber(string number)
        {
            if(number.All(char.IsDigit))
            {
                return true;
            }
            return false;
        }
        private bool isInLayersRange(int layerNumber)
        {
            if(layerNumber >= 0 && layerNumber  < myNetwork.mLayers.Count)
            {
                return true;
            }
            return false;
        }
        private bool isInNeuronsRange(int layerNumber, int neuronNumber)
        {
            if (neuronNumber >= 0 && neuronNumber < myNetwork.mLayers[layerNumber].mNeurons.Count)
            {
                return true;
            }
            return false;
        }
        private string SelectTxtFile(string typeOfSelectingFile)
        {
            string path = null;
            try
            {
                OpenFileDialog selectingTxtFile = new OpenFileDialog();
                selectingTxtFile.InitialDirectory = Directory.GetCurrentDirectory();
                selectingTxtFile.Filter = "Text files (*.txt)|*.txt";
                selectingTxtFile.Title = typeOfSelectingFile;

                if (selectingTxtFile.ShowDialog() == true)
                {
                    path = selectingTxtFile.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return path;
        }
        private void ChooseTrainingData_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectTxtFile("Plik zapisu do sieci");
            if(path != null)
            {
                savingFile = new StreamWriter(path);
            }
        }
        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            myNetwork.AddLayer();
            LayerCount.Text = myNetwork.mLayers.Count.ToString();
        }

        private void DeleteLayerButton_Click(object sender, RoutedEventArgs e)
        {
            if(isNumber(LayerNumber.Text))
            {
                if(isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    myNetwork.RemoveLayer(Int32.Parse(LayerNumber.Text));
                    LayerCount.Text = myNetwork.mLayers.Count.ToString();
                    NeuronCount.Text = "0";
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer nie jest liczba!");
            }
        }

        private void AddNeuronButton_Click(object sender, RoutedEventArgs e)
        {

            if (isNumber(LayerNumber.Text))
            {
                if (isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    myNetwork.AddNeuron(Int32.Parse(LayerNumber.Text));
                    NeuronCount.Text = myNetwork.mLayers[Int32.Parse(LayerNumber.Text)].mNeurons.Count.ToString();
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer nie jest liczba!");
            }
        }

        private void DeleteNeuronButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNumber(LayerNumber.Text) && isNumber(NeuronNumber.Text))
            {
                if (isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    if(isInNeuronsRange(Int32.Parse(LayerNumber.Text),Int32.Parse(NeuronNumber.Text)))
                    {
                        myNetwork.RemoveNeuron(Int32.Parse(LayerNumber.Text), Int32.Parse(NeuronNumber.Text));
                        NeuronCount.Text = myNetwork.mLayers[Int32.Parse(LayerNumber.Text)].mNeurons.Count.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Wybrany Neuron znajduje się poza zakresem Layerów sieci");
                    }
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer lub Neuron nie jest liczba!");
            }
        }
        private double[] GenerateRandomWeights()
        {
            List<double> weights = new List<double>();
      
            for (int i = 0; i < Int32.Parse(WeightNumber.Text); i++)
            {
                weights.Add(Math.Round(2 * random.NextDouble() - 1, 5));
            }
            return weights.ToArray();
        }
        private bool isCorrectWeightAmount()
        {
            int weightCount = 0;
            if (isNumber(WeightNumber.Text))
            {
                weightCount = Int32.Parse(WeightNumber.Text);
            }
            else
            {
                MessageBox.Show("Podana ilosc wag nie jest liczba!");
                return false;
            }
            if(weightCount >= 2)
            {
                if (Int32.Parse(LayerNumber.Text) > 0 && weightCount == myNetwork.mLayers[Int32.Parse(LayerNumber.Text) - 1].mNeurons.Count + 1 || Int32.Parse(LayerNumber.Text) == 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Ilosc wag w danym Neuronie musi być o 1 wieksza niz ilosc neuronow w poprzednim layerze (uwzgledniajac, ze waga nr. 0 to BIAS) ");
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Podana ilosc wag musi być wieksza od 1. Pamietaj, że pierwsza waga to liczba BIAS");
                return false;
            }
        }
        private void AddNeuronWeightButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (isNumber(LayerNumber.Text) && isNumber(NeuronNumber.Text))
            {
                if (isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    if (isInNeuronsRange(Int32.Parse(LayerNumber.Text), Int32.Parse(NeuronNumber.Text)))
                    {
                        double[] weights = GenerateRandomWeights();
                        if(weights.Length > 1)
                        {
                            myNetwork.SetNeuronWeights(Int32.Parse(LayerNumber.Text), Int32.Parse(NeuronNumber.Text), weights);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wybrany Neuron znajduje się poza zakresem Layerów sieci");
                    }
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer lub Neuron nie jest liczba!");
            }
            */
            if(isNumber(LayerNumber.Text))
            {
                if (isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    if (isCorrectWeightAmount())
                    {
                        foreach (var neuron in myNetwork.mLayers[Int32.Parse(LayerNumber.Text)].mNeurons)
                        {
                           neuron.setWeights(GenerateRandomWeights().ToList());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer nie jest liczba!");
            }
        }

        private void DeleteNeuronWeightButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNumber(LayerNumber.Text) && isNumber(NeuronNumber.Text))
            {
                if (isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    if (isInNeuronsRange(Int32.Parse(LayerNumber.Text), Int32.Parse(NeuronNumber.Text)))
                    {
                        myNetwork.RemoveWeightsFromNeuron(Int32.Parse(LayerNumber.Text), Int32.Parse(NeuronNumber.Text));
                    }
                    else
                    {
                        MessageBox.Show("Wybrany Neuron znajduje się poza zakresem Layerów sieci");
                    }
                }
                else
                {
                    MessageBox.Show("Wybrany Layer znajduje się poza zakresem Layerów sieci");
                }
            }
            else
            {
                MessageBox.Show("Wybrany Layer lub Neuron nie jest liczba!");
            }
        }

        private void SaveNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            if(savingFile !=null)
            {
                savingFile.Write(myNetwork.GetWholeStructure());
            }
            else
            {
                MessageBox.Show("Najpierw musisz wybrac sciezke do pliku w ktorym siec ma zostac zapisana!");
            }
        }
        private void LayerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(isNumber(LayerNumber.Text) && LayerNumber.Text.Length > 0)
            {
                if(isInLayersRange(Int32.Parse(LayerNumber.Text)))
                {
                    NeuronCount.Text = myNetwork.mLayers[Int32.Parse(LayerNumber.Text)].mNeurons.Count.ToString();
                }
            }
        }
        private void NeuralNetworkDraw()
        {
            if (myNetwork.mLayers.Count > 0)
            {
                bool isNetworkEmpty = false;
                foreach (var layer in myNetwork.mLayers)
                {
                    if (layer.mNeurons.Count == 0)
                    {
                        isNetworkEmpty = true;
                    }
                }
                if (!isNetworkEmpty)
                {
                    NeuralNetworkDrawer.GenerateSchemaStructure(myNetwork);
                    NeuralNetworkDrawer.CreateNeuronsDrawings(NeuralNetworkStructure);
                    NeuralNetworkDrawer.DrawNeuralNetworkSchema(NeuralNetworkStructure);
                }
                else
                {
                    MessageBox.Show("Nie mozna narysowac sieci z pustymi layerami");
                }
            }
            else
            {
                MessageBox.Show("Aby moc narysowac siec najpierw musisz ja stworzyc");
            }
        }

        private void NeuralNetworkCheckout_Click(object sender, RoutedEventArgs e)
        {
            NeuralNetworkDraw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(myNetwork.mLayers.Count > 0)
            {
                NeuralNetworkDraw();
            }
        }
    }
}

