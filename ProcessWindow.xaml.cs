using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;

namespace SystemProgramming_111
{
    /// <summary>
    /// Логика взаимодействия для ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private Dictionary<string, List<Process>> processDict = new();

        public ProcessWindow()
        {
            InitializeComponent();
        }

        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            ShowProcesses.IsEnabled = false;
            new Thread(UpdateProcesses).Start();
        }

        private void UpdateProcesses()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                List<Process> list;
                if (processDict.ContainsKey(process.ProcessName))
                {
                    list = processDict[process.ProcessName];
                    list.Add(process);
                }
                else
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }
            }
            sw.Stop();

            Dispatcher.Invoke(() => {
            timeElapsed.Content = sw.ElapsedTicks + " tck";
            treeView.Items.Clear();
            foreach (var pair in processDict)
            {
                TreeViewItem node = new() { Header = pair.Key };
                foreach (Process process in pair.Value)
                {
                    TreeViewItem subnode = new() { Header = process.Id };
                    node.Items.Add(subnode);
                }
                treeView.Items.Add(node);
            }

                ShowProcesses.IsEnabled = true;
            });
        }

        private Process notepadProcess;
        private String fullPath;

        private void StartNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (fullPath is not null)
            {
                try
                {
                    notepadProcess = Process.Start(@"D:\Notepad++\notepad++.exe", fullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Incorrect Path",MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                notepadProcess = Process.Start(@"D:\Notepad++\notepad++.exe");
            }

            if (notepadProcess is not null)
            {
                StopNotepad.IsEnabled = true;
                StartNotepad.IsEnabled = false;
            }
        }

        private void StopNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (notepadProcess is not null)
            {
                notepadProcess.Kill();
                
                StopNotepad.IsEnabled = false;
                StartNotepad.IsEnabled = true;
                FileButton.IsEnabled = true;

                notepadProcess = null;
            }
        }

        private Process chromeProcess;

        private String URL;

        private void StartChrome_Click(object sender, RoutedEventArgs e)
        {
            if (URL is not null)
            {
                try
                {
                    chromeProcess = Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "-url " + URL);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Incorrect Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                chromeProcess = Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
            }


            if (chromeProcess is not null)
            {
                StopChrome.IsEnabled = true;
                StartChrome.IsEnabled = false;
            }
        }

        private void StopChrome_Click(object sender, RoutedEventArgs e)
        {
            if (chromeProcess is not null)
            {
                chromeProcess.Kill();

                StopChrome.IsEnabled = false;
                StartChrome.IsEnabled = true;
                URLButton.IsEnabled = true;

                chromeProcess = null;
            }
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            fullPath = PathBox.Text;
            PathBox.Text = "";
            FileButton.IsEnabled = false;
        }

        private void URLButton_Click(object sender, RoutedEventArgs e)
        {
            URL = URLBox.Text;
            URLBox.Text = "";
            URLButton.IsEnabled = false;
        }
    }
}
