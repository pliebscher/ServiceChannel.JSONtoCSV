using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceChannel.JSONtoCSV.Core
{
    /// <summary>
    /// Provides Jobs to Workers as jobs are added to the queue and workers become available.
    /// </summary>
    public class WorkQueue
    {

        public event EventHandler<JobProcessedEventArgs> JobProcessed;

        private Queue<Worker> _workers = new Queue<Worker>();
        private Queue<Job> _jobs = new Queue<Job>();

        private object _lock = new Object();
        private Timer _queueTimer;
        private int _numWorkers;

        /// <summary>
        /// Constructs a WorkQueue.
        /// </summary>
        /// <param name="numWorkers">Number of Queue workers.</param>
        /// <param name="jobPollInterval">Interval in seconds to poll the queue for jobs.</param>
        public WorkQueue(int numWorkers, int jobPollInterval)
        {
            _numWorkers = numWorkers;

            for (int i = 0; i < numWorkers; i++)
            {
                _workers.Enqueue(new Worker());
            }

            // User a timer to track the job queue and give work out as Workers become available.
            _queueTimer = new System.Timers.Timer(jobPollInterval * 1000); 
            _queueTimer.Elapsed += OnProcessJobQueue;
            _queueTimer.AutoReset = true;
            _queueTimer.Enabled = false;
        }

        /// <summary>
        /// Starts processing the job queue.
        /// </summary>
        public void Start()
        {
            _queueTimer.Enabled = true;
        }

        /// <summary>
        /// Stops processing the job queue.
        /// </summary>
        public void Stop()
        {
            _queueTimer.Enabled = false;
        }

        private void OnProcessJobQueue(Object source, ElapsedEventArgs e)
        {
            // Process as many jobs as possible until we run out of woorkers...
            while (_jobs.Count > 0 && _workers.Count > 0)
            {
                Job job = _jobs.Dequeue();
                Worker worker = _workers.Dequeue();

                // Wire up some event handlers so we can track each job being worked on...
                worker.JobComplete += OnJobComplete;
                worker.JobError += OnJobError;

                // Do the work...
                Task.Run(() => worker.Process(job));

            }

        }

        protected virtual void OnJobProcessed(JobProcessedEventArgs e)
        {
                JobProcessed?.Invoke(this, e);         
        }

        private void OnJobComplete(object sender, JobCompleteEventArgs e)
        {
            // Update statistics and return the worker to the queue...
            this.SuccessCount += 1;

            if (e.ElapsedTime > this.MaxJobTime)
                this.MaxJobTime = e.ElapsedTime;

            this.EnqueueWorker(e.Worker);
            this.OnJobProcessed(new JobProcessedEventArgs(e.Job));
        }

        private void OnJobError(object sender, JobErrorEventArgs e)
        {
            this.ErrorCount += 1;
            this.EnqueueWorker(e.Worker);
            this.OnJobProcessed(new JobProcessedEventArgs(e.Job));
        }

        private void EnqueueWorker(Worker worker)
        {
            // Unook the worker and return to the queue...
            worker.JobComplete -= OnJobComplete;
            worker.JobError -= OnJobError;

            _workers.Enqueue(worker);
        }

        /// <summary>
        /// Jobs waiting to be worked on.
        /// </summary>
        public Queue<Job> Jobs
        {
            get { return _jobs; }
        }

        /// <summary>
        /// The number of failed jobs.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Number of successfully processed jobs.
        /// </summary>
        public int SuccessCount { get; private set; }

        /// <summary>
        /// Time in milliseconds of the longest running job.
        /// </summary>
        public long MaxJobTime { get; private set; }
    }
}
 