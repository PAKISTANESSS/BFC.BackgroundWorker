using System;

namespace BFC.BackgroundWorker.Exceptions
{
    public class BackgroundQueueException : Exception
    {
        public BackgroundQueueException(string message) : base(message)
        {
        }
    }
}
