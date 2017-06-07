using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample06
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        public void Run()
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = cts.Token;
                Console.WriteLine("\nStart Run");
                Task t = Task.Run(() => DoCancel(cts), cts.Token);
                Console.WriteLine("\nParallel.Invoke");
                Parallel.Invoke(po,
                                () => Do01(po.CancellationToken),
                                () => Do02(po.CancellationToken),
                                () => Do03(po.CancellationToken));
            }
            catch (AggregateException aex)
            {
                Console.WriteLine("\nAggregateException in Run: " + aex.Message);
                aex.Flatten();
                foreach (Exception ex in aex.InnerExceptions)
                    Console.WriteLine("  Exception: " + ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("\nOperationCanceledException in Run:\n" + ex.Message);
            }
            finally
            {
                Console.WriteLine("\nEnd Run");
                Console.ReadLine();
            }
        }
        private void DoCancel(CancellationTokenSource cts)
        {
            // cts.Cancel(); // this will invoke the OperationCanceledException in Run()
            Console.WriteLine("Cancel in 2sec");
            cts.CancelAfter(2000);
        }
        private void Do01(CancellationToken ct)
        {
            Console.WriteLine("Start Do01");
            try
            {
                Thread.Sleep(1000);  // doing some work
                int a = 1;
                a = a / --a; // create an exception.
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Do01:\n" + ex);
                throw;
            }
            finally
            {
                Console.WriteLine("\nEnd Do01");
            }
        }
        private void Do02(CancellationToken ct)
        {
            Console.WriteLine("Start Do02");
            try
            {
                for (int i = 1; i < 30; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(100); // doing some work
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Do02:\n" + ex);
                throw;
            }
            finally
            {
                Console.WriteLine("\nEnd Do02");
            }
        }
        private void Do03(CancellationToken ct)
        {
            Console.WriteLine("Start Do03");
            try
            {
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Do03:\n" + ex);
                throw;
            }
            finally
            {
                Console.WriteLine("\nEnd Do03");
            }
        }
    }
}
