namespace MetroMVVM.Threading
{
    using System;
    using System.Collections.Generic;

    public delegate void AsyncOperation(Action<Exception> completed);

    public static class AsyncOperationService
    {
        public static void Run(this IEnumerable<AsyncOperation> asyncOps, Action<Exception> completed)
        {
            IEnumerator<AsyncOperation> enumerator = asyncOps.GetEnumerator();

            Action<Exception> disposeAndComplete = exception =>
            {
                enumerator.Dispose();
                completed(exception);
            };

            Action executeNextOp = null;
            executeNextOp = () =>
            {
                bool asyncCallbackExecuted = false;
                bool sequenceIncomplete = true;

                try
                {
                    sequenceIncomplete = enumerator.MoveNext();
                    if (sequenceIncomplete)
                    {
                        enumerator.Current(asyncError =>
                        {
                            if (!asyncCallbackExecuted)
                            {
                                asyncCallbackExecuted = true;

                                if (asyncError == null)
                                {
                                    executeNextOp();
                                }
                                else
                                {
                                    disposeAndComplete(asyncError);
                                }
                            }
                        });
                    }
                }
                catch (Exception syncError)
                {
                    disposeAndComplete(syncError);
                }

                if (!sequenceIncomplete)
                {
                    disposeAndComplete(null);
                }
            };
            executeNextOp();
        }
    }
}