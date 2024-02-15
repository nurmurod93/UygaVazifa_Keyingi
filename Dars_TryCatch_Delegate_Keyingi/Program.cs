using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace Dars_TryCatch_Delegate_Keyingi
{
    public sealed class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = 1;

        internal delegate void Feedback(Int32 value);

        [STAThread]
        static void Main()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
            //callback function, closure, delegate
            StaticDelegateDemo();

            InstanceDelegateDemo();



            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
        private static void StaticDelegateDemo()
        {
            Console.WriteLine("------- Static Delegate Demo ------");
            Counter(1, 3, null);
            Counter(1, 3, new Feedback(Program.FeedbackToConsole));
            Counter(1, 3, new Feedback(FeedbackToMsgBox)); // "Program." - ixtiyoriy
        }
        private static void InstanceDelegateDemo()
        {
            Console.WriteLine("------- Instance Delegfate Demo ------");
            Program p = new Program();
            Counter(1, 3, new Feedback(p.FeedbackToFile));
            Console.WriteLine();
        }
        private static void ChainDelegateDemo1(Program p)
        {
            Console.WriteLine("------ Chain Delegate Demo 1");
            Feedback fb1 = new Feedback (FeedbackToConsole);
            Feedback fb2 = new Feedback (FeedbackToMsgBox);
            Feedback fb3 = new Feedback (p.FeedbackToFile);

            Feedback fbChain = null;
            fbChain = (Feedback)Delegate.Combine(fbChain, fb1);
            fbChain = (Feedback)Delegate.Combine( fbChain, fb2);
            fbChain = (Feedback)Delegate.Combine(fbChain, fb3);

            Counter(1, 2, fbChain);

            Console.WriteLine();
            fbChain = (Feedback)Delegate.Remove(fbChain, new Feedback(FeedbackToMsgBox));
            Counter(1, 2, fbChain);

        }
        private static void ChainDelegateDemo2(Program p)
        {
            Console.WriteLine("------ Chain Delegate demo 2");
            Feedback fb1 = new Feedback (FeedbackToConsole);
            Feedback fb2 = new Feedback (FeedbackToMsgBox);
            Feedback fb3 = new Feedback(p.FeedbackToFile);

            Feedback fbChain = null;
            fbChain += fb1;
            fbChain += fb2;
            fbChain += fb3;
            Counter(1, 2, fbChain);

            Console.WriteLine();
            fbChain -= new Feedback(FeedbackToMsgBox);
            Counter(1, 2, fbChain);
        }

        private static void Counter(Int32 from, Int32 to, Feedback fb) 
        {
            for(Int32 val = from; val <= to; val++)
            {
                if (fb != null)
                    fb(val); // => bf.Invoke(val);
            }
        }
        private static void FeedbackToConsole(Int32 value)
        {
            Console.WriteLine("Item = " + value);
        }
        private static void FeedbackToMsgBox(Int32 value)
        {
            MessageBox.Show("Item = " + value);
        }
        private void FeedbackToFile(Int32 value)
        {
            using (StreamWriter sw = new StreamWriter("Status", true))
            {
                sw.WriteLine("Item = " + value);
            }
        }
    }
}