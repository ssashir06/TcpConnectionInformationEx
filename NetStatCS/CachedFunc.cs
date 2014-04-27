using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStatCS
{
    class CachedFunc<TKey, TValue> : IDisposable
    {
        Dictionary<TKey, Tuple<TValue, DateTime>> _cache = new Dictionary<TKey, Tuple<TValue, DateTime>>();
        Func<TKey, TValue> _func;
        Task _check_timeout;
        bool _quit;

        public TimeSpan? Timeout { get; set; }

        public CachedFunc(Func<TKey, TValue> Func)
        {
            _func = Func;
            _check_timeout = Task.Factory.StartNew(() =>
            {
                while (!_quit)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(250));
                    CheckTimeout();
                }
            });
        }

        void CheckTimeout()
        {
            if (!Timeout.HasValue) return;

            lock (_cache)
            {
                var query_timedout =
                    from kv in _cache
                    where DateTime.Now - kv.Value.Item2 > Timeout.Value
                    select kv.Key;
                query_timedout.ToList().ForEach(key => _cache.Remove(key));
            }
        }

        public TValue this[TKey Key]
        {
            get
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(Key))
                    {
                        return _cache[Key].Item1;
                    }
                    else
                    {
                        TValue value = _func(Key);
                        _cache.Add(Key, Tuple.Create((TValue)value, DateTime.Now));
                        return value;
                    }
                }
            }
            set
            {
                lock (_cache)
                {
                    _cache[Key] = Tuple.Create((TValue)value, DateTime.Now);
                }
            }
        }

        #region IDisposable メンバー

        public void Dispose()
        {
            _quit = true;
            _check_timeout.Wait();
            lock (_cache) { _cache.Clear(); }
            _cache = null;
        }

        #endregion
    }
}
