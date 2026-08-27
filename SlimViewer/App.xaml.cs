/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        App.xaml.cs
 * PURPOSE:     Start up the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

//System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Warning | System.Diagnostics.SourceLevels.Error;

namespace SlimViewer
{
    /// <inheritdoc />
    /// <summary>
    ///     Startup SlimViewer
    /// </summary>
    internal sealed partial class App
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Exceptions thrown while WPF processes UI-thread work (event handlers,
            // command execution, bindings) surface here. This is the main safety net
            // for a desktop app - without it, an exception that escapes an event
            // handler tears down the whole process with no record of what happened.
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Exceptions on any OTHER thread (raw Task.Run work, timers, etc) that
            // would otherwise crash the process outright. By the time this fires the
            // runtime has already decided to terminate - that can't be stopped from
            // here - but the exception can be logged before it goes down.
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

            // Exceptions from "fire and forget" Tasks nobody awaited (e.g. a call like
            // `LoadThumbs(folder, file);` with no `await` or `_ =`). Without this,
            // those exceptions are silently swallowed once the Task is collected.
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogFatal("UI thread", e.Exception);

            MessageBox.Show(
                "Something went wrong and the last action couldn't be completed. Details were logged.",
                "SlimViewer",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Keeps the app alive instead of crashing - a reasonable default for a
            // viewer, where one bad file or filter shouldn't end the whole session.
            // If the same exception shows up repeatedly, treat it as a bug to fix
            // rather than something to keep swallowing: continuing blindly can leave
            // the UI in a half-updated state.
            e.Handled = true;
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogFatal("Non-UI thread (fatal)", e.ExceptionObject as Exception);
            // The process is already terminating; nothing here can stop that.
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogFatal("Unobserved task", e.Exception);
            e.SetObserved();
        }

        private static void LogFatal(string source, Exception? ex)
        {
            // TODO: this is the one wire you said you haven't connected yet - swap
            // Trace.WriteLine for your actual logger. Everything above stays the same
            // regardless of what's on the other end of this call.
            Trace.WriteLine($"[FATAL:{source}] {ex}");
        }
    }
}
