using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Services.Interfaces;
using TwitchLeecher.Shared.Helpers;
using TwitchLeecher.Shared.IO;

namespace TwitchLeecher.Services.Services
{
    internal class ProcessingService : IProcessingService
    {
        #region Constants

        public string FFMPEGExe =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? "ffmpeg"
                : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");

        #endregion Constants

        #region Properties

        #endregion Properties

        #region Methods

        public IEnumerable<string> ConcatParts(Action<string> log, Action<string> setStatus, Action<double> setProgress,
            TwitchPlaylist vodPlaylist, string concatFile)
        {
            var warnings = new List<string>();
            
            setStatus("Merging files");
            setProgress(0);

            log(Environment.NewLine + Environment.NewLine + "Merging all VOD parts into '" + concatFile + "'...");

            using (FileStream outputStream = new FileStream(concatFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                TwitchPlaylistPart initSegment = vodPlaylist.InitSegment;

                if (initSegment != null)
                {
                    if (File.Exists(initSegment.LocalFile))
                    {
                        using (FileStream initStream = new FileStream(initSegment.LocalFile, FileMode.Open, FileAccess.Read))
                        {
                            initStream.CopyTo(outputStream);
                        }

                        FileSystem.DeleteFile(initSegment.LocalFile);
                    }
                    else
                    {
                        warnings.Add($"Warning: missing VOD init segment: {initSegment.LocalFile}");
                    }
                }

                int partsCount = vodPlaylist.Count;

                for (int i = 0; i < partsCount; i++)
                {
                    TwitchPlaylistPart part = vodPlaylist[i];

                    if (!File.Exists(part.LocalFile)) {
                        warnings.Add($"Warning: missing VOD segment: {part.LocalFile}");
                        continue;
                    }

                    using (FileStream partStream = new FileStream(part.LocalFile, FileMode.Open, FileAccess.Read))
                    {
                        partStream.CopyTo(outputStream);
                    }

                    FileSystem.DeleteFile(part.LocalFile);

                    setProgress(i * 100 / partsCount);
                }
            }

            setProgress(100);
            return warnings;
        }


        public void ConvertVideo(Action<string> log, Action<string> setStatus, Action<double> setProgress,
            Action<bool> setIsIndeterminate, string sourceFile, string outputFile, CropInfo cropInfo)
        {
            RunFfmpeg(log, setStatus, setProgress, setIsIndeterminate, sourceFile, outputFile, cropInfo, null);
        }

        public IEnumerable<string> ConvertParts(Action<string> log, Action<string> setStatus,
            Action<double> setProgress, Action<bool> setIsIndeterminate, TwitchPlaylist vodPlaylist,
            string outputFile, CropInfo cropInfo, CancellationToken cancellationToken)
        {
            // Streams init segment + chunks (the exact bytes ConcatParts would write) into ffmpeg's stdin, so no
            // merged intermediate file is needed. stdin is not seekable, so the input-side -ss used for CropStart
            // cannot work here; callers must use ConcatParts + ConvertVideo when CropStart is set.
            if (cropInfo.CropStart)
            {
                throw new InvalidOperationException("Streaming conversion does not support CropStart.");
            }

            var warnings = new List<string>();

            RunFfmpeg(log, setStatus, setProgress, setIsIndeterminate, null, outputFile, cropInfo, stdin =>
            {
                TwitchPlaylistPart initSegment = vodPlaylist.InitSegment;

                if (initSegment != null)
                {
                    if (File.Exists(initSegment.LocalFile))
                    {
                        if (!PipeFile(initSegment.LocalFile, stdin))
                        {
                            return;
                        }
                    }
                    else
                    {
                        warnings.Add($"Warning: missing VOD init segment: {initSegment.LocalFile}");
                    }
                }

                int partsCount = vodPlaylist.Count;

                for (int i = 0; i < partsCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    TwitchPlaylistPart part = vodPlaylist[i];

                    if (!File.Exists(part.LocalFile))
                    {
                        warnings.Add($"Warning: missing VOD segment: {part.LocalFile}");
                        continue;
                    }

                    if (!PipeFile(part.LocalFile, stdin))
                    {
                        // ffmpeg stopped reading (e.g. reached the -t limit); remaining chunks are not needed
                        return;
                    }
                }
            }, cancellationToken);

            return warnings;
        }

        // Copies a chunk into ffmpeg's stdin and deletes it. Returns false if ffmpeg closed its input early.
        private static bool PipeFile(string file, Stream stdin)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[81920];
                int read;

                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    try
                    {
                        stdin.Write(buffer, 0, read);
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                }
            }

