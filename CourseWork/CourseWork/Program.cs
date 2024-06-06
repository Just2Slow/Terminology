namespace CourseWork
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            TermDatabase termDatabase = new TermDatabase();
            MainForm mainForm = new MainForm(termDatabase);
            Application.Run(mainForm);
        }
    }
}