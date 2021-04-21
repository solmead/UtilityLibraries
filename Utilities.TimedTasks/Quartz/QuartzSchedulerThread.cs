using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.TimedTasks.Quartz
{
    internal class QuartzSchedulerThread
    {
        private readonly object sigLock = new object();

        private bool signaled;
        private DateTimeOffset? signaledNextFireTimeUtc;
        private bool paused;
        private bool halted;


        // When the scheduler finds there is no current trigger to fire, how long
        // it should wait until checking again...
        private static readonly TimeSpan DefaultIdleWaitTime = TimeSpan.FromSeconds(30);

        private TimeSpan idleWaitTime = DefaultIdleWaitTime;
        private int idleWaitVariableness = 7 * 1000;
        private CancellationTokenSource cancellationTokenSource = null!;
        private Task task = null!;

        private readonly QuartzRandom random = new QuartzRandom();

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        internal ILogger Log { get; }

        /// <summary>
        /// Sets the idle wait time.
        /// </summary>
        /// <value>The idle wait time.</value>
        //[TimeSpanParseRule(TimeSpanParseRule.Milliseconds)]
        internal virtual TimeSpan IdleWaitTime
        {
            set
            {
                idleWaitTime = value;
                idleWaitVariableness = (int)(value.TotalMilliseconds * 0.1);
            }
        }

        /// <summary>
        /// Gets the randomized idle wait time.
        /// </summary>
        /// <value>The randomized idle wait time.</value>
        private TimeSpan GetRandomizedIdleWaitTime()
        {
            return idleWaitTime - TimeSpan.FromMilliseconds(random.Next(-idleWaitVariableness, idleWaitVariableness));
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="QuartzSchedulerThread"/> is paused.
        /// </summary>
        /// <value><c>true</c> if paused; otherwise, <c>false</c>.</value>
        internal virtual bool Paused => paused;

        /// <summary>
        /// Construct a new <see cref="QuartzSchedulerThread" /> for the given
        /// <see cref="QuartzScheduler" /> as a non-daemon <see cref="Thread" />
        /// with normal priority.
        /// </summary>
        internal QuartzSchedulerThread(ILogger logger)//QuartzScheduler qs, QuartzSchedulerResources qsRsrcs)
        {
            Log = logger;
            IdleWaitTime = TimeSpan.FromSeconds(30);

            ////ThreadGroup generatedAux = qs.SchedulerThreadGroup;
            //this.qs = qs;
            //this.qsRsrcs = qsRsrcs;

            // start the underlying thread, but put this object into the 'paused'
            // state
            // so processing doesn't start yet...
            paused = false;
            halted = false;
        }

        /// <summary>
        /// Signals the main processing loop to pause at the next possible point.
        /// </summary>
        internal virtual void TogglePause(bool pause)
        {
            Log.LogInformation("QuartzSchedulerThread TogglePause [" + pause + "]");
            lock (sigLock)
            {
                paused = pause;

                if (paused)
                {
                    //SignalSchedulingChange(SchedulerConstants.SchedulingSignalDateTime);
                }
                else
                {
                    Monitor.PulseAll(sigLock);
                }
            }
        }

        /// <summary>
        /// Signals the main processing loop to pause at the next possible point.
        /// </summary>
        internal virtual async Task Halt(bool wait)
        {
            Log.LogInformation("QuartzSchedulerThread Halt [" + wait + "]");
            lock (sigLock)
            {
                halted = true;

                if (paused)
                {
                    Monitor.PulseAll(sigLock);
                }
                else
                {
                    //SignalSchedulingChange(SchedulerConstants.SchedulingSignalDateTime);
                }
            }

            if (wait)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }


        /// <summary>
        /// The main processing loop of the <see cref="QuartzSchedulerThread" />.
        /// </summary>
        public async Task Run()
        {
            Log.LogInformation("QuartzSchedulerThread Run");
            int acquiresFailed = 0;
            //Context.CallerId.Value = Guid.NewGuid();

            while (!halted)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    // check if we're supposed to pause...
                    lock (sigLock)
                    {
                        var fst = true;
                        while (paused && !halted)
                        {
                            if (fst)
                            {
                                Log.LogInformation("QuartzSchedulerThread Run is paused");
                                fst = false;
                            }
                            try
                            {
                                // wait until togglePause(false) is called...
                                Monitor.Wait(sigLock, 1000);
                            }
                            catch (ThreadInterruptedException)
                            {
                            }

                            // reset failure counter when paused, so that we don't
                            // wait again after unpausing
                            acquiresFailed = 0;
                        }
                        if (!fst && !halted)
                        {
                            Log.LogInformation("QuartzSchedulerThread Run is unpaused");
                        }
                        if (halted)
                        {
                            Log.LogInformation("QuartzSchedulerThread Run is halted");
                            break;
                        }
                    }

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    
                    bool goAhead;
                    lock (sigLock)
                    {
                        goAhead = !halted;
                    }

                    if (goAhead)
                    {
                        try
                        {
                            await Core.Instance.TriggerAsync();
                            //Fire triggers here

                        }
                        catch (Exception se)
                        {
                            Log.LogError(se, se.ToString());
                        }
                    }


                       

                    DateTimeOffset utcNow = DateTime.Now;
                    DateTimeOffset waitTime = utcNow.Add(GetRandomizedIdleWaitTime());
                    TimeSpan timeUntilContinue = waitTime - utcNow;
                    lock (sigLock)
                    {
                        if (!halted)
                        {
                            try
                            {
                                Monitor.Wait(sigLock, timeUntilContinue);
                            }
                            catch (ThreadInterruptedException)
                            {
                            }
                        }
                    }
                }
                catch (Exception re)
                {
                    Log.LogError(re, "Runtime error occurred in main trigger firing loop.");
                }
            } // while (!halted)
        }

        public void Start()
        {
            Log.LogInformation("QuartzSchedulerThread Start");
            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(Run);
        }

        public async Task Shutdown()
        {
            Log.LogInformation("QuartzSchedulerThread Shutdown");
            cancellationTokenSource.Cancel();
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
