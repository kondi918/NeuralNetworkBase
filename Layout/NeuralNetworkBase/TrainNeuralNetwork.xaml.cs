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
        StreamWriter logFile = null;    // Tworzymy plik do logowania
        StreamWriter savingFile = null;   // Tworzymy plik do zapisu
        NeuralNetwork myNetwork = null;             //NULL BO WYBIERAM SAM PLIK Z SIECIĄ POCZĄTKOWĄ  // new NeuralNetwork("plikiTekstowe/dlugopisObraczka/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
        List<double[]> trainingData = new List<double[]>();
        List<int> trainingResults = new List<int>();
        bool isTrainingCompleted = true;
        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private readonly object mLayersLock = new object();
        public TrainNeuralNetwork()
        {
            InitializeComponent();
            Closing += TrainNeuralNetwork_Closing;
        }
        private void TrainNeuralNetwork_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (logFile != null)
            {
                logFile.Close();
            }
            if(savingFile != null)
            {
                savingFile.Close();
            }
        }
        void GetTrainingData(string path)
        {
            List<double> data = new List<double>();
            if (path != null)
            {
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
            catch(Exception ex){
                MessageBox.Show(ex.ToString());
            }
            return path;
        }


        private void ChooseTrainingData_Click(object sender, RoutedEventArgs e)  
        {
            GetTrainingData(SelectTxtFile("Dane treningowe"));
        }

        private void SelectTrainedNeuralNetwork_Click(object sender, RoutedEventArgs e)
        {
            Task selectmyNetworkFile = Task.Run(() => myNetwork = new NeuralNetwork(SelectTxtFile("Plik z siecią początkową")));
            selectmyNetworkFile.ContinueWith(task => {
                NeuralNetworkStructure.Dispatcher.Invoke(() => DrawNeuralNetwork());
            });
        }

        private void SelectFileToSaveTrainedNeuralNetwork_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectTxtFile("Plik zapisu do sieci");
            if (path != null)
            {
                savingFile = new StreamWriter(path);
            }
        }

        private void DrawNeuralNetwork()
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
                    MessageBox.Show("Nie mozna wczytac sieci. Upewnij sie czy plik ktory wybrales zawiera poprawna strukture");
                }
            }
            else
            {
                MessageBox.Show("Nie mozna wczytac sieci. Upewnij sie czy plik ktory wybrales zawiera poprawna strukture");
            }
        }

        // Wstrzymaj zadanie
        private void Pause()
        {
            pauseEvent.Reset();
            StopBtn.Content = "Wznów";
        }
        private void Resume()
        {
            pauseEvent.Set();
            StopBtn.Content = "Stop";
        }
        void Timer(int seconds)
        {
            int mSeconds = 0;
            while (mSeconds != seconds && !cancelTokenTraining.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                mSeconds++;
                TrainingProgressBar.Dispatcher.Invoke(() =>
                {
                    TrainingProgressBar.Value = (double)mSeconds / (double)seconds * 100;
                });
                pauseEvent.Wait();
            }
        }
        private bool isSigmoidChecked()
        {
            bool isSigmoidChecked = false;
            if (Sigmoid.IsChecked == true)
            {
                isSigmoidChecked = true;
            }
            return isSigmoidChecked;
        }

        private bool isReluChecked()
        {
            bool isReluChecked = false;
            if (Relu.IsChecked == true)
            {
                isReluChecked = true;
            }
            return isReluChecked;
        }

        private void SetActivationFunction()
        {
            isSigmoidChecked();
            if (isSigmoidChecked())
            {
                myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.sigmoid;
            }
            else if (isReluChecked())
            {
                myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.relu;
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
            int seconds = 5;                                                 // TUTAJ DODAC POBIERANIE OKRESLONEGO CZASU Z OKIENKA
            ExpectedTrainingTime.Dispatcher.Invoke(() =>
            {
                 int.TryParse(ExpectedTrainingTime.Text, out seconds);
            });
            Task setTimer = Task.Run(() => Timer(seconds));
            int mistakes = 1;
            Task ShowErrors = null;
            while (mistakes != 0 && !cancelTokenTraining.IsCancellationRequested)
            {
                pauseEvent.Wait();
                lock (mLayersLock)
                {
                    mistakes = Training();
                }
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
            isTrainingCompleted = true;
        }

        private void setSettings()
        {
            isTrainingCompleted = false;
            Resume();
            cancelTokenTraining = new CancellationTokenSource();
            InformationStatus.Text = "Rozpoczęto Nauczanie";
            SetActivationFunction();
            TrainingProgressBar.Value = 0;
        }
        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (myNetwork != null)
            {
                 if (trainingData.Count > 0 && myNetwork.mLayers[0].mNeurons[0].weights.Count-1 == trainingData[0].Length)
                 {
                    Task trainingNetworkTask = new Task(() =>
                        {
                            TrainNetwork();
                        });
                    if(isTrainingCompleted)
                    {
                        setSettings();
                        try
                        {
                            trainingNetworkTask.Start();
                        }
                        catch(AggregateException ae)
                        {
                            foreach(var element in ae.InnerExceptions)
                            {
                                MessageBox.Show(element.ToString());
                            }
                        }
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
            Resume();
        }
        private void StopResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if(pauseEvent.IsSet)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lock (mLayersLock)
                {
                    if (savingFile != null)
                    {
                        savingFile.Write(myNetwork.GetWholeStructure());
                    }
                    else
                    {
                        MessageBox.Show("Aby moc zapisac siec musisz najpierw wybrac plik w ktorym ma zostac zapisana");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (myNetwork != null)
            {
                DrawNeuralNetwork();
            }
        }
    }
}

//myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.relu;