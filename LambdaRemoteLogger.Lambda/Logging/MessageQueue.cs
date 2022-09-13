using System.Collections.Concurrent;
using System.Reflection;

namespace LambdaRemoteLogger.Lambda.Logging
{
    public class MessageQueue
    {
        private BlockingCollection<string> _messages;
        private Task _outputTask;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly List<string> _currentBatch = new List<string>();
        private int _messagesDropped;

        internal void Start()
        {
            _messages = new BlockingCollection<string>(new ConcurrentQueue<string>(), 20);
            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Run(ProcessLogQueue);
        }

        internal void Stop()
        {
            try
            {
                if (_messages.Count > 0)
                {
                    // Remaining messages in queue. Flush them if possible.
                    ProcessMessages().GetAwaiter().GetResult();
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }

            _cancellationTokenSource.Cancel();
            _messages.CompleteAdding();
        }

        public void AddMessage(string message)
        {
            if (!_messages.IsAddingCompleted)
            {
                try
                {
                    if (!_messages.TryAdd(message, millisecondsTimeout: 0, cancellationToken: _cancellationTokenSource.Token))
                    {
                        Interlocked.Increment(ref _messagesDropped);
                    }
                }
                catch
                {
                    // Cancellation token canceled or CompleteAdding called
                }
            }
        }

        private async Task WriteMessagesAsync(IEnumerable<string> messages, CancellationToken token)
        {
            var bulk = new List<string>();
            foreach (var message in messages)
            {
                bulk.Add(message);

                if (bulk.Count >= 20)
                {
                    Console.WriteLine($"Simulate writing {bulk.Count} messages to a remote logger");
                    bulk.Clear();
                }
            }

            if (bulk.Count > 0)
            {
                Console.WriteLine($"Simulate writing {bulk.Count} messages to a remote logger");
            }

        }

        private Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        private async Task ProcessLogQueue()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                // Process messages
                await ProcessMessages();
                // Then wait until next check
                await IntervalAsync(TimeSpan.FromSeconds(10), _cancellationTokenSource.Token);
            }
        }

        private async Task ProcessMessages()
        {
            while (_messages.TryTake(out var message))
            {
                _currentBatch.Add(message);
            }

            if (_currentBatch.Count > 0)
            {
                try
                {
                    await WriteMessagesAsync(_currentBatch, _cancellationTokenSource.Token);
                }
                catch
                {
                    // ignored
                }

                _currentBatch.Clear();
            }
        }
    }
}