            FileSystem.DeleteFile(file);
            return true;
        }

        private void RunFfmpeg(Action<string> log, Action<string> setStatus, Action<double> setProgress,
            Action<bool> setIsIndeterminate, string sourceFile, string outputFile, CropInfo cropInfo,
            Action<Stream> feedStdin, CancellationToken cancellationToken = default)
        {
            bool piped = feedStdin != null;

            setStatus("Converting Video");
            setIsIndeterminate(true);

            log(Environment.NewLine + Environment.NewLine + "Executing '" + FFMPEGExe + "' on '" +
                (piped ? "VOD parts (streamed)" : sourceFile) + "'...");

            ProcessStartInfo psi = new ProcessStartInfo(FFMPEGExe)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // ffmpeg reads stdin for interactive commands; inherited garbage there corrupted conversions (#92)
            psi.ArgumentList.Add("-nostdin");
            psi.ArgumentList.Add("-y");

            if (cropInfo.CropStart)
            {
                psi.ArgumentList.Add("-ss");
                psi.ArgumentList.Add(cropInfo.Start.ToString(CultureInfo.InvariantCulture));
            }

            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add(piped ? "pipe:0" : sourceFile);
            psi.ArgumentList.Add("-analyzeduration");
            psi.ArgumentList.Add(int.MaxValue.ToString(CultureInfo.InvariantCulture));
            psi.ArgumentList.Add("-probesize");
            psi.ArgumentList.Add(int.MaxValue.ToString(CultureInfo.InvariantCulture));
            psi.ArgumentList.Add("-c:v");
            psi.ArgumentList.Add("copy");
            psi.ArgumentList.Add("-c:a");
            psi.ArgumentList.Add("copy");

            if (cropInfo.CropEnd)
            {
                psi.ArgumentList.Add("-t");
                psi.ArgumentList.Add(cropInfo.Length.ToString(CultureInfo.InvariantCulture));
            }

            psi.ArgumentList.Add(outputFile);

            log(Environment.NewLine + "Command line arguments: " + string.Join(" ", psi.ArgumentList) + Environment.NewLine);

            using (Process p = new Process())
            {
                FixedSizeQueue<string> logQueue = new FixedSizeQueue<string>(200);

                TimeSpan duration = TimeSpan.FromSeconds(cropInfo.Length);

                DataReceivedEventHandler outputDataReceived = new DataReceivedEventHandler((s, e) =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            string dataTrimmed = e.Data.Trim();

                            logQueue.Enqueue(dataTrimmed);

                            if (dataTrimmed.StartsWith("frame", StringComparison.OrdinalIgnoreCase) &&
                                duration != TimeSpan.Zero)
                            {
                                string timeStr = dataTrimmed.Substring(dataTrimmed.IndexOf("time") + 4).Trim();
                                timeStr = timeStr.Substring(timeStr.IndexOf("=") + 1).Trim();
                                timeStr = timeStr.Substring(0, timeStr.IndexOf(" ")).Trim();

                                if (TimeSpan.TryParse(timeStr, out TimeSpan current))
                                {
                                    setIsIndeterminate(false);
                                    setProgress(current.TotalMilliseconds / duration.TotalMilliseconds * 100);
                                }
                                else
                                {
                                    setIsIndeterminate(true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log(Environment.NewLine + "An error occured while reading '" + FFMPEGExe + "' output stream!" +
                            Environment.NewLine + Environment.NewLine + ex.ToString());
                    }
                });

                p.OutputDataReceived += outputDataReceived;
                p.ErrorDataReceived += outputDataReceived;
                p.StartInfo = psi;
                p.Start();
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

                if (piped)
                {
                    try
                    {
                        feedStdin(p.StandardInput.BaseStream);
                        p.StandardInput.BaseStream.Close();
                    }
                    catch
                    {
                        // canceled or failed while feeding: stop ffmpeg and drop the partial output
                        try { if (!p.HasExited) p.Kill(true); } catch { }
                        p.WaitForExit();
                        try { if (File.Exists(outputFile)) File.Delete(outputFile); } catch { }
                        throw;
                    }
                }

                p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    log(Environment.NewLine + "Video conversion complete!");
                }
                else
                {
                    if (!logQueue.IsEmpty)
                    {
                        foreach (string line in logQueue)
                        {
                            log(Environment.NewLine + line);
                        }
                    }

                    throw new ApplicationException("An error occured while converting the video!");
                }
            }
        }

        #endregion Methods
    }
}