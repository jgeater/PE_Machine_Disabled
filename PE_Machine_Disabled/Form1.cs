using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PE_Machine_Disabled
{
    public partial class Form1 : Form
    {
        private List<Form> blackScreens = new List<Form>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string logfile = @"C:\PKGLOG\PE_Machine_Disabled.log";
            // cmdline option setsched is specified, create the scheduled task
            if (Environment.GetCommandLineArgs().Contains("setsched"))
            {
                //copy self to c:\ebicode\PE_Machine_Disabled.exe
                string sourcePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string destinationPath = @"C:\ebicode\PE_Machine_Disabled.exe";
                try
                {
                    System.IO.File.Copy(sourcePath, destinationPath, true);
                }
                catch (Exception ex)
                {
                    // Handle the exception (e.g., log it, show a message, etc.)
                    System.Windows.MessageBox.Show("Error copying file: " + ex.Message);
                    Environment.Exit(0);
                }

                // Create the scheduled task here
                // 2 tasks one shuts the machine down 60 minutes after boot, the other one runs C:\ebicode\PE_Machine_Disabled.exe at user logon
                string taskName = "PE_Machine_Disabled";
                // Create a task to run at user logon
                try
                {
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log("Creating Scheduled task", w);
                    }
                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("PE_Machine_Disabled.PE_Machine_Disabled_Logon.xml"))
                    {
                        using (var file = new FileStream("c:\\ebicode\\PE_Machine_Disabled_Logon.xml", FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                    //now import the xml file to create the scheduled task    
                    try
                    {
                        var setsched = new ProcessStartInfo();
                        setsched.FileName = "schtasks.exe";
                        setsched.UseShellExecute = false;
                        setsched.Arguments = @"/Create /XML c:\ebicode\PE_Machine_Disabled_Logon.xml /tn PE_Machine_Disabled_Logon /F";
                        setsched.CreateNoWindow = true;
                        var p1 = Process.Start(setsched);
                        p1.WaitForExit();

                        File.Delete("c:\\ebicode\\PE_Machine_Disabled_Logon.xml");
                        using (StreamWriter w = File.AppendText(logfile))
                        {
                            Log("Done Setting schedule, exiting now", w);
                        }
                        //Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter w = File.AppendText(logfile))
                        {
                            Log("Error creating schedule.", w);
                            Log(ex.Message, w);
                        }
                    }
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log("Scheduled task created successfully", w);
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log(ex.Message, w);
                    }
                    System.Windows.MessageBox.Show(ex.Message);
                }

                //now do the shutdown task using PE_Machine_Disabled_Shutdown.xml
                try
                {
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log("Creating Scheduled task for shutdown", w);
                    }
                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("PE_Machine_Disabled.PE_Machine_Disabled_Shutdown.xml"))
                    {
                        using (var file = new FileStream("c:\\ebicode\\PE_Machine_Disabled_Shutdown.xml", FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                    //now import the xml file to create the scheduled task    
                    try
                    {
                        var setsched = new ProcessStartInfo();
                        setsched.FileName = "schtasks.exe";
                        setsched.UseShellExecute = false;
                        setsched.Arguments = @"/Create /XML c:\ebicode\PE_Machine_Disabled_Shutdown.xml /tn PE_Machine_Disabled_Shutdown /F";
                        setsched.CreateNoWindow = true;
                        var p1 = Process.Start(setsched);
                        p1.WaitForExit();
                        File.Delete("c:\\ebicode\\PE_Machine_Disabled_Shutdown.xml");
                        using (StreamWriter w = File.AppendText(logfile))
                        {
                            Log("Done Setting schedule, exiting now", w);
                        }
                        //Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter w = File.AppendText(logfile))
                        {
                            Log("Error creating schedule.", w);
                            Log(ex.Message, w);
                        }
                    }
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log("Scheduled task for shutdown created successfully", w);
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter w = File.AppendText(logfile))
                    {
                        Log(ex.Message, w);
                    }
                    System.Windows.MessageBox.Show(ex.Message);
                }

                Environment.Exit(0);

            }


            // cmdline option delsched is specified, delete the scheduled task
            if (Environment.GetCommandLineArgs().Contains("delsched"))
            {
                // Delete the scheduled task here
                string taskName = "PE_Machine_Disabled";
                System.Diagnostics.Process.Start("schtasks", $"/delete /tn \"{taskName}_Logon\" /f");
                System.Diagnostics.Process.Start("schtasks", $"/delete /tn \"{taskName}_Shutdown\" /f");

                //System.Windows.MessageBox.Show("Scheduled task deleted successfully.");
                Environment.Exit(0);
            }

            // look for PE_Machine_Disabled.txt in the current directory
            if (System.IO.File.Exists("PE_Machine_Disabled.txt"))
            {
                // read the file and display the contents in label1
                string message = System.IO.File.ReadAllText("PE_Machine_Disabled.txt");
                label1.Text = message;
            }
            else
            {
                // if the file does not exist, display a default message
                
            }


            //scale the UI elements based on screen resolution
            float fontSize = Math.Min(this.ClientSize.Width, this.ClientSize.Height) / 30f; // Adjust the divisor to change the scaling factor
            label1.Font = new Font(label1.Font.FontFamily, fontSize, FontStyle.Bold);

            //scale button1 and button2 based on screen resolution
            button1.Font = new Font(button1.Font.FontFamily, fontSize, FontStyle.Bold);
            button2.Font = new Font(button2.Font.FontFamily, fontSize, FontStyle.Bold);
            //scale the buttons to fit the font size
            button1.Width = (int)(this.ClientSize.Width * 0.3f); // 30% of the form width
            button2.Width = (int)(this.ClientSize.Width * 0.3f); // 30% of the form width
            button1.Height = (int)(fontSize * 2.5f); // Adjust height based on font size
            button2.Height = (int)(fontSize * 2.5f); // Adjust height based on font size


            button1.Visible = false;
            // Check to see if user is local admin
            // If user is local admin, show button1
            if (IsUserLocalAdmin())
            {
                button1.Visible = true;
            }
          
            //show form1 full screen anmd topmost on main screen
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Location = new Point(0, 0);
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;

            //center justify lable1, button 1 and button 2  
            label1.TextAlign = ContentAlignment.MiddleCenter;
            button1.TextAlign = ContentAlignment.MiddleCenter;
            button2.TextAlign = ContentAlignment.MiddleCenter;
            //place lable1 center screen
            label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, (this.ClientSize.Height - label1.Height) / 2);
            //place button1 below label1
            button1.Location = new Point((this.ClientSize.Width - button1.Width) / 2, label1.Bottom + 10);
            //place button2 below button1   
            button2.Location = new Point((this.ClientSize.Width - button2.Width) / 2, button1.Bottom + 10);

            // Set dispaly black screen on all secondary monitors
            foreach (Screen s in Screen.AllScreens)
            {
                if (s != Screen.PrimaryScreen)
                {
                    Form blackScreen = new Form();
                    blackScreen.FormBorderStyle = FormBorderStyle.None;
                    blackScreen.StartPosition = FormStartPosition.Manual;
                    blackScreen.Bounds = s.Bounds;
                    blackScreen.BackColor = Color.Black;
                    blackScreen.ShowInTaskbar = false;
                    blackScreen.TopMost = true;
                    blackScreen.Show();

                    blackScreens.Add(blackScreen); // Keep a reference
                }
            }
        }


        private bool IsUserLocalAdmin()
        {
            // Check if the current user is a local administrator
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //shutdown the machine
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Environment.Exit(0);
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0}: {1}: {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
        }
    }
}
