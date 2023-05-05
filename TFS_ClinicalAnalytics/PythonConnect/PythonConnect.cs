
using Microsoft.Scripting.Hosting;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonConnect
{
    public class PythonConnect
    {


        public static void runPythonCmd(string cmd = null, string args = null)
        {


            //set PYTHONPATH=%PYTHONPATH%;C:\My_python_lib
            //            import sys



            //#sys.path.append("C:\\where-ever")



            //print(sys.path)

            //ProcessStartInfo start = new ProcessStartInfo();
            //start.FileName = @"C:\Python36\python.exe";
            //start.Arguments = string.Format("{0} {1}", cmd, args);
            //start.UseShellExecute = false;
            //start.RedirectStandardOutput = true;
            //using (Process process = Process.Start(start))
            //{
            //    using (StreamReader reader = process.StandardOutput)
            //    {
            //        string result = reader.ReadToEnd();
            //        Console.Write(result);
            //    }
            //}
            //https://stackoverflow.com/questions/27381264/python-3-4-how-to-import-a-module-given-the-full-path


            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Python36\python.exe";
            //start.WorkingDirectory = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\venv\Lib\site-packages";
            //start.Arguments = string.Format("D:\\script\\test.py -a {0} -b {1} ", "some param", "some other param");
            start.Arguments = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\GetCMSFile_20220111.py";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }





        }




        public static void runPythonEngine(string strPathToPythonScript = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\GetCMSFile_20220111.py", string strPathToPythonLibs = @"C:\Python36\Lib", string strPathToPythonPackages = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\venv\Lib\site-packages")
        {
            //var pythonPath = @"C:\Users\Admin\anaconda3";

            //Environment.SetEnvironmentVariable("PATH", $@"{pythonPath};" + Environment.GetEnvironmentVariable("PATH"));
            //Environment.SetEnvironmentVariable("PYTHONHOME", pythonPath);
            //Environment.SetEnvironmentVariable("PYTHONPATH ", $@"{pythonPath}\Lib");

            //string scriptFile = "myfunction.py";
            //string pythonCode = "";
            //using (var streamReader = new StreamReader(scriptFile, Encoding.UTF8))
            //{
            //    pythonCode = streamReader.ReadToEnd();
            //}

            //using (Py.GIL())
            //{
            //    var scope = Py.CreateScope();
            //    scope.Exec(pythonCode);
            //    greyImage1 = (scope as dynamic).binarise(greyImage1);
            //    pictureBox1.Image = (System.Drawing.Image)greyImage1;
            //    this.Cursor = Cursors.Default;
            //}
        }



        //PYTHON 2.7
        //public static void runPythonEngine(string strPathToPythonScript = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\GetCMSFile_20220111.py", string strPathToPythonLibs = @"C:\Python36\Lib", string strPathToPythonPackages = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\venv\Lib\site-packages")
        //{
        //    ScriptEngine engine = Python.CreateEngine();

        //    ICollection<string> searchPaths = engine.GetSearchPaths();
        //    searchPaths.Add(strPathToPythonLibs);
        //    searchPaths.Add(strPathToPythonPackages);
        //    engine.SetSearchPaths(searchPaths);



        //    ScriptScope scope = engine.CreateScope();
        //    //scope.Add = new Func<int, int, int>((x, y) => x + y);
        //    //Console.WriteLine(scope.Add(2, 3)); // prints 5


        //    // execute the script
        //    engine.ExecuteFile(strPathToPythonScript);

        //    // execute and store variables in scope
        //    engine.ExecuteFile(strPathToPythonScript, scope);


        //    // variables and functions defined in the scrip are added to the scope
        //    //scope.SomeFunction();
        //    dynamic testFunction = scope.GetVariable("test_func");
        //    int var1 = 1;
        //    int var2 = 2;
        //    var result = testFunction(var1, var2);




        //}



    }

}
