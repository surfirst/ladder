# How to launch a new process from ASP.NET Core
This code shows how to launch a new process from asp.net core and how to collect console output from the process.

## Launch a new process
The code is in LadderAPI/Controllers/LadderController.cs.
You'd better set working folder like this. Otherwise, you may see your program cannot find some files.
process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);




## Capture console output
I recomend using 'process.StandardOutput.ReadToEnd()'. I failed to get output with callback method.
