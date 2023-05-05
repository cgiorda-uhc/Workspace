using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UCS_Automation_Manager
{
    public class AsyncSampleWindowsService
    {
        //https://msdn.microsoft.com/en-us/library/dd997396(v=vs.110).aspx

        private static bool _stateRunning = false;
        private static CancellationTokenSource _tokenSource;
        private Task t;
        private CancellationToken token;

        //The worker
        public async Task<bool> DoWork(CancellationToken token)
        {
            Debug.WriteLine("DoWork() 1");

            // Were we already canceled?
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {
                Debug.WriteLine("DoWork() 2");
                await Task.Delay(1000); //Simulate work

                //time to stop?
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Task cancelled");
                    _stateRunning = false;
                }
            }
            return true; //avoiding async void
        }

        public void OnStart(string[] args)
        {

            if (!_stateRunning)
            {
                Debug.WriteLine("OnStart()");
                _stateRunning = true;
                _tokenSource = new CancellationTokenSource();
                token = _tokenSource.Token;

                try
                {
                    // Request cancellation of a single task when the token source is canceled. 
                    // Pass the token to the user delegate, and also to the task so it can  
                    // handle the exception correctly.
                    t = Task.Factory.StartNew(() => DoWork(token), token);
                    Debug.WriteLine("Task {0} executing", t.Id);
                    t.Wait(); //start task and don't await task to finish
                }
                catch (Exception e)
                {
                    //Expecting 
                    Debug.WriteLine("OnStart - Exception: {0}", e.GetType().Name);
                }
                finally
                {
                    Debug.WriteLine("OnStart - finally");
                }
            }

        }

        public void OnStop()
        {
            Debug.WriteLine("OnStop()");

            if (_stateRunning)
            {
                //SetDoStop
                try
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                        Debug.WriteLine("Task cancellation requested.");

                    }
                }
                catch (Exception e)
                {
                    //Expecting 
                    Debug.WriteLine("OnStop - Exception: {0}", e.GetType().Name);
                }
                finally
                {
                    Debug.WriteLine("OnStop - finally");
                    if (_tokenSource != null)
                    {
                        _tokenSource.Dispose();
                        Debug.WriteLine("_tokenSource.Disposed");
                    }
                }
            }
        }

    }



    public class AsyncSample
    {
        static void Main(string[] args)
        {
            Go();
        }
        public static void Go()
        {
            GoAsync();
            Console.ReadLine();
        }
        public static async void GoAsync()
        {

            Console.WriteLine("Starting");

            var task1 = Sleep(5000);
            var task2 = Sleep(3000);

            int[] result = await Task.WhenAll(task1, task2);

            Console.WriteLine("Slept for a total of " + result.Sum() + " ms");

            //This is a great answer... but I thought this wrong was answer until I ran it. then I understood. It really does execute in 5 seconds. The trick is to NOT await the tasks immediately, instead await on Task.WhenAll.

        }

        private async static Task<int> Sleep(int ms)
        {
            Console.WriteLine("Sleeping for {0} at {1}", ms, Environment.TickCount);
            await Task.Delay(ms);
            Console.WriteLine("Sleeping for {0} finished at {1}", ms, Environment.TickCount);
            return ms;
        }

        ////OR
        ///async Task<int> LongTask1() { 
            //  ...
            //  return 0; 
            //}

            //    async Task<int> LongTask2()
            //    { 
            //  ...
            //  return 1;
            //    }

            //...
            //{
            //   Task<int> t1 = LongTask1();
            //    Task<int> t2 = LongTask2();
            //    await Task.WhenAll(t1, t2);
            //    //now we have t1.Result and t2.Result
            //}




    }






}
