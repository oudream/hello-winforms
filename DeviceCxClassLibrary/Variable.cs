using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceCxClassLibrary
{
    // 变量基类
    public abstract class VariableBase
    {
        public int Id { get; set; }
        public int Gid { get; set; }
        public string Code { get; set; }
        public string URI { get; set; }
        public string Name { get; set; }
        public Type ValueType
        {
            get
            {
                return GetValueType(); // 存储类型
            } // 存储类型
        }

        public string ValidRange { get; set; } // 有效范围
        public int MaxLength { get; set; } // 最大长度

        public string VariableType { get; set; } // 变量类型（功能、业务）

        public DateTime LastChangeTime { get; private set; } // 最近一次值变化时间
        public int ChangeSource { get; set; } // 变化源


        protected abstract Type GetValueType();
        public abstract object GetValueObject();
        public abstract void UpdateValue(object newValue);

        protected void RecordChange()
        {
            LastChangeTime = DateTime.UtcNow; // 记录变化时间（UTC时间）
        }
    }


    // 整数类型的变量
    public class VariableInt : VariableBase
    {
        public int Value { get; private set; }
        protected override Type GetValueType() => typeof(int);
        public override object GetValueObject() => Value;

        public override void UpdateValue(object newValue)
        {
            if (newValue is int intValue)
            {
                Value = intValue;
                RecordChange(); // 记录变化时间
            }
        }
    }

    // 字符串类型的变量
    public class VariableString : VariableBase
    {
        public string Value { get; private set; }

        protected override Type GetValueType() => typeof(string);
        public override object GetValueObject() => Value;

        public override void UpdateValue(object newValue)
        {
            if (newValue is string stringValue)
            {
                Value = stringValue;
                RecordChange(); // 记录变化时间
            }
        }
    }

    public class VariableDouble : VariableBase
    {
        public double Value { get ; private set; }

        protected override Type GetValueType() => typeof(double);
        public override object GetValueObject() => Value;

        public override void UpdateValue(object newValue)
        {
            if (newValue is double doubleValue)
            {
                Value = doubleValue;
                RecordChange(); // 记录变化时间
            }
        }
    }

    /**
        VariableBase variable;
        switch (entry.Value)
        {
            case int intValue:
                variable = new VariableInt { Id = entry.Id, Value = intValue };
                break;
            case double doubleValue:
                variable = new VariableDouble { Id = entry.Id, Value = doubleValue };
                break;
            case string stringValue:
                variable = new VariableString { Id = entry.Id, Value = stringValue };
                break;
            default:
                throw new InvalidOperationException("Unsupported variable type");
        }
     */

    public struct VariableEntry
    {
        public int UpdateId { get; set; }
        public int Id { get; set; }
        public Object Value { get; set; }
        public int Source { get; set; }
    }


    public class VariableManager
    {
        private Dictionary<int, VariableBase> _variables = new Dictionary<int, VariableBase>();

        private int _updateId;
        //private ConcurrentStack<VariableEntry> _updateStack;

        private readonly object _listLock = new object();
        private List<VariableEntry> _updateList = new List<VariableEntry>();

        private ManualResetEvent _updateSignal = new ManualResetEvent(false);
        public event Action<int, List<VariableEntry>> OnVariableChanged;

        public VariableManager()
        {
            //_updateStack = new ConcurrentStack<VariableEntry>();
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        // 等待一小段时间或直到收到更新信号
                        //_updateSignal.WaitOne(); // 等待更新信号
                        _updateSignal.WaitOne(TimeSpan.FromMilliseconds(100));

                        // 处理所有积累的更新
                        //if (_updateStack.IsEmpty)
                        //{
                        //    _updateSignal.Reset();
                        //    continue;
                        //}

                        // 
                        ProcessUpdates();

                        _updateSignal.Reset();
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常
                    LogException(ex);
                }

            })
            { IsBackground = true }.Start();
        }

        private void LogException(Exception ex)
        {
            // 这里只是一个示例，具体实现取决于您的日志系统
            Console.WriteLine($"Exception in VariableManager thread: {ex.Message}");
            // 如果有更复杂的日志系统，可以这样：
            // Logger.Error(ex, "Exception in VariableManager thread");
        }

        public void RegisterVariable(VariableBase variable)
        {
            if (!_variables.ContainsKey(variable.Id))
            {
                _variables[variable.Id] = variable;
            }
        }

        // Push 一个变量
        public void PushInt(int id, int value, int source)
        {
            Interlocked.Increment(ref _updateId);
            var entry = new VariableEntry { UpdateId = _updateId, Id = id, Value = value, Source = source };
            lock (_listLock)
            {
                _updateList.Add(entry);
            }
            _updateSignal.Set(); // 触发信号通知更新
        }
        public void PushDouble(int id, double value, int source)
        {
            Interlocked.Increment(ref _updateId);
            var entry = new VariableEntry { UpdateId = _updateId, Id = id, Value = value, Source = source };
            lock (_listLock)
            {
                _updateList.Add(entry);
            }
            _updateSignal.Set(); // 触发信号通知更新
        }
        public void PushString(int id, string value, int source)
        {
            Interlocked.Increment(ref _updateId);
            var entry = new VariableEntry { UpdateId = _updateId, Id = id, Value = value, Source = source };
            lock (_listLock)
            {
                _updateList.Add(entry);
            }
            _updateSignal.Set(); // 触发信号通知更新
        }

        // Push 多个变量
        public void PushInts(params (int Id, int Value, int Source)[] ps)
        {
            //var lv = ps.Select(e => new VariableEntry { Id = e.Id, Value = e.Value, Source = e.Source });
            lock (_listLock)
            {
                foreach (var p in ps)
                {
                    Interlocked.Increment(ref _updateId);
                    var entry = new VariableEntry { UpdateId = _updateId, Id = p.Id, Value = p.Value, Source = p.Source };
                    _updateList.Add(entry);
                }
            }
            _updateSignal.Set(); // 只需触发一次信号
        }
        public void PushDoubles(params (int Id, double Value, int Source)[] ps)
        {
            //var lv = ps.Select(e => new VariableEntry { Id = e.Id, Value = e.Value, Source = e.Source });
            lock (_listLock)
            {
                foreach (var p in ps)
                {
                    Interlocked.Increment(ref _updateId);
                    var entry = new VariableEntry { UpdateId = _updateId, Id = p.Id, Value = p.Value, Source = p.Source };
                    _updateList.Add(entry);
                }
            }
            _updateSignal.Set(); // 只需触发一次信号
        }
        public void PushStrings(params (int Id, string Value, int Source)[] ps)
        {
            //var lv = ps.Select(e => new VariableEntry { Id = e.Id, Value = e.Value, Source = e.Source });
            lock (_listLock)
            {
                foreach (var p in ps)
                {
                    Interlocked.Increment(ref _updateId);
                    var entry = new VariableEntry { UpdateId = _updateId, Id = p.Id, Value = p.Value, Source = p.Source };
                    _updateList.Add(entry);
                }
            }
            _updateSignal.Set(); // 只需触发一次信号
        }
        private void ProcessUpdates()
        {
            List<VariableEntry> nvList;
            lock (_listLock)
            {
                nvList = new List<VariableEntry>(_updateList);
                _updateList.Clear();
            }
            if (nvList.Count <= 0)
            {
                return;
            }
            //List<VariableEntry> nvList = _updateStack.ToList();
            //_updateStack.Clear();
            //while (_updateStack.TryPop(out var logEntry))
            //{
            //    nvList.Add(logEntry);
            //}
            //nvList.Sort((a, b) => a.Id.CompareTo(b.Id));

            List<VariableEntry> changedEntries = new List<VariableEntry>();
            foreach (var nv in nvList)
            {
                var oldVariable = _variables[nv.Id];
                if (oldVariable != null)
                {
                    var oldValue = oldVariable.GetValueObject();
                    if (!Equals(oldValue, nv.Value))
                    {
                        oldVariable.UpdateValue(nv.Value);
                        changedEntries.Add(nv); // 收集发生变化的条目
                    }
                }
            }

            if (changedEntries.Any())
            {
                OnVariableChanged?.Invoke(changedEntries.Count, changedEntries); // 一次性通知所有变化
            }
        }

    }

    /**  事件处理程序的线程安全性
     * 
// 操作共享资源 
private readonly object _lockObject = new object();

private void OnVariableChangedHandler(int id, VariableEntry entry)
{
        lock (_lockObject)
        {
            // 安全地操作共享资源
        }
}

// Windows Forms 示例
private void OnVariableChangedHandler(int id, VariableEntry entry)
{
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(() => UpdateUI(id, entry)));
        }
        else
        {
            UpdateUI(id, entry);
        }
}

private void UpdateUI(int id, VariableEntry entry)
{
        // 更新 UI 的代码
}

     */

}
