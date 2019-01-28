using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServiceChannel.JSONtoCSV.Core
{

    /// <summary>
    /// Represents a job to be worked on.
    /// </summary>
    public class Job : IEquatable<Job>
    {
        private string _name;
        private string _path;

        /// <summary>
        /// Constructs a job to be worked on.
        /// </summary>
        /// <param name="name">A unique name for this job.</param>
        /// <param name="path">The full path to the JSON file.</param>
        public Job(string name, string path)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            _name = name;

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            _path = path;

        }

        /// <summary>
        /// The name of the job.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The path to the JSON file.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Job);
        }

        public bool Equals(Job other)
        {
            return _name == other.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
