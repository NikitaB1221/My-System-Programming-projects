using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using static SystemProgramming_111.MultiWindow;

namespace SystemProgramming_111
{
    /// <summary>
    /// Логика взаимодействия для DllWindow.xaml
    /// </summary>
    public partial class DllWindow : Window
    {
        [DllImport("User32.dll")]
        public static extern
            int MessageBoxA(
            IntPtr hWnd,                       // HWND
            String lpText,                     // LPCSTR
            String lpCaption,                  // LPCSTR
            uint uType                         // UINT
            );
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern
            int MessageBoxW(
            IntPtr hWnd,                       // HWND
            String lpText,                     // LPCSTR
            String lpCaption,                  // LPCSTR
            uint uType                         // UINT
            );
        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern bool Sound(uint dwFreq, uint dwDuration);
        public DllWindow()
        {
            InitializeComponent();
        }
        private void MsgA_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero,"Message","Title",1);
        }
        private void MsgW_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(IntPtr.Zero,"Message","Title",1);

        }
        private void Msg1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero,"Повторить попытку"
                , "Соединение не установленно", 0x35);
        }

        private void MsgError_Click(object sender, RoutedEventArgs e)
        {
            ErrorAlert("Ошибка!");
        }

        private void ConfirmError_Click(object sender, RoutedEventArgs e)
        {
            ConfirmMessage("Процесс занимает много времени!");
        }


        private bool? ConfirmMessage(String message)
        {
            int res = MessageBoxW(IntPtr.Zero, message, "", 0x46);
                return res switch
                {
                    11 => true,   //Continue
                    10 => false,  //Try again
                    _ => null     //Cancel (res = 2)
                };

        }

        private bool Ask(String message)
        {
            int res = MessageBoxW(IntPtr.Zero, message, "", 0x24);
            return res switch
            {
                6 => true,   //Continue
                7 => false,  //Try again
            };
        }



        private void ErrorAlert(String message)
        {
            MessageBoxW(IntPtr.Zero, message, null!, 0x10);
        }

        private void Ask_Click(object sender, RoutedEventArgs e)
        {
            if (Ask("Подтвердить?"))
            {
                MessageBoxW(IntPtr.Zero, "Действие подтвержденно", "", 0x30);
            }
            else
            {
                MessageBoxW(IntPtr.Zero, "Действие отменнено", "", 0x10);
            }
        }

        private void Beep_Click(object sender, RoutedEventArgs e)
        {
            Sound(250, 450);
        }

        private readonly object monitor = new();

        private void RedXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(250 , 250);
        }

        private void YellowXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(500 , 250);
        }

        private void GreenXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(750 , 250);
        }

        private void BlueXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(1000 , 250);
        }


        private void SMaker(uint Hertz, uint Msec)
        {
            try
            {
                Monitor.Enter(monitor);
                Task.Delay(10);
                Dispatcher.Invoke(() => Sound(Hertz, Msec));

            }
            finally
            {
                Monitor.Exit(monitor);  // Выход == разблокирование
            }
        }
    }
}
