using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.MediaConverter
{
    internal class ShellCommand
    {

        public delegate void DebugMessageEventHandler(string Msg);
        public delegate void ShellMessageEventHandler(string msg);


        public event DebugMessageEventHandler DebugMessage;
        public event ShellMessageEventHandler ShellMessage;






        public void DebugMsg(string Msg)
        {
            DebugMessage?.Invoke(Msg);
        }

        public void ShellMsg(string Msg)
        {
            ShellMessage?.Invoke(Msg);
        }




        private ProcessStartInfo GetProcessSettings(string command, string args)
        {
            var FI = new FileInfo(command);
            var pStart = new ProcessStartInfo();
            pStart.UseShellExecute = false;
            pStart.WindowStyle = ProcessWindowStyle.Normal; // System.Diagnostics.ProcessWindowStyle.Hidden
            pStart.Arguments = args;
            pStart.FileName = command;
            pStart.CreateNoWindow = true; // True
            pStart.RedirectStandardOutput = true;
            pStart.RedirectStandardError = true;
            pStart.WorkingDirectory = FI.Directory.FullName;

            return pStart;
        }

        /// <summary>
        /// Run a process asynchronously
        /// <para>To capture STDOUT, set StartInfo.RedirectStandardOutput to TRUE</para>
        /// <para>To capture STDERR, set StartInfo.RedirectStandardError to TRUE</para>
        /// </summary>
        /// <param name="startInfo">ProcessStartInfo object</param>
        /// <param name="timeoutMs">The timeout in milliseconds (null for no timeout)</param>
        /// <returns>Result object</returns>
        public async Task<Result> RunAsync(ProcessStartInfo startInfo, int? timeoutMs = null)
        {
            Result result = new Result();


            using (var process = new Process() { StartInfo = startInfo, EnableRaisingEvents = true })
            {
                // List of tasks to wait for a whole process exit
                List<Task> processTasks = new List<Task>();

                // === EXITED Event handling ===
                var processExitEvent = new TaskCompletionSource<object>();
                DebugMsg("RunAsync - Creating Exit Event Task");
                process.Exited += (sender, args) =>
                {
                    DebugMsg("RunAsync - Process Task Exited");
                    processExitEvent.TrySetResult(true);
                };
                processTasks.Add(processExitEvent.Task);

                // === STDOUT handling ===
                var stdOutBuilder = new StringBuilder();
                if (process.StartInfo.RedirectStandardOutput)
                {
                    DebugMsg("RunAsync - Creating StdOut Event Task");
                    var stdOutCloseEvent = new TaskCompletionSource<bool>();

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            DebugMsg("RunAsync - StdOut Task Finished");
                            stdOutCloseEvent.TrySetResult(true);
                        }
                        else
                        {
                            stdOutBuilder.AppendLine(e.Data);
                        }
                    };

                    processTasks.Add(stdOutCloseEvent.Task);
                }
                else
                {
                    // STDOUT is not redirected, so we won't look for it
                }

                // === STDERR handling ===
                var stdErrBuilder = new StringBuilder();
                if (process.StartInfo.RedirectStandardError)
                {

                    DebugMsg("RunAsync - Creating Error Event Task");
                    var stdErrCloseEvent = new TaskCompletionSource<bool>();

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            DebugMsg("RunAsync - Error Task Finished");
                            stdErrCloseEvent.TrySetResult(true);
                        }
                        else
                        {
                            stdErrBuilder.AppendLine(e.Data);

                            ShellMsg(e.Data);
                        }
                    };

                    processTasks.Add(stdErrCloseEvent.Task);
                }
                else
                {
                    // STDERR is not redirected, so we won't look for it
                }

                // === START OF PROCESS ===
                DebugMsg("RunAsync - Starting Process");
                if (!process.Start())
                {
                    result.ExitCode = process.ExitCode;
                    return result;
                }

                // Reads the output stream first as needed and then waits because deadlocks are possible
                if (process.StartInfo.RedirectStandardOutput)
                {
                    process.BeginOutputReadLine();
                }
                else
                {
                    // No STDOUT
                }

                if (process.StartInfo.RedirectStandardError)
                {
                    process.BeginErrorReadLine();
                }
                else
                {
                    // No STDERR
                }

                // === ASYNC WAIT OF PROCESS ===

                DebugMsg("RunAsync - processTasks.Count=" + processTasks.Count);
                // Process completion = exit AND stdout (if defined) AND stderr (if defined)
                Task processCompletionTask = Task.WhenAll(processTasks);

                // Task to wait for exit OR timeout (if defined)
                Task<Task> awaitingTask = timeoutMs.HasValue
                    ? Task.WhenAny(Task.Delay(timeoutMs.Value), processCompletionTask)
                    : Task.WhenAny(processCompletionTask);

                var finalTask = await awaitingTask;//.ConfigureAwait(false);
                DebugMsg("RunAsync - awaitingTask Completed");
                // Let's now wait for something to end...
                //(await awaitingTask.ConfigureAwait(false))
                if (finalTask == processCompletionTask)
                {
                    // -> Process exited cleanly
                    result.ExitCode = process.ExitCode;
                    DebugMsg("RunAsync - Process Exited");
                }
                else
                {
                    // -> Timeout, let's kill the process
                    try
                    {
                        DebugMsg("RunAsync - Process Killed");
                        process.Kill();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                // Read stdout/stderr
                result.StdOut = stdOutBuilder.ToString();
                result.StdErr = stdErrBuilder.ToString();
            }

            DebugMsg("RunAsync - Finished Process");
            return result;
        }





        public async Task<string> ShellAsync(string command, string arguments)
        {
            //return Shell(command, arguments);


            DebugMsg("ShellAsync - " + command);
            DebugMsg("ShellAsync - " + arguments);
            var PSI = GetProcessSettings(command, arguments);

            var timeOut = (60 * 60 * 2) * 1000;


            DebugMsg("ShellAsync - Running Async");
            var resp = await RunAsync(PSI, timeOut);
            DebugMsg("ShellAsync - Finished Running");

            return resp.StdErr;
        }




        private string Shell(string command, string arguments)
        {
            DebugMsg(command);
            DebugMsg(arguments);
            var p = new System.Diagnostics.Process();
            var PSI = GetProcessSettings(command, arguments);

            p.StartInfo = PSI;

            var sTimeOut = "" + (60 * 60 * 2); // System.Configuration.ConfigurationManager.AppSettings("TranscodeTimeoutSeconds")
            var timeOut = int.Parse(sTimeOut);

            DebugMsg("Shell - Starting Process Command:[" + command + "] Args:[" + arguments + "]");
            var success = p.Start();
            // p.PriorityClass = ProcessPriorityClass.BelowNormal
            DebugMsg("Shell - Process Started");
            var SR2 = p.StandardError;
            string S2 = "";
            if ((success))
            {
                DebugMsg("Shell - Process Success");

                var elapsedTime = System.Diagnostics.Stopwatch.StartNew();

                DebugMsg("Shell - Waiting till process exiting");
                while ((!p.HasExited))
                {
                    var SS = SR2.ReadLine();
                    // DebugMsg(SS)
                    ShellMsg(SS);
                    S2 = S2 + SS + "\n\r"; // Constants.vbCrLf;
                    if ((elapsedTime.Elapsed.Seconds > timeOut))
                    {
                        p.Kill();
                        p.Close();
                        p.Dispose();
                        p = null;

                        elapsedTime.Stop();
                        elapsedTime = null;
                        break;
                    }
                }
                DebugMsg("Shell - Process Completed");
            }
            DebugMsg("Shell - Getting Output");

            var SR = p.StandardOutput;
            var S = SR.ReadToEnd();

            S2 = S2 + SR2.ReadToEnd();
            return S2;
        }


        //public async Task<string> ShellAsync(string Command, string Args)
        //{

        //    Func<string> function = new Func<string>(() =>
        //    {
        //        var r = Shell(Command, Args);
        //        return r;
        //    });
        //    var tsk = Task.Run<string>(function);

        //    //tsk.Start();

        //    var ret =  await tsk;



        //    return ret;

        //}
    }


    /// <summary>
    /// Run process result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Exit code
        /// <para>If NULL, process exited due to timeout</para>
        /// </summary>
        public int? ExitCode { get; set; } = null;

        /// <summary>
        /// Standard error stream
        /// </summary>
        public string StdErr { get; set; } = "";

        /// <summary>
        /// Standard output stream
        /// </summary>
        public string StdOut { get; set; } = "";
    }
}
