namespace GameLauncher.App.Classes
{
    /// <summary>
    /// A simple singleton class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object InstanceLock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
