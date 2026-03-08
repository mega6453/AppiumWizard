namespace Appium_Wizard
{
    internal static class Program
    {
        private static Mutex? mutex = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string mutexName = "AppiumWizard_SingleInstanceMutex";
            bool createdNew;

            mutex = new Mutex(true, mutexName, out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show(
                    "Appium Wizard is already running!\n\nPlease check your taskbar or task manager for the existing instance.",
                    "Application Already Running",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new LoadingScreen());
            }
            finally
            {
                // Release the mutex when application exits
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }
    }
}