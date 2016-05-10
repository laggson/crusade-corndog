namespace LPlay.Logic
{
    public class Class1
    {
        public string GetVersion()
        {
            System.Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return ver.Major + "." + ver.Minor + "." + ver.Build;
        }
    }
}
