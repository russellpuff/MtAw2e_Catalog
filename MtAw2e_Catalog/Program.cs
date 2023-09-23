// Program
// Start end end of application execution is in this file. 

namespace MtAw2e_Catalog
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            ServerInfoForm sif = new();
            Application.Run(sif);
        }
    }
}