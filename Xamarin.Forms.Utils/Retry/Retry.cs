using Polly;

using System;
using System.Threading.Tasks;

namespace Celery.Helpers
{
	public class Retry
	{
		public static Task<T> AttemptAndRetry<T, TException>( Func<Task<T>> action, int numRetries = 10 ) where TException : Exception
		{
			return Policy.Handle<TException>().WaitAndRetryAsync( numRetries, pollyRetryAttempt ).ExecuteAsync( action );

			static TimeSpan pollyRetryAttempt( int attemptNumber ) => TimeSpan.FromMilliseconds( Math.Pow( 2, attemptNumber ) );
		}
	}
}