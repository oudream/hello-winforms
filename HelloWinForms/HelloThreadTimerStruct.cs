using CxWorkStation.Utilities;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HelloWinForms
{
    // 线程定时器：在独立线程中，加入定时器机制
    public class ThreadTimer : IDisposable
    {
        // 线程与线程运行标记
        private volatile bool _isRunning = false;
        private Thread _runningThread;

        // 线程信号
        private AutoResetEvent _updateSignal = new AutoResetEvent(false);

        // 待处理的事情队列、锁
        private readonly object _lockUpdateList = new object();
        private List<DealingEntry> _updateList = new List<DealingEntry>();

        // 定时器队列、锁
        private readonly object _lockTimerList = new object();
        private List<TimerEntry> _timerList = new List<TimerEntry>();

        //public int Times = 0;

        // 线程运行实体
        public void Run()
        {
            if (_isRunning) return;
            _isRunning = true;
            _runningThread = new Thread(() =>
            {
                DateTime nextEarlyMorningTime = GetNextEarlyMorningTime(DateTime.Now);

                while (_isRunning)
                {
                    try
                    {
                        _updateSignal.WaitOne(TimeSpan.FromMilliseconds(100));

                        var dtNow = DateTime.Now;

                        //Times++;

                        // 检测队列处理
                        ProcessQueue();

                        // 检查定时器
                        CheckTimers(dtNow);

                        // 执行凌晨任务
                        CheckAndPerformEarlyMorningTasks(dtNow, ref nextEarlyMorningTime);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"Exception in ThreadTimer thread, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                        // Optionally restart the thread or handle recovery
                    }

                }

            })
            { IsBackground = true };
            _runningThread.Start();
        }

        // 检测队列处理
        private void ProcessQueue()
        {
            List<DealingEntry> entries = null;
            lock (_lockUpdateList)
            {
                if (_updateList.Count > 0)
                {
                    entries = new List<DealingEntry>(_updateList);
                    _updateList.Clear();
                }
            }
            if (entries != null)
            {
                DealThings(entries);
            }
        }

        private void CheckAndPerformEarlyMorningTasks(DateTime dtNow, ref DateTime nextEarlyMorningTime)
        {
            // 检查是否到达了下一次凌晨的时间
            if (dtNow > nextEarlyMorningTime)
            {
                PerformEarlyMorningTasks();
                nextEarlyMorningTime = GetNextEarlyMorningTime(dtNow);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            // 通知线程
            _updateSignal.Set();
            _runningThread?.Join(); // Wait for the thread to finish
            Dispose(); // Ensure all resources are properly released
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateSignal?.Dispose();
                _updateSignal = null;
            }
        }

        private void CheckTimers(DateTime dtNow)
        {
            lock (_lockTimerList)
            {
                foreach (var timer in _timerList)
                {
                    // 检查是否到达了下一次执行时间
                    if (dtNow >= timer.NextRunTime)
                    {
                        timer.Callback?.Invoke();
                        timer.NextRunTime = timer.StartTime.AddMilliseconds(timer.PeriodMilliseconds);
                    }
                }
            }
        }

        public void AddTimer(int periodMilliseconds, Action callback)
        {
            lock (_lockTimerList)
            { 
                var startTime = DateTime.Now;
                _timerList.Add(new TimerEntry
                {
                    PeriodMilliseconds = periodMilliseconds,
                    StartTime = startTime,
                    NextRunTime = startTime.AddMilliseconds(periodMilliseconds),
                    Callback = callback
                });
            }
        }

        public void RemoveTimer(Action callback)
        {
            lock (_lockTimerList)
            {
                _timerList.RemoveAll(t => t.Callback == callback);
            }
        }

        private DateTime GetNextEarlyMorningTime(DateTime current)
        {
            // 计算下一个凌晨日期，通常为当前日期的次日凌晨
            return current.Date.AddDays(1).AddSeconds(30);
        }

        // 其它线程调用此方法
        // 压入待处理的事情队列
        public void PushDealingThing(string title, string content)
        {
            // 压入队列
            lock (_lockUpdateList)
            {
                _updateList.Add(new DealingEntry { Title = title, Content = content });
            }
            // 通知线程
            _updateSignal.Set();
        }

        // 处理待处理的事情列表
        private void DealThings(List<DealingEntry> entries)
        {
            // 按队列来处理
            foreach (var entry in entries)
            {
                try
                {
                    DealThing(entry);
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"处理事情 Failed，Title:'{entry.Title}',Content:'{entry.Content}',Exception: {ex.Message}");
                }
            }
        }

        // 处理待处理的事情
        private void DealThing(DealingEntry entry)
        {
            
        }

        // 执行凌晨任务
        private void PerformEarlyMorningTasks()
        {
            try
            {
                long dtNow = TimeHelper.GetNow();

                // 执行凌晨任务

                LogHelper.Debug($"凌晨任务 执行过程消耗时间：{TimeHelper.GetNow() - dtNow}ms");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"凌晨任务 Exception: {ex.Message}");
            }
        }

    }

    // 待处理的事情数据结构体
    public struct DealingEntry
    {
        public string Title;
        public string Content;
        public int ResInt;
        public string ResString;
    }

    public class TimerEntry
    {
        public int PeriodMilliseconds; // 定时周期（毫秒）
        public DateTime StartTime;
        public DateTime NextRunTime; // 下一次运行时间
        public Action Callback; // 回调函数
    }
}
