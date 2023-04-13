using System;

namespace ZfyChatGPT.Global
{
    public static class EventDispatcher
    {
        private static Dictionary<string, Action<object>> _actions;
        private static Dictionary<string, Func<object>> _funcs;

        static EventDispatcher()
        {
            _funcs = new Dictionary<string, Func<object>>();
            _actions = new Dictionary<string, Action<object>>();
        }

        public static void AddAction(string key, Action<object> action)
        {
            if (!_actions.ContainsKey(key))
            {
                _actions.Add(key, action);
            }
            else
            {
                throw new Exception($"event key{key} has existed");
            }
        }

        public static void AddFunc(string key, Func<object> action)
        {
            if (!_funcs.ContainsKey(key))
            {
                _funcs.Add(key, action);
            }
            else
            {
                throw new Exception($"event key{key} has existed");
            }
        }

        public static void RemoveAction(string key)
        {
            if (_actions.ContainsKey(key))
            {
                _actions.Remove(key);
            }
        }

        public static void RemoveFunc(string key)
        {
            if (_funcs.ContainsKey(key))
            {
                _funcs.Remove(key);
            }
        }

        public static void DispatchAction(string key, object? value = null)
        {
            Console.WriteLine("Dispatch");
            Console.WriteLine(string.Join(",", _actions.Keys));
            if (_actions.ContainsKey(key))
            {
                var act = _actions[key];
                act.Invoke(value);
            }
        }

        public static object DispatchFunc(string key)
        {
            if (_funcs.ContainsKey(key))
            {
                var act = _funcs[key];
                return act.Invoke();
            }
            else { throw new Exception("未注册Func"); }
        }
    }
}