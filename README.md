# How to launch a new process from ASP.NET Core
This code shows how to launch a new process from asp.net core and how to collect console output from the process. This will enable you to control running process

## Launch a new process
The code is in LadderAPI/Controllers/LadderController.cs.
You'd better set working folder like this. Otherwise, you may see your program cannot find some files.
process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);




## Capture console output
I recomend using 'process.StandardOutput.ReadToEnd()'. I failed to get output with callback method but ReadToEnd works well.

## Check if a process is running
We can use Process's static method to check it.
Process.GetProcessesByName(name)
