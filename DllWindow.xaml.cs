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
    public partial class DllWindow : Window
    {
        #region Basics
        /// <summary>
        /// Логика взаимодействия для DllWindow.xaml
        /// </summary>
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

        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern IntPtr CreateThread(
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            ThreadMethod lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId
            );

        public delegate void ThreadMethod();

        public void SayHello()
        {
            Dispatcher.Invoke(() => SayHelloLabel.Content = "Hello");
            sayHelloHandle.Free();
        }
        #endregion

        #region MM Timer  

        delegate void TimerMethod(uint uTimer, uint uMsg, ref uint dwUser, uint dw1, uint dw2);
        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        static extern uint timeSetEvent(
            uint Delay,
            uint uResolution,
            TimerMethod lpTimeProc,
            ref uint dwUser,
            uint eventType
            );

        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        static extern uint timeKillEvent(uint uTimerId);

        const uint TIME_ONESHOT = 0;
        const uint TIME_PERIODIC = 1;

        uint uDelay;
        uint uResolution;
        uint timerId;
        uint dwUser = 0;
        TimerMethod timerMethod = null!;
        GCHandle timerHandle;

        int ticks;
        void TimerTick(uint uTimer, uint uMsg, ref uint dwUser, uint dw1, uint dw2)
        {
            ticks++;
            Dispatcher.Invoke(() => {
                TicksLabel.Content = ticks.ToString();
            });
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 100;       //  Задержка между вызовами 10 ms = 10 Hz
            uResolution = 10;   //  Допустимая погрешность
            ticks = 0;
            timerMethod = new TimerMethod(TimerTick);
            timerHandle = GCHandle.Alloc(timerMethod);
            timerId = timeSetEvent(uDelay, uResolution, timerMethod, ref dwUser, TIME_PERIODIC);
            if (timerId != 0)
            {
                Start.IsEnabled = false;
                Stop.IsEnabled = true;
            }
            else
            {
                timerHandle.Free();
                timerMethod = null!;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            timeKillEvent(timerId);
            timerHandle.Free();
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        #endregion

        public DllWindow()
        {
            InitializeComponent();
        }

        #region Basics methods
        private void MsgA_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero, "Message", "Title", 1);
        }
        private void MsgW_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(IntPtr.Zero, "Message", "Title", 1);

        }
        private void Msg1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero, "Повторить попытку"
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
            SMaker(250, 250);
        }

        private void YellowXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(500, 250);
        }

        private void GreenXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(750, 250);
        }

        private void BlueXylophoneKey_Click(object sender, RoutedEventArgs e)
        {
            SMaker(1000, 250);
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

        GCHandle sayHelloHandle;
        private void Thread_Click(object sender, RoutedEventArgs e)
        {
            // CreateThread(IntPtr.Zero, 0, SayHello, IntPtr.Zero, 0, IntPtr.Zero);
            // Потенциальная проблема - сборщик мусора. При работе он дефрагментирует
            //  память, перенося объекты между поколениями
            // [.][..][.][.x.][..][.] ==> [.][..][.]     [..][.] ==> [.][..][.][..][.]
            //                                                                 эти два
            // объекта поменяют свой адрес в памяти
            // Необходимо "сказать" сборщику мусора о том, что объект не нужно перемещать
            // Для того чтобы не "фиксировать" целое окно, отделим метод в новый объект
            var sayHelloObject = new ThreadMethod(SayHello);
            // и укажем сборщику мусора (GC) разместить этот объект на постоянном месте
            sayHelloHandle = GCHandle.Alloc(sayHelloObject);
            // передаем в неуправляемый код ссылку на объект sayHelloObject
            CreateThread(IntPtr.Zero, 0, sayHelloObject, IntPtr.Zero, 0, IntPtr.Zero);
            // долго удерживать объекты на одном месте нежелательно, после использования
            // нужно их "расфиксировать" - см. SayHello()
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new TimerWindow().Show();  //  По задумке можно сделать несколько Таймеров
        }
    }


}
