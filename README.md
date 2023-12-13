CreateProcessAsUser
===================

This uses the Win32 apis to:

1. Find the currently active user session
2. Spawn a new process in that session

This allows a process running in a different session (such as a windows service) to start a process with a graphical user interface that the user must see.

Note that the process must have the appropriate (admin) privileges for this to work correctly. For [WTSQueryUserToken](https://github.com/murrayju/CreateProcessAsUser/blob/0381db2e8fb36f48794c073e87f773f7ca1ae039/ProcessExtensions/ProcessExtensions.cs#L197) you will need the __SE_TCB_NAME__ privilege, which is typically only held by Services running under the LocalSystem account ( [SO Link](https://stackoverflow.com/a/1289126/1872399) ).

## Usage
```C#
using murrayju.ProcessExtensions;
// ...
ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
```

### Parameters
The second argument is used to pass the command line arguments as a string. Depending on the target application, `argv[0]` might be expected to be the executable name, or it might be the first parameter. See [this stack overflow answer](https://stackoverflow.com/a/14001282) for details. When in doubt, try it both ways.
