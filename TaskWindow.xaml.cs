using System;
using System.Collections.Generic;
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

namespace SystemProgramming_111
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        Random _random = new();
        public TaskWindow()
        {
            InitializeComponent();
        }
        public CancellationTokenSource cts;
        private async void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            cts = new CancellationTokenSource();
            ConsoleBlock.Text = "";
            progressBar1.Value = 0;
            ButtonStart1.IsEnabled = false;
            for (int i = 0; i < 12; i++)
            {
                // Task.Run(PlusPercent).Wait();
                await PlusPercent(new ThreadData
                {
                    Month = i + 1,
                    Token = cts.Token
                });
            }
            ButtonStart1.IsEnabled = true;
        }
        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            ButtonStart1.IsEnabled = true;

        }

        private double sum;

        private async Task PlusPercent(object? data)
        {
            var threadData = data as ThreadData;
            if (threadData is null) return;
            double val;
            try
            {
                await Task.Delay(_random.Next(250,350));
                threadData!.Token.ThrowIfCancellationRequested();
                double percent = 10 + threadData!.Month;
                double factor = 1 + percent / 100;
                sum *= 1.1;
                val = sum;
                val *= factor;
                sum = val;
                ConsoleBlock.Text += $"Month:{threadData.Month}|Percent:{percent}|Sum:{sum}\n";
                progressBar1.Value += 100.0 / 12;
                // Dispatcher.Invoke(() => ConsoleBlock.Text += $"{sum}\n");
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public class ThreadData  // Комплексный тип данных для передачи в поток
    {
        public int Month { get; set; }

        public CancellationToken Token { get; set; }

    }
}