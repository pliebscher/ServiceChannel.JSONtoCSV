using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceChannel.JSONtoCSV.Core
{
    public class JobCompleteEventArgs : EventArgs
    {
        private Worker _worker;
        private Job _job;
        private long _elapsedTime;

        public JobCompleteEventArgs(Worker worker, Job job, long elapsedTime)
        {
            if (worker == null)
                throw new ArgumentNullException("worker");

            if (job == null)
                throw new ArgumentNullException("job");

            _worker = worker;
            _job = job;
            _elapsedTime = elapsedTime;
        }

        public Worker Worker
        {
            get { return _worker; }
        }

        /// <summary>
        /// The Job that was processed.
        /// </summary>
        public Job Job
        {
            get { return _job; }
        }

        /// <summary>
        /// The amount of time in milliseconds it took to process the job.
        /// </summary>
        public long ElapsedTime
        {
            get { return _elapsedTime; }
        }
    }
}
