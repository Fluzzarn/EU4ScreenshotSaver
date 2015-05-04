using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace EU4ScreenshotSaver
{
    public partial class Form1 : Form
    {
        int picCounter = 0;
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                textBox1.Text = file;
                string path = Path.GetFullPath(file);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = folderBrowserDialog1.SelectedPath;
                textBox2.Text = file;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //Runspace runspace = RunspaceFactory.CreateRunspace();
            //runspace.Open();
            //PowerShell ps = PowerShell.Create();
            //ps.Runspace = runspace;
            //ps.AddScript((Directory.GetCurrentDirectory() + "\\timelapse.ps1")).AddScript("Start-TimeLapse").AddParameters(new Dictionary<string, string>() { { "exe", textBox1.Text }, { "savedir", textBox2.Text }, { "waiting_time", numericUpDown1.Value.ToString() } });
            //runspace.SessionStateProxy.SetVariable("TextForm", this);
            //ps.Invoke(); 
            
            //Pipeline pipeline = runspace.CreatePipeline();

            
            

                Console.WriteLine(Directory.GetCurrentDirectory() + "\\timelapse.ps1");
                ProcessStartInfo psi = new ProcessStartInfo();

                System.Diagnostics.Process.Start(textBox1.Text);
               // psi.Arguments = ("-command \"& { . " +"\'"+Directory.GetCurrentDirectory()+ "\\timelapse.ps1\'; StartTimeLapse \'" + textBox1.Text + "\' \'" + textBox2.Text + "\' " + numericUpDown1.Value + "}\"");

                System.Timers.Timer screenTimer = new System.Timers.Timer((double)numericUpDown1.Value * 1000.0);
                screenTimer.Elapsed += new System.Timers.ElapsedEventHandler(TakeScreenShot);
                screenTimer.AutoReset = false;
                screenTimer.Enabled = true;
            


                
                
            
        }

        private void TakeScreenShot(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer screenTimer = new System.Timers.Timer((double)numericUpDown1.Value * 1000.0);
            screenTimer.Elapsed += new System.Timers.ElapsedEventHandler(TakeScreenShot);
            screenTimer.AutoReset = false;
            screenTimer.Enabled = true;
            IntPtr WindowToFind = FindWindow(null, "Europa Universalis IV");
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x101;
            const uint WM_SYSCOMMAND = 0x018;
            const uint SC_CLOSE = 0x053;
            IntPtr result3 = SendMessage(WindowToFind, WM_KEYDOWN, ((IntPtr)Keys.F10), (IntPtr)0);
            IntPtr result2 = SendMessage(WindowToFind, WM_KEYUP, ((IntPtr)Keys.F10), (IntPtr)0);
            Console.WriteLine("SHOT TAKEN");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            picCounter = 0;
            string[] files = Directory.GetFiles(textBox2.Text);
            string[] firstHalf = files.Take(files.Length / 2).ToArray();
            string[] secondHalf = files.Skip(files.Length / 2).ToArray();
            Thread resizeThread1 = new Thread(() => resizeImages(firstHalf));
            resizeThread1.Start();

            //Thread resizeThread2 = new Thread(() => resizeImages(secondHalf));
            //resizeThread2.Start();

        }

        private void resizeImages(string[] file)
        {
            string[] files = Directory.GetFiles(textBox2.Text);
            
            Image tempImage;
            foreach (var image in files)
            {
                try
                {
                    using (Image picture = Image.FromFile(image))
                    {
                        //-16 -13
                        tempImage = (Image)(new Bitmap(picture, new Size(3942, 1434)));
                        Graphics textBurn = Graphics.FromImage(tempImage);
                        string year = image.Substring(image.Length - 16,4);
                        
                        textBurn.DrawString(year, new Font(FontFamily.GenericMonospace,72), Brushes.Black, 0, 1300);
                        
                        tempImage.Save(textBox2.Text + "\\" + picCounter + ".png");
                        picCounter++;
                        textBox3.Invoke(new Action(delegate() { textBox3.AppendText( "\n Resized Image:" + picCounter + '\n'); }));
                        tempImage.Dispose();
                        picture.Dispose();
                        Thread.Sleep(16);
                    }

                }
                catch (Exception e)
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    Console.WriteLine(e.ToString());
                }
            }

            textBox3.Invoke(new Action(delegate() { textBox3.AppendText("\n Done on Thread:" + Thread.CurrentThread.ManagedThreadId.ToString() + '\n'); }));
        }
    }
}
