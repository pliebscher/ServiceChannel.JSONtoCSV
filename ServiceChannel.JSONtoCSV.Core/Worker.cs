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

                // Deserialize the JSON file...
                String locationsRaw = File.ReadAllText(job.Path);
                IEnumerable<Location> locations = JsonConvert.DeserializeObject<List<Location>>(locationsRaw);

                var cities = locations.GroupBy
                (
                    loc => loc.City.ToLower(),
                    loc => loc.Type.Count(), (city, locType) => new { City = city, Type = locType, Count = locType.Count() }
                );

                // Write the CSV file...
                using (var writer = new StreamWriter(job.Path.Replace("Incoming", "Outgoing").Replace(".json", ".csv")))
                {
                    writer.WriteLine(string.Format("\"City\", \"Type\", \"Count\""));

                    foreach (var result in cities)
                    {
                        writer.WriteLine(string.Format("\"{0}\", \"{1}\", \"{2}\"", result.City, result.Type, result.Count));
                        writer.Flush();
                    }
                }               

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
