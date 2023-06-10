namespace MLock
{
    internal class Events
    {
        public delegate void Event();

        public enum EventSource
        {
            USB,
            Password
        }

        private static EventSource _unlockSource;
        public static event Event UnlockApp;
        public static event Event LockApp;

        public static void Unlock(EventSource source = EventSource.USB)
        {
            _unlockSource = source;
            UnlockApp?.Invoke();
        }

        public static void Lock(EventSource source)
        {
            if (_unlockSource == source) LockApp?.Invoke();
        }

        public static void Lock()
        {
            LockApp?.Invoke();
        }
    }
}