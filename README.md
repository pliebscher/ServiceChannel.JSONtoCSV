# ServiceChannel.JSONtoCSV
Converts incoming JSON location files to summarized CSV files. ServiceChannel Take-home Coding Project

Project Requirements

    Create a standalone utility that watches a directory for incoming files, processes them, outputs the results into a target folder
    You should have 4 directories: incoming, outgoing, success, error
        "incoming" is directory where a JSON file is dropped
        "outgoing" is for the generated CSV files
        "success" is where the original incoming JSON is put when it has been successfully processed
        "error" is where the original incoming JSON (and *.err file) is put if the JSON file could not be processed
    The incoming file is a JSON file with a list of "store locations" (sample inputs are attached)
    The outgoing file is a generated "summarized" CSV file
    The utility should "watch" the "incoming" folder for new JSON files and process them accordingly
    At any point of time, there should only be one copy of the store locations JSON file (valid directories: incoming, success, or error)
    If there is an error processing the JSON file, there should be a corresponding "*.err" that will should contains details of why the file could not be processed
    The store locations JSON file input format is:
        Array of "store locations"
            "name" - string - name of the store
            "city" - string - city where the store is located (these might not be normalized)
            "type" - string - type of store (e.g. retail, restaurant) (these might not be normalized)
        Each row must have all 3 fields (more can be ignored).  If any of the 3 fields are missing, the entire file should be rejected.
    The CSV file output format will be:
        File name is <inputfilename>.csv
        Each row is a comma delimited list of fields
        Each row contains a rollup of the number of store locations grouped by City and Type
        The first row must have the following header with 3 fields:
            "City", "Type", "Count"
        The output types are:
            City - string - (should be normalized)
            Type - string - (should be normalized)
            Count - integer
        The output should have the Types grouped by City, both in ascending output, e.g.:
            "Napa", "gym", 10
            "Napa", "restaurant", 60
            "Napa", "retail", 30

Technical Requirements

    Written in C#
    Must be able to run w/o modification on any machine w/o any source/config editing.
    For JSON parsing, you can use standard JSON reader libraries
    CSV output must be hand-written (i.e. no 3rd party CSV libraries)
    Code should be created/organized for proper maintainability
    Levels of complexity:
        Level 1 - Provided files work as expected, output matches requirements
        Level 2 - Will not reprocess files with the same name, but will move them to a unique location under the 'error' directory
        Level 3 - Concurrent processing.  Can process up to (and no more than) 3 files at the same time (i.e. multi-threaded)
        Level 4 - Every time a files changes state, the app will write to STDOUT the following details:
            In progress: XX
            Total processed: XX
            Successful: XX
            Errors: XX
            Maximum Processing Time (in millis): XXXXX
        Level 5 - ensure that the input files are processed when complete (i.e. the file should not be read when it might be written to by another process).  Also to ensure that the output file (<inputfilename>.csv) is only available when fully written to (i.e. the file should not be available while being written).  Hint: use the file system to your advantage (i.e. don't try and be clever on how to read the file.  There is nothing saying that you cannot use other files or that you have to initially write to the actual file name).
        Level 6 - If the app crashes during processing, application can work as expected when restarted (i.e. won't duplicate processing, won't leave invalid files around in multiple locations)
