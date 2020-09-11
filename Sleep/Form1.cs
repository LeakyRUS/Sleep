using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Sleep
{
    public partial class SleepSetForm : Form
    {
        private System.Threading.Timer hibernateTimer;
        private System.Threading.Timer prepareTimer;

        private TimeSpan timerSpan;

        public SleepSetForm()
        {
            InitializeComponent();
        }

        public void SleepSetForm_Load(object state, EventArgs e)
        {

        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            string timeText = this.maskedTextBox1.Text;

            if (TimeSpan.TryParseExact(timeText, "g", System.Globalization.CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                timerSpan = timeSpan;
                label2.Text = $"Компьютер будет переведен в спящий режим {DateTime.Now + timerSpan:G}";
            }
            else
            {
                timerSpan = TimeSpan.MaxValue;
                label2.Text = "Неверный формат времени";
            }
        }

        private void OnHibernate(object sender)
        {
            bool can = Application.SetSuspendState(PowerState.Hibernate, false, false);
            if(!can)
                can = Application.SetSuspendState(PowerState.Suspend, false, false);

            if(!can)
            {
                this.notifyIcon1.BalloonTipText = "Ошибка";
                this.notifyIcon1.BalloonTipTitle = $"Не удалось перевести компьютер в спящий режим";
                this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;

                this.notifyIcon1.ShowBalloonTip(3000);
            }

            hibernateTimer?.Dispose();
        }

        private void MessageOnHibernate(object sender)
        {
            this.notifyIcon1.BalloonTipText = "Уведомление";
            this.notifyIcon1.BalloonTipTitle = $"Ваш компьютер будет переведен в спящий режим через 5 минут";
            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;

            this.notifyIcon1.ShowBalloonTip(3000);
        }

        private void SleepSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.notifyIcon1.BalloonTipText = "Уведомление";
            this.notifyIcon1.BalloonTipTitle = $"Таймер выключения остановлен";
            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

            this.notifyIcon1.ShowBalloonTip(3000);

            prepareTimer?.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timerSpan != null)
            {
                SetupTimer();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void SetupTimer()
        {
            this.Hide();

            this.notifyIcon1.BalloonTipText = "Уведомление";
            this.notifyIcon1.BalloonTipTitle = $"Компьютер будет переведен в спящий режим {DateTime.Now + timerSpan:G}";
            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

            this.notifyIcon1.ShowBalloonTip(3000);

            //hibernateTimer = new System.Threading.Timer(OnHibernate, null, timerSpan, TimeSpan.Zero);
            if(timerSpan != TimeSpan.MaxValue)
            {
                if(timerSpan.TotalMinutes > 5d)
                {
                    prepareTimer = new System.Threading.Timer(MessageOnHibernate, null, timerSpan - TimeSpan.FromMinutes(5), TimeSpan.Zero);
                }
            }
        }
    }
}
