using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;
using ServiceChannel.JSONtoCSV.Models;

namespace ServiceChannel.JSONtoCSV.Core
{
    public class Worker
    {

        public event EventHandler<JobCompleteEventArgs> JobComplete;
        public event EventHandler<JobErrorEventArgs> JobError;

        public Worker()
        {

        }

        /// <summary>
        /// Processes a Job.
        /// </summary>
        /// <param name="job">The Job to be processed.</param>
        public void Process(Job job)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // TODO: Process file and copy to Output folder.


                File.Move(job.Path, job.Path.Replace("Incoming", "Success"));

                this.OnJobComplete(new JobCompleteEventArgs(this, job, sw.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                File.Move(job.Path, job.Path.Replace("Incoming", "Error"));
                
                // TODO: Dump ex to a file along side the json file.
                
                this.OnJobError(new JobErrorEventArgs(this, job, ex.ToString()));
            }
            finally
            {

            }
        }

        /// <summary>
        /// Raises the JobComplete event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnJobComplete(JobCompleteEventArgs e)
        {
            JobComplete?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the JobError event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnJobError(JobErrorEventArgs e)
        {
            JobError?.Invoke(this, e);
        }
    }

    
}
