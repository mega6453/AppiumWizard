namespace Appium_Wizard
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new Object_Spy(5555, 390, 844));
            //var size = iOSAPIMethods.GetScreenSize(5555);
            //Application.Run(new Object_Spy("ios", 5555, size.Item1, size.Item2));

            Application.Run(new LoadingScreen());
        }
    }
}