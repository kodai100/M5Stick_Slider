using System;
using System.Threading;
using System.Threading.Tasks;

    
public static class TaskExtensions
{
    public static async Task<T> WithCancellation<T>( this Task<T> task, CancellationToken cancellationToken )
    {
        var tcs = new TaskCompletionSource<bool>();
        using( cancellationToken.Register( s => ( (TaskCompletionSource<bool>)s ).TrySetResult( true ), tcs ) )
        {
            if( task != await Task.WhenAny( task, tcs.Task ) )
            {
                throw new OperationCanceledException( cancellationToken );
            }
        }

        return task.Result;
    }
}

