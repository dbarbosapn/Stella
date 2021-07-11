using log4net;
using log4net.Config;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Stella
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ILog _logger = LogManager.GetLogger("Stella");

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Info(" ============= STARTED LOGGING ============ ");

            base.OnStartup(e);

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in LogUnhandledException", ex);
            }
            finally
            {
                _logger.Error(message, exception);
            }
        }
    }
}
