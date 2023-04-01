using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// Логика взаимодействия для TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
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

        uint HH = 0;
        uint MM = 0;
        uint SS = 0;
        uint DS = 0;

        void TimerTick(uint uTimer, uint uMsg, ref uint dwUser, uint dw1, uint dw2)
        {
            Dispatcher.Invoke(() =>
            {
                DecesecondLabel.Content = (DS <= 9 ? "0" + DS : DS);
                SecondLabel.Content = (SS <= 9 ? "0" + SS : SS);
                MinuteLabel.Content = (MM <= 9 ? "0" + MM : MM);
                HoursLabel.Content = (HH <= 9 ? "0" + HH : HH);
            });
            DS++;
            if (DS == 10)
            {
                DS = 0;
                SS++;
            }
            if (SS == 60)
            {
                SS = 0;
                MM++;
            }
            if (MM == 60)
            {
                MM = 0;
                HH++;
            }
        }

        #endregion

        public TimerWindow()
        {
            InitializeComponent();
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 100;      
            uResolution = 0;   
            timerMethod = new TimerMethod(TimerTick);
            timerHandle = GCHandle.Alloc(timerMethod);
            timerId = timeSetEvent(uDelay, uResolution, timerMethod, ref dwUser, TIME_PERIODIC);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timeKillEvent(timerId);
            timerHandle.Free();
            timerMethod = null!;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (timerHandle.IsAllocated)
            {
                timeKillEvent(timerId);
                timerHandle.Free();
                timerMethod = null!;
            }
            HH = 0;
            MM = 0;
            SS = 0;
            DS = 0;
            Dispatcher.Invoke(() =>
            {
                DecesecondLabel.Content = "00";
                SecondLabel.Content = "00";
                MinuteLabel.Content = "00";
                HoursLabel.Content = "00";
            });
        }
    }
}
