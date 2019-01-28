using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceChannel.JSONtoCSV.Core
{
    public class JobProcessedEventArgs : EventArgs
    {
        private Job _job;

        public JobProcessedEventArgs(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            _job = job;
        }

        /// <summary>
        /// The Job that was processed.
        /// </summary>
        public Job Job
        {
            get { return _job; }
        }

    }
}
