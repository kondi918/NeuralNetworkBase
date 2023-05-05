using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateNewNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var isWindowClosed = new TaskCompletionSource<object>();
            var CreateNeuralNetworkWindow = new CreateNeuralNetwork();

            CreateNeuralNetworkWindow.Closed += (s, args) =>
            {
                this.Show();
            };

            CreateNeuralNetworkWindow.Show();
            var task = Task.Run(async () => await isWindowClosed.Task);
        }

        private void TrainNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var isWindowClosed = new TaskCompletionSource<object>();
            var TrainNeuralNetworkWindow = new TrainNeuralNetwork();

            TrainNeuralNetworkWindow.Closed += (s, args) =>
            {
                this.Show();
            };

            TrainNeuralNetworkWindow.Show();
            var task = Task.Run(async () => await isWindowClosed.Task);
        }

        private void ExitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
