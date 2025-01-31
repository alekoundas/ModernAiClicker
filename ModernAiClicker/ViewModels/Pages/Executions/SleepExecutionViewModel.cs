using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using System.Windows.Threading;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class SleepExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private string _timeLeft;

        [ObservableProperty]
        private string _timeTotal;

        private readonly DispatcherTimer _timer;
        private TimeSpan _timeElapsed = new TimeSpan();

        public SleepExecutionViewModel()
        {
            _execution = new Execution();

            // Update every second
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;

            if (Execution.FlowStep != null)
            {

                int miliseconds = 0;
                if (Execution.FlowStep.SleepForMilliseconds.HasValue)
                    miliseconds += Execution.FlowStep.SleepForMilliseconds.Value;

                if (Execution.FlowStep.SleepForSeconds.HasValue)
                    miliseconds += Execution.FlowStep.SleepForSeconds.Value * 1000;

                if (Execution.FlowStep.SleepForMinutes.HasValue)
                    miliseconds += Execution.FlowStep.SleepForMinutes.Value * 60 * 1000;

                if (Execution.FlowStep.SleepForHours.HasValue)
                    miliseconds += Execution.FlowStep.SleepForHours.Value * 60 * 60 * 1000;

                TimeTotal = TimeSpan.FromMilliseconds(miliseconds).ToString(@"hh\:mm\:ss");



                // Update every second
                _timeElapsed = TimeSpan.FromMilliseconds(miliseconds);

                void UpdateTimer(object sender, EventArgs e)
                {
                    _timeElapsed = _timeElapsed.Subtract(TimeSpan.FromSeconds(1));
                    TimeLeft = _timeElapsed.ToString(@"hh\:mm\:ss");
                }

                _timer.Tick += UpdateTimer;
                _timer.Start();
            }
        }
    }
}
