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
        CancellationTokenSource cancelTokenTraining = new CancellationTokenSource();
        StreamWriter logFile = new StreamWriter("plikiTekstowe/dlugopisObraczka/logiNauczania.txt", true);    // Tworzymy plik do logowania
        NeuralNetwork myNetwork = new NeuralNetwork("plikiTekstowe/dlugopisObraczka/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
        List<double[]> trainingData = new List<double[]>();
        List<int> trainingResults = new List<int>();
        bool isTrainingCompleted = true;

        public TrainNeuralNetwork()
        {
            InitializeComponent();
        }

        void GetTrainingData(string path)
        {
            List<double> data = new List<double>();
            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();
            line = sr.ReadLine();
            while (line != null)
            {
                string[] dataString = line.Split(';');
                for (int i = 0; i < dataString.Length - 1; i++)
                {
                    data.Add(double.Parse(dataString[i]));
                }
                trainingData.Add(data.ToArray());
                trainingResults.Add(Int32.Parse(dataString[dataString.Length - 1]));
                data.Clear();
                line = sr.ReadLine();
            }
            sr.Close();
        }
        private void ChooseTrainingData_Click(object sender, RoutedEventArgs e)     // TU DO POPRAWY UWAGA KURWA TU DO POPRAWY
        {
            GetTrainingData("plikiTekstowe/dlugopisObraczka/daneNauczania.txt");
            /*
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a file";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            bool? result = openFileDialog1.ShowDialog();
            if (result == true)
            {
                string filePath = openFileDialog1.FileName;
                myNetwork = new NeuralNetwork(filePath);
            }
            */
        }
        void Timer(int seconds)
        {
            while (seconds != 0)
            {
                Thread.Sleep(1000);
                seconds--;
            }
        }
  
        private void TrainNetwork()
        {
            int seconds = 5; // TUTAJ DODAC POBIERANIE OKRESLONEGO CZASU Z OKIENKA
            Task setTimer = Task.Run(() => Timer(seconds));
            int mistakes = 1;
            Task ShowErrors = null;
            while (mistakes != 0 || cancelTokenTraining.IsCancellationRequested)
            {
                mistakes = 0;
                for (int i = 0; i < trainingData.Count; i++)
                {
                    if (!myNetwork.NetworkTraining(trainingData[i], trainingResults[i], 0.1))
                    {
                        mistakes++;
                    }
                }
                if (ShowErrors == null || ShowErrors.IsCompleted)
                {
                    ShowErrors = Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                    });
                    MistakesNumber.Dispatcher.Invoke(() =>
                    {
                        MistakesNumber.Text = "Ilosc bledow " + mistakes;
                    });
                }
            }
            ShowErrors.Wait();
            MistakesNumber.Dispatcher.Invoke(() =>
            {
                MistakesNumber.Text = "Ilosc bledow " + mistakes;
            });
            if (!cancelTokenTraining.IsCancellationRequested)
            {
                setTimer.Wait();
                InformationStatus.Dispatcher.Invoke(() =>
                {
                    InformationStatus.Text = "Zakończono nauczanie";
                });
            }
            else if(cancelTokenTraining.IsCancellationRequested)
            {
                InformationStatus.Dispatcher.Invoke(() =>
                {
                    InformationStatus.Text = "Przerwano nauczanie";
                });
            }
            isTrainingCompleted = true;
            cancelTokenTraining = new CancellationTokenSource();
        }
        private async void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (myNetwork != null)
            {
                 if (trainingData.Count > 0 && myNetwork.mLayers[0].mNeurons[0].weights.Count-1 == trainingData[0].Length)
                 {
                    InformationStatus.Text = "Rozpoczęto Nauczanie";
                    Task trainingNetworkTask = new Task(() =>
                        {
                            TrainNetwork();
                        });
                    if(isTrainingCompleted)
                    {
                        isTrainingCompleted = false;
                        trainingNetworkTask.Start();
                    }
                    else
                    {
                        MessageBox.Show("Aby rozpoczac trenowanie sieci musisz poczekać aż aktualne trenowanie zostanie zakończone. Możesz również anulować aktualny trening");
                    }
                 }
                 else
                 {
                    MessageBox.Show("Dane nauczania nie zostały wybrane lub nie są zgodne ze strukturą sieci");
                 }
            }
            else
            {
                MessageBox.Show("Najpierw musisz wybrać odpowiednie pliki z danymi nauczania i strukturą sieci");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
