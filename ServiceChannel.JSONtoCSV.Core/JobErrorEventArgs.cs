using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceChannel.JSONtoCSV.Core
{
    public class JobErrorEventArgs : EventArgs
    {
        private Worker _worker;
        private Job _job;
        private string _error;

        public JobErrorEventArgs(Worker worker, Job job, string error)
        {
            if (worker == null)
                throw new ArgumentNullException("worker");

            if (job == null)
                throw new ArgumentNullException("job");

            _worker = worker;
            _job = job;
            _error = error;
        }

        public Worker Worker
        {
            get { return _worker; }
        }

        public Job Job
        {
            get { return _job; }
        }

        public string Error
        {
            get { return _error; }
        }
    }
}
