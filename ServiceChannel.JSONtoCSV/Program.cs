using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ServiceChannel.JSONtoCSV.Core;

namespace ServiceChannel.JSONtoCSV
{
    class Program
    {
        static void Main(string[] args)
        {

            // Init stuff...
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Incoming")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Incoming"));

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Outgoing")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Outgoing"));

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Success")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Success"));

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Error")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Error"));

            Program processor = new Program();
            processor.Run();

            Console.WriteLine("> Running! Press any key to exit...");
            Console.WriteLine("---------------------------------------------");
            Console.ReadKey();

            // Dispose, cleanup, etc...
        }



        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private WorkQueue _workQueue = new WorkQueue(3, 2);

        private void Run()
        {
            _watcher.Path = Path.Combine(Environment.CurrentDirectory, @"Incoming");
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.Filter = "*.json";
            _watcher.Changed += new FileSystemEventHandler(OnIncomingFile);
            _watcher.EnableRaisingEvents = true;

            _workQueue.JobProcessed += OnJobProcessed;
            _workQueue.Start();

        }

        private void OnIncomingFile(object source, FileSystemEventArgs e)
        {
            Job job = new Job(e.Name, e.FullPath);

            if (!_workQueue.Jobs.Contains(job)) // Hack for FileSystemWatcher firing multiple events for same file.
            {
                _workQueue.Jobs.Enqueue(job);                
            }    
        }

        private void OnJobProcessed(object sender, JobProcessedEventArgs e)
        {
            lock (_workQueue) // Output is not clean when multiple threads are hitting Console.Clear(). A timer may be a better consideration so we don't block!
            {
                Console.Clear();
                Console.WriteLine("> Running! Press any key to exit...");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("In Progress: {0}", _workQueue.Jobs.Count);
                Console.WriteLine("Total Processed: {0}", _workQueue.SuccessCount + _workQueue.ErrorCount);
                Console.WriteLine("Successful: {0}", _workQueue.SuccessCount);
                Console.WriteLine("Errors: {0}", _workQueue.ErrorCount);
                Console.WriteLine("Maximum Processing Time: {0}", _workQueue.MaxJobTime);
                Console.WriteLine("---------------------------------------------");
            }
        }
    }
}
