using System;
using System.Threading;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// Provides a mechanism that synchronizes access to objects with a specified timeout (using Monitor.TryEnter).
    /// </summary>
    /// <remarks>
    /// This is a Monitor.TryEnter construct developed by Ian Griffith.
    /// http://www.interact-sw.co.uk/iangblog/2004/03/23/locking
    /// </remarks>
    /// <example>
    /// private static readonly object _locker = new object();
    /// 
    /// using (TimedLock.Lock(_locker))
    /// {
    ///     ...
    /// }
    /// </example>
    public struct TimedLock : IDisposable
    {
        /// <summary>
        /// Attempts, for 5 sec, to acquire an exclusive lock on the specified object, and throws a LockTimeoutException if lock could not be taken.
        /// </summary>
        /// <param name="o">The object on which to acquire the lock.</param>
        /// <returns>A TimedLock object which allows for a using statement</returns>
        /// <exception cref="LockTimeoutException">
        /// The lock could not be taken in the specified amount of time.
        /// </exception>
        public static TimedLock Lock(object o)
        {
            return Lock(o, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Attempts, for 5 sec, to acquire an exclusive lock on the specified object, and throws a LockTimeoutException if lock could not be taken.
        /// </summary>
        /// <param name="o">The object on which to acquire the lock.</param>
        /// <param name="message">The message to be thrown by the LockTimeoutException.</param>
        /// <returns>A TimedLock object which allows for a using statement</returns>
        /// <exception cref="LockTimeoutException">
        /// The lock could not be taken in the specified amount of time.
        /// </exception>
        public static TimedLock Lock(object o, string message)
        {
            return Lock(o, TimeSpan.FromSeconds(5), message);
        }

        /// <summary>
        /// Attempts, for the specified amount of time, to acquire an exclusive lock on the specified object, and throws a LockTimeoutException if lock could not be taken.
        /// </summary>
        /// <param name="o">The object on which to acquire the lock.</param>
        /// <param name="timeout">The amount of time to wait for the lock. A value of –1 millisecond specifies an infinite wait.</param>
        /// <param name="message">The message to be thrown by the LockTimeoutException.</param>
        /// <returns>
        /// A TimedLock object which allows for a using statement
        /// </returns>
        /// <exception cref="LockTimeoutException">
        /// The lock could not be taken in the specified amount of time.
        /// </exception>
        public static TimedLock Lock(object o, TimeSpan timeout, string message = "")
        {
            TimedLock tl = new TimedLock(o);
            if (!Monitor.TryEnter(o, timeout))
            {
                LockTimeoutException timeoutEx = null;

                if (!String.IsNullOrEmpty(message))
                    timeoutEx = new LockTimeoutException(message);
                else
                    timeoutEx = new LockTimeoutException();

                throw timeoutEx;
            }

            return tl;
        }

        private TimedLock(object o)
        {
            target = o;
        }
        private object target;

        /// <summary>
        /// Releases the TimedLock.
        /// </summary>
        public void Dispose()
        {
            Monitor.Exit(target);
        }
    }

    #region LockTimeoutException

    /// <summary>
    /// Represents a timeout error waiting for a lock.
    /// </summary>
    public class LockTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockTimeoutException"/> class.
        /// </summary>
        public LockTimeoutException()
            : base("Operation timed out (waiting for lock)")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LockTimeoutException(string message)
            : base(message)
        {
        }
    }

    #endregion
}
