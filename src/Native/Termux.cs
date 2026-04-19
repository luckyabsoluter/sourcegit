using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;

using Avalonia;
using Avalonia.Controls;

namespace SourceGit.Native
{
    [SupportedOSPlatform("linux")]
    internal class Termux : OS.IBackend
    {
        public void SetupApp(AppBuilder builder)
        {
            _linux.SetupApp(builder);
        }

        public void SetupWindow(Window window)
        {
            _linux.SetupWindow(window);
        }

        public string GetDataDir()
        {
            return _linux.GetDataDir();
        }

        public string FindGitExecutable()
        {
            return _linux.FindGitExecutable();
        }

        public string FindTerminal(Models.ShellOrTerminal shell)
        {
            return _linux.FindTerminal(shell);
        }

        public List<Models.ExternalTool> FindExternalTools()
        {
            return _linux.FindExternalTools();
        }

        public void OpenTerminal(string workdir, string args)
        {
            _linux.OpenTerminal(workdir, args);
        }

        public void OpenInFileManager(string path)
        {
            var target = path;
            if (!Directory.Exists(target))
            {
                var dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                    return;

                target = dir;
            }

            if (TryStart("termux-open", target.Quoted()))
                return;

            if (TryStart("xdg-open", target.Quoted()))
                return;

            App.RaiseException(string.Empty, $"Failed to open: {target}");
        }

        public void OpenBrowser(string url)
        {
            if (TryStart("termux-open-url", url.Quoted()))
                return;

            if (TryStart("termux-open", url.Quoted()))
                return;

            var browser = Environment.GetEnvironmentVariable("BROWSER");
            if (!string.IsNullOrEmpty(browser) && TryStart(browser, url.Quoted()))
                return;

            if (TryStart("xdg-open", url.Quoted()))
                return;

            App.RaiseException(string.Empty, "Failed to open URL. Install termux-api or xdg-utils.");
        }

        public void OpenWithDefaultEditor(string file)
        {
            if (TryStart("termux-open", file.Quoted()))
                return;

            if (TryStart("xdg-open", file.Quoted()))
                return;

            App.RaiseException(string.Empty, $"Failed to open: {file}");
        }

        private static bool TryStart(string exec, string args)
        {
            if (string.IsNullOrWhiteSpace(exec))
                return false;

            try
            {
                Process.Start(exec, args);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private readonly Linux _linux = new Linux();
    }
}
