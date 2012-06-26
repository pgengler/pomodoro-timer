using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pomodoro
{
    public class PomodoroTimer : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new PomodoroTimer());
        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private TextIcon[] numberIcons;

        private System.Timers.Timer timer;
        private bool running = false;
        private int elapsedTime = 0;

        private const int pomorodoMinutesToWait = 25;
        private const int shortBreakMinutesToWait = 5;
        private const int longBreakMinutesToWait = 15;

        private int minutesToWait;

        public PomodoroTimer()
        {
            InitializeMenu();
            ShowStoppedMenu();

            CreateNumberIcons();
            InitializeIcon();
        }

        private void CreateNumberIcons()
        {
            numberIcons = new TextIcon[25];
            for (int i = 1; i <= 25; i++)
            {
                numberIcons[i - 1] = new TextIcon(i.ToString());
            }
        }

        private void InitializeIcon()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Pomodoro Timer";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void InitializeMenu()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Start Pomodoro", OnStartPomodoro);
            trayMenu.MenuItems.Add("Start short break", OnStartShortBreak);
            trayMenu.MenuItems.Add("Start long break", OnStartLongBreak);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Pause", OnTogglePause);
            trayMenu.MenuItems.Add("Stop", OnStop);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Exit", OnExit);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnStartPomodoro(object sender, EventArgs e)
        {
            StartTimer(pomorodoMinutesToWait);
            ShowRunningMenu();
        }

        private void OnStartShortBreak(object sender, EventArgs e)
        {
            StartTimer(shortBreakMinutesToWait);
            ShowRunningMenu();
        }

        private void OnStartLongBreak(object sender, EventArgs e)
        {
            StartTimer(longBreakMinutesToWait);
            ShowRunningMenu();
        }

        private void OnTogglePause(object sender, EventArgs e)
        {
            ToggleTimerPaused();
        }

        private void OnStop(object sender, EventArgs e)
        {
            StopTimer();
            ShowStoppedMenu();
        }

        private void StartTimer(int minutes)
        {
            elapsedTime = 0;
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // timer fires every 1s
            timer.Elapsed += TimerFired;
            timer.Start();
            running = true;

            minutesToWait = minutes;
            TextIcon(minutes);
        }

        private void ToggleTimerPaused()
        {
            running = !running;
        }

        void TimerFired(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (running)
            {
                // for simplicity assume that the exact amount of the timer's interval has passed
                // in practice, the actual elapsed time will probably be more than the interval
                elapsedTime += (int)timer.Interval;

                if (elapsedTime > (minutesToWait * 60 * 1000))
                {
                    StopTimer();
                    ShowStoppedMenu();
                    MessageBox.Show("Time's up!", "Pomodoro Timer", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    int minutesRemaining = minutesToWait - (elapsedTime / 60 / 1000);
                    TextIcon(minutesRemaining);
                }
            }
        }

        private void StopTimer()
        {
            timer.Stop();
            running = false;
            DefaultIcon();
        }

        private void ShowStoppedMenu()
        {
            // enable 'Start' items; disable 'Pause' and 'Stop'
            trayMenu.MenuItems[0].Enabled = true;
            trayMenu.MenuItems[1].Enabled = true;
            trayMenu.MenuItems[2].Enabled = true;

            trayMenu.MenuItems[4].Enabled = false;
            trayMenu.MenuItems[5].Enabled = false;
        }

        private void ShowRunningMenu()
        {
            // disable 'Start' items; enable 'Pause' and 'Stop'
            trayMenu.MenuItems[0].Enabled = false;
            trayMenu.MenuItems[1].Enabled = false;
            trayMenu.MenuItems[2].Enabled = false;

            trayMenu.MenuItems[4].Enabled = true;
            trayMenu.MenuItems[5].Enabled = true;

        }

        private void OnExit(object sender, EventArgs e)
        {
            if (timer != null) timer.Stop();
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void DefaultIcon()
        {
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
        }

        private void TextIcon(int number)
        {
            trayIcon.Icon = numberIcons[number - 1].get();
        }
    }
}