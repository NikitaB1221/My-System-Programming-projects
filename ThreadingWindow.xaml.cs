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
    public partial class ThreadingWindow : Window
    {
        public ThreadingWindow()
        {
            InitializeComponent();
        }
        #region part 1 - "зависание интерфейса"
        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            Start1();
        }

        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Start1()
        {
            for (int i = 0; i < 10; i++)
            {
                progressBar1.Value = (i + 1) * 10;
                ConsoleBlock.Text += i.ToString() + "\n";
                Thread.Sleep(300);
            }
        }
        #endregion

        #region part 2 - Исключение (обращение к другому потоку)
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            new Thread(Start1).Start();
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region part 3 - Решение
        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = false;
            new Thread(Start3).Start();
        }

        private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = true;
        }
        private bool isStopped;
        private void Start3()
        {
            for (int i = 0; i < 10 && !isStopped; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    progressBar3.Value = (i + 1) * 10;
                    ConsoleBlock.Text += i.ToString() + "\n";
                });

                Thread.Sleep(300);
            }
        }
        #endregion

        #region part 4 - Взаимодействие потоков (продолжение прогресса)
        private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = false;
            ButtonStart4.IsEnabled = false;

            new Thread(Start4).Start(savedIndex4);
            if (savedIndex4 == 0)
            {
                ConsoleBlock.Text = "";
            }
        }
        private void ButtonStop4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = true;
        }
        private bool isStopped4;
        private int savedIndex4; 
        private void Start4(object? startIndex)
        {
            if (startIndex is int startFrom)
            {
                for (int i = startFrom; i < 10; i++)
                {
                    if (isStopped4)
                    {
                        savedIndex4 = i;
                        this.Dispatcher.Invoke(() =>
                        {
                            ButtonStart4.IsEnabled = true;
                        });
                        return;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        progressBar4.Value = (i + 1) * 10;
                        ConsoleBlock.Text += i.ToString() + "\n";
                    });

                    Thread.Sleep(300);
                }
                savedIndex4 = 0;
                this.Dispatcher.Invoke(() =>
                {
                    ButtonStart4.IsEnabled = true;
                });
            }
        }
        #endregion
    }
}