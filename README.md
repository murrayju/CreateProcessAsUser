CreateProcessAsUser
===================

This uses the Win32 apis to:

1. Find the currently active user session
2. Spawn a new process in that session

This allows a process running in a different session (such as a windows service) to start a process with a graphical user interface that the user must see.

Note that the process must have the appropriate (admin) privileges for this to work correctly.

## Usage
```C#
using murrayju.ProcessExtensions;
// ...
ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
```

### Parameters
The second argument is used to pass the command line arguments as a string. Depending on the target application, `argv[0]` might be expected to be the executable name, or it might be the first parameter. See [this stack overflow answer](https://stackoverflow.com/a/14001282) for details. When in doubt, try it both ways.
