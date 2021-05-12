using System.IO;

namespace Cowin_Slot_Notification
{
    class Logger
    {
        public static void Log(string text)
        {
            var currDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = "ExceptionLog.txt";
            var exceptionFile = Path.Combine(currDir, fileName);

            if (File.Exists(exceptionFile) == false)
            {
                File.Create(exceptionFile);
            }
            using (StreamWriter sw = File.AppendText(exceptionFile))
            {
                sw.WriteLine(text + " - " + System.DateTime.Now.ToString());
                sw.Close();
            }
        }
    }
}
