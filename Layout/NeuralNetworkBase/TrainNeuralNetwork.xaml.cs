using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for TrainNeuralNetwork.xaml
    /// </summary>
    public partial class TrainNeuralNetwork : Window
    {
        StreamWriter logFile = new StreamWriter("plikiTekstowe/rozpoznawanieZdan/logiNauczania.txt", true);    // Tworzymy plik do logowania
        NeuralNetwork myNetwork = null;   //pobieram dane sieci z pliku
        List<double[]> trainingData = new List<double[]>();
        List<int> trainingResults = new List<int>();

        public TrainNeuralNetwork()
        {
            InitializeComponent();
        }

        private void ChooseTrainingData_Click(object sender, RoutedEventArgs e)
        {
            // your code here

            // Create an instance of the OpenFileDialog class
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set the title and filter for the dialog
            openFileDialog1.Title = "Select a file";
            openFileDialog1.Filter = "All files (*.*)|*.*";

            // Show the dialog and get the result
            bool? result = openFileDialog1.ShowDialog();

            // If the user clicks OK, get the selected file name and do something with it
            if (result == true)
            {
                string filePath = openFileDialog1.FileName;
                myNetwork = new NeuralNetwork(filePath);
                // do something with the file path
            }
        }

            private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (myNetwork != null)
            {
                int bledy = 100;
                while (bledy != 0)
                {
                    bledy = 0;
                    for (int i = 0; i < trainingData.Count; i++)
                    {
                        if (!myNetwork.NetworkTraining(trainingData[i], trainingResults[i], 0.1))
                        {
                            bledy++;
                        }
                    }
                    MistakesNumber.FontSize = 72;
                    MistakesNumber.Text = bledy.ToString();
                }
            }
            else
            {
                MistakesNumber.FontSize = 15;
                MistakesNumber.Text = "You didn't selected any training data";
            }
        }
    }
}
