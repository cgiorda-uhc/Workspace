using Amazon;
using Cronos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAD_Worker_Service.Shared
{
    //https://github.com/HangfireIO/Cronos
    //https://stackoverflow.com/questions/73951786/how-to-run-10-long-running-functions-repeatedly-at-specific-times-avoiding-over/73960989#73960989
    //Here is a CronosTimer class similar in shape with the System.Timers.Timer class, that fires the Elapsed event on dates and times specified with a Cron expression.The event is fired in a non-overlapping manner. The CronosTimer has a dependency on the Cronos library by Sergey Odinokov. This library is a TimeSpan calculator, not a scheduler. Caveat: in its current version (0.7.1), the Cronos library is capped to the year 2099.

    //you can just create a separate CronosTimer for each long running function, and invoke this function from the Elapsed event handler of the associated CronosTimer.You could consider putting these timers in a list, so that you can dispose them when the service stops. Unless you are OK with the service stopping abruptly, while some timers might be in the midst of executing their function.


    //USAGE:
    //    CronosTimer timer = new("30 6,14,22 * * MON-FRI");
    //    timer.Elapsed += async _ =>
    //{
    //    try
    //    {
    //        await LongRunningAsync();
    //}
    //    catch (Exception ex)
    //{
    //    _logger.LogError(ex);
    //}
    //};
    public class CronosTimer : IAsyncDisposable
    {
        private readonly System.Threading.Timer _timer; // Used also as the locker.
        private readonly CronExpression _cronExpression;
        private readonly CancellationTokenSource _cts;
        private Func<CancellationToken, Task> _handler;
        private Task _activeTask;
        private bool _disposed;
        private static readonly TimeSpan _minDelay = TimeSpan.FromMilliseconds(500);

        public CronosTimer(string expression, CronFormat format = CronFormat.Standard)
        {
            _cronExpression = CronExpression.Parse(expression, format);
            //_cts = new CancellationTokenSource();
            _cts = new (); //NEW SHORT HAND
            _timer = new(async _ =>
            {
                Task task;
                lock (_timer)//CHECK IF _timer IS LOCKED. IF NOT PROCEED AND LOCK _timer. SUPPORTS _timer QUEUE FOR LOOPING _timer.Tasks
                {
                    if (_disposed) return; //IF DISPOSE UNLOCK
                    if (_activeTask is not null) return; //TASK IS RUNNING?
                    if (_handler is null) return; //_handler SET WITHIN DELEGATE Func<CancellationToken, Task> Elapsed
                    Func<CancellationToken, Task> handler = _handler; //NEW DELEGATE Task handler(CancellationToken c)
                    CancellationToken token = _cts.Token; //handler(CancellationToken c = token)
                    _activeTask = task = Task.Run(() => handler(token)); //RUN NEW DELEGATE handler(token)
                }
                try { await task.ConfigureAwait(false); } //USED TO LIBRARIES (TRUE IS UI)
                catch (OperationCanceledException) when (_cts.IsCancellationRequested) { }
                finally
                {
                    lock (_timer)
                    {
                        Debug.Assert(ReferenceEquals(_activeTask, task));
                        _activeTask = null; //CLEAN OUT TASK
                        if (!_disposed && _handler is not null) ScheduleTimer();
                    }
                }
            });
        }

        //UPDATE _timer. WITH NEXT TIME TO RUN
        private void ScheduleTimer()
        {
            Debug.Assert(Monitor.IsEntered(_timer)); //CHECK IF _timer IS LOCKED
            Debug.Assert(!_disposed); //CHECK IF _disposed IS FALSE
            Debug.Assert(_handler is not null); //CHECK IF _handler IS NULL ???
            //CSG ADDED TimeZoneInfo.Local AND CHANGED DateTime TO DateTimeOffset
            //DateTime utcNow = DateTime.UtcNow;
            DateTimeOffset utcNow = DateTimeOffset.Now;
            DateTimeOffset? utcNext = _cronExpression.GetNextOccurrence(utcNow + _minDelay, TimeZoneInfo.Local); //MOVE NEXT!!! 
            if (utcNext is null)
                throw new InvalidOperationException("Unreachable date.");
            TimeSpan delay = utcNext.Value - utcNow;
            Debug.Assert(delay > _minDelay); //CHEK IF ITS TIME
            _timer.Change(delay, Timeout.InfiniteTimeSpan); //ADD delay TO TIMER
        }

        /// <summary>
        /// Occurs when the next occurrence of the Cron expression has been reached,
        /// provided that the previous asynchronous operation has completed.
        /// The CancellationToken argument is canceled when the timer is disposed.
        /// </summary>
        public event Func<CancellationToken, Task> Elapsed
        {
            add
            {
                if (value is null) return; //NOTHING TO ADD
                lock (_timer)
                {
                    if (_disposed) return;
                    if (_handler is not null) throw new InvalidOperationException(
                        "More than one handlers are not supported."); //ONLY ONE OF THE SAME HANDLER AT A TIME
                    _handler = value; //SET HANDLER Func<CancellationToken, Task> _handler
                    if (_activeTask is null) ScheduleTimer(); //SCHEDULE _timer.Change(delay, Timeout.InfiniteTimeSpan); Task _activeTask
                }
            }
            remove
            {
                if (value is null) return; //NOTHING TO REMOVE
                lock (_timer)
                {
                    if (_disposed) return;
                    if (!ReferenceEquals(_handler, value)) return; //NOT SAME INSTANCE GET OUT!!!
                    _handler = null;//CLEAR HANDLER
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Returns a ValueTask that completes when all work associated with the timer
        /// has ceased.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            Task task;
            lock (_timer)//CLEANUP OF CURRENT _timer IS NOT LOCKED
            {
                if (_disposed) return; //ALREADY CLEANED, GET OUT
                _disposed = true;
                _handler = null;
                task = _activeTask; //LOCK TASK TO BE CANCELLED
            }
            await _timer.DisposeAsync().ConfigureAwait(false);
            _cts.Cancel(); //REQUEST CANCELLATION CancellationTokenSource
            if (task is not null) //WAIT FOR TASK TO CANCEL
                try { await task.ConfigureAwait(false); } catch { }
            _cts.Dispose(); //CLEAN UP CancellationTokenSource
        }
    }
}
