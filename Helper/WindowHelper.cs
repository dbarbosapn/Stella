using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Stella.Helper
{
    public class WindowHelper
    {
        public static WindowHelper Instance {
            get {
                if (_instance == null)
                    _instance = new WindowHelper();
                return _instance;
            }
        }
        private static WindowHelper _instance;

        public Frame LoadingFrame = null;
        public Frame MainFrame = null;
        public Frame LoadingContentFrame = null;
        public Snackbar BottomSnackbar;

        /// <summary>
        /// Shows or hides the loading frame
        /// </summary>
        /// <param name="v">Boolean that defines if we stop or start loading</param>
        /// <param name="content">Defines if the loading page to show/hide is the content page</param>
        public void SetLoading(bool v, bool content = false)
        {
            if(!content)
            {
                if (LoadingFrame != null)
                    LoadingFrame.Visibility = v ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                if (LoadingContentFrame != null)
                    LoadingContentFrame.Visibility = v ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// Sets the content of the main frame
        /// </summary>
        /// <param name="content">Content page to display</param>
        public void SetContent(Page content)
        {
            MainFrame.Content = content;
        }

        /// <summary>
        /// Shows a snackbar with given content
        /// </summary>
        /// <param name="content">Content to display</param>
        public void ShowSnackbar(string content)
        {
            BottomSnackbar.MessageQueue.Enqueue(content);
        }

        /// <summary>
        /// Runs Action on UI Thread
        /// </summary>
        /// <param name="a">Action to run</param>
        public void RunOnUIThread(Action a)
        {
            Application.Current.Dispatcher.Invoke(a);
        }
    }
}
