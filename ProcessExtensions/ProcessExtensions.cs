using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.RemoteDesktop;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace murrayju.ProcessExtensions
{
    /// <summary>
    /// Custom exception for handling process creation failures.
    /// </summary>
    public class ProcessCreationException : Exception
    {
        public ProcessCreationException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Provides methods for creating processes under the active user session.
    /// </summary>
    public static class ProcessExtensions
    {
        private const uint INVALID_SESSION_ID = 0xFFFFFFFF;

        /// <summary>
        /// Retrieves the user token from the currently active session.
        /// </summary>
        /// <param name="phUserToken">A handle to the user token for the active session.</param>
        /// <returns>True if the token retrieval is successful; otherwise, false.</returns>
        private static unsafe bool GetSessionUserToken(ref SafeFileHandle phUserToken)
        {
            var bResult = false;
            HANDLE hImpersonationToken = default;
            var activeSessionId = INVALID_SESSION_ID;

            if (WTSEnumerateSessions(HANDLE.Null, 0, 1, out var pSessionInfo, out var sessionCount) != 0)
            {
                var sessionInfo = pSessionInfo;

                for (var i = 0; i < sessionCount; i++)
                {
                    if (sessionInfo[i].State == WTS_CONNECTSTATE_CLASS.WTSActive)
                    {
                        activeSessionId = sessionInfo[i].SessionId;
                        break;
                    }
                }

                WTSFreeMemory(pSessionInfo);
            }

            if (activeSessionId == INVALID_SESSION_ID)
            {
                activeSessionId = WTSGetActiveConsoleSessionId();

                if (activeSessionId == INVALID_SESSION_ID)
                {
                    return false;
                }
            }

            if (WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
            {
                var hImpersonationTokenSafe = new SafeFileHandle(hImpersonationToken, true);

                bResult = DuplicateTokenEx(hImpersonationTokenSafe, 0, null, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out phUserToken);

                CloseHandle(hImpersonationToken);
            }

            return bResult;
        }

        /// <summary>
        /// Starts a process as the current active user.
        /// </summary>
        /// <param name="appPath">The path to the application to start.</param>
        /// <param name="cmdLine">The command line arguments for the process.</param>
        /// <param name="workDir">The working directory for the process.</param>
        /// <param name="visible">Whether the process window should be visible.</param>
        /// <returns>True if the process starts successfully; otherwise, false.</returns>
        /// <exception cref="ProcessCreationException">Thrown when the process could not be started.</exception>
        public unsafe static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null, bool visible = true)
        {
            SafeFileHandle hUserToken = null;
            var startInfo = new STARTUPINFOW();
            var procInfo = new PROCESS_INFORMATION();
            void* pEnv = null;
            int iResultOfCreateProcessAsUser;

            startInfo.cb = (uint)Marshal.SizeOf(typeof(STARTUPINFOW));

            try
            {
                if (!GetSessionUserToken(ref hUserToken))
                {
                    throw new ProcessCreationException("StartProcessAsCurrentUser: GetSessionUserToken failed.");
                }

                var dwCreationFlags = PROCESS_CREATION_FLAGS.CREATE_UNICODE_ENVIRONMENT | (visible ? PROCESS_CREATION_FLAGS.CREATE_NEW_CONSOLE : PROCESS_CREATION_FLAGS.CREATE_NO_WINDOW);
                startInfo.dwFlags = STARTUPINFOW_FLAGS.STARTF_USESHOWWINDOW;
                startInfo.wShowWindow = (ushort)(visible ? SHOW_WINDOW_CMD.SW_SHOW : SHOW_WINDOW_CMD.SW_HIDE);

                fixed (char* pDesktopName = $@"winsta0\Default")
                {
                    startInfo.lpDesktop = pDesktopName;
                }

                if (!CreateEnvironmentBlock(out pEnv, hUserToken, false))
                {
                    throw new ProcessCreationException("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");
                }

                if (workDir != null)
                {
                    Directory.SetCurrentDirectory(workDir);
                }

                cmdLine += char.MinValue;
                var commandSpan = new Span<char>(cmdLine.ToCharArray());

                if (!CreateProcessAsUser(hUserToken, appPath, ref commandSpan, null, null, false, dwCreationFlags, pEnv, workDir, startInfo, out procInfo))
                {
                    iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
                    throw new ProcessCreationException("StartProcessAsCurrentUser: CreateProcessAsUser failed. Error Code -" + iResultOfCreateProcessAsUser);
                }

                iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
            }
            finally
            {
                if (pEnv != null)
                {
                    DestroyEnvironmentBlock(pEnv);
                }

                CloseHandle(procInfo.hThread);
                CloseHandle(procInfo.hProcess);
            }

            return true;
        }
    }
}
