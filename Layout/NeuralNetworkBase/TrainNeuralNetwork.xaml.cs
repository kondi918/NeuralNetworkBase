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
        CancellationTokenSource cancelTokenTraining = new CancellationTokenSource();                           //TU ZMIANA NA NULLE UWAGA KURWA TU ZMIANA NA NULLE
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
            while (seconds != 0 && !cancelTokenTraining.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                seconds--;
            }
        }
        
        private int Training()
        {
            int mistakes = 0;
            for (int i = 0; i < trainingData.Count; i++)
            {
                if (!myNetwork.NetworkTraining(trainingData[i], trainingResults[i], 0.1))
                {
                    mistakes++;
                }
            }
            return mistakes;
        }
        private void SetTextOnTextBlock(TextBlock textBlock, string text)
        {
            textBlock.Dispatcher.Invoke(() =>
            {
                textBlock.Text = text;
            });
        }
        private void TrainNetwork()
        {
            int seconds = 5;                                                                // TUTAJ DODAC POBIERANIE OKRESLONEGO CZASU Z OKIENKA
            Task setTimer = Task.Run(() => Timer(seconds));
            int mistakes = 1;
            Task ShowErrors = null;
            while (mistakes != 0 || cancelTokenTraining.IsCancellationRequested)
            {
                mistakes = Training();
                if (ShowErrors == null || ShowErrors.IsCompleted)
                {
                    ShowErrors = Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                    });
                    SetTextOnTextBlock(MistakesNumber, mistakes.ToString());
                }
            }
            ShowErrors.Wait();
            SetTextOnTextBlock(MistakesNumber, mistakes.ToString());
            if (!cancelTokenTraining.IsCancellationRequested)
            {
                setTimer.Wait();
            }
            if(cancelTokenTraining.IsCancellationRequested)
            {
                SetTextOnTextBlock(InformationStatus, "Przerwano nauczanie");
            }
            else
            {
                SetTextOnTextBlock(InformationStatus, "Zakończono nauczanie");
            }
            cancelTokenTraining = new CancellationTokenSource();
            isTrainingCompleted = true;
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenTraining.Cancel();
        }
    }
}


// UWAHA KURWA UWAGA KURWA POWTARZAM UWAGA TUTAJ ZMIANY DO ZROBIENIA
// 1. Zmienic w layoucie wielkosc tej jednej czcionki i wycentrowac calosc tak jak bylo na poczatku             //K
// 2. Dodac sekundy i zczytywac do zmiennej
// 3. Dodac checkboxa obsluge z wyborem funkcji aktywacji (pamietaj ENUM jest kurwa ENUM ENUM)
// 4. Jak wystarczy czasu to dodac funkcje z progress barem ktora sprawi ze sie bedzie zapelnial
// 4.1 Jak wystarczy czasu to dodac zczytywanie plikow z menu i zmienic wtedy na NULL kurwa NULL te na poczatku (patrz jebitny komentarz)