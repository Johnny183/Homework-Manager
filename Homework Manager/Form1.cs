using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework_Manager
{
    public partial class homePanel : Form
    {
        // Multi Array for storing homework: Title, Due Date, Due Time, Resources, Description, Status
        List<List<string>> homeworkArray = new List<List<string>>();
        private bool mouseDown;
        private Point lastLocation;

        public homePanel()
        {
            InitializeComponent();
        }

        private void homePanel_Load(object sender, EventArgs e)
        {
            // Capture exit event to call OnApplicationExit function
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            // Open text file to read from
            if (System.IO.File.Exists("homework.txt"))
            {
                System.IO.StreamReader streamReader = new System.IO.StreamReader("homework.txt");
                string line;

                // Read each line, convert to array and add to new entry
                while ((line = streamReader.ReadLine()) != null)
                {
                    List<string> newEntry = new List<string>();
                    homeworkArray.Add(newEntry);
                    string[] dataSplit = line.Split(',');

                    foreach (string word in dataSplit)
                    {
                        if (homeworkArray.ElementAt(homeworkArray.Count - 1).Count < 7)
                        {
                            homeworkArray.ElementAt(homeworkArray.Count - 1).Add(word);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                streamReader.Close();
            }

            for(var i = 0; i < homeworkArray.Count; i++)
            {
                homeworkArray[i][6] = "temp";
            }

            refreshHomework();
        }

        /* ----------------------------------------------------------HOMEWORK DISPLAY MENU FUNCTIONS--------------------------------------------------------- */
        private void toggleHomework_Click(object sender, EventArgs e)
        {
            // Toggle display of homework menu
            if (displayHomework.Visible == true || viewHomeworkPanel.Visible == true)
            {
                toggleHomework.Text = "Return To Homework View";
                displayHomework.Visible = false;
                viewHomeworkPanel.Visible = false;
                newEntryPanel.Visible = true;
                HWDisplay.Text = "";
            } else
            {
                toggleHomework.Text = "Create New Homework Entry";
                displayHomework.Visible = true;
                newEntryPanel.Visible = false;
                viewHomeworkPanel.Visible = false;
                HWDisplay.Text = "";
                refreshHomework();

            }
        }

        private void hwView_Click(object sender, EventArgs e)
        {

            // Get button sender and display new menus
            Button cb = (Button)sender;
            viewHomeworkPanel.Visible = true;
            displayHomework.Visible = false;


            // Load entry data
            var i = Convert.ToInt32(cb.Tag);
            viewTitleBox.Text = homeworkArray[i][0];
            viewStatusBox.Text = homeworkArray[i][5];
            viewDueDateBox.Text = homeworkArray[i][1];
            viewTimeBox.Text = homeworkArray[i][2];
            viewResourcesBox.Text = homeworkArray[i][3];
            viewDescriptionBox.Text = homeworkArray[i][4];
            viewUpdateButton.Tag = i;
            viewDeleteButton.Tag = i;
            viewDueInLabel.Text = "Homework Due In:";
        }

        public void refreshHomework()
        {
            // Clears the homework panel of previous entries
            for (int i = displayHomework.Controls.Count - 1; i >= 0; i--)
            {
                if (displayHomework.Controls[i] is TextBox || displayHomework.Controls[i] is Button)
                {
                    displayHomework.Controls[i].Dispose();
                }
            }

            if (homeworkArray.Count == 0)
            {
                HWDisplay.Text = "No homework was found. Please create a new entry";
                HWDisplay.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                // Creates new entry for each element of the homeworkArray
                for (var i = 0; i < homeworkArray.Count; i++)
                {
                    var addAmount = i * 35;

                    var b = new Button();
                    b.Name = "hwView";
                    b.Click += hwView_Click;
                    b.Tag = i;
                    b.BackColor = SystemColors.Control;
                    b.ForeColor = SystemColors.ControlText;
                    b.Font = new Font(b.Font.FontFamily,9, FontStyle.Bold);
                    b.Location = new Point(3, 45 + addAmount);
                    b.Size = new System.Drawing.Size(111, 29);
                    b.Text = "View";
                    displayHomework.Controls.Add(b);

                    var v = new TextBox();
                    v.Name = "hwBoxes";
                    v.Tag = i;
                    v.Location = new Point(120, 50 + addAmount);
                    v.Size = new System.Drawing.Size(326, 20);
                    v.Text = homeworkArray[i][0];
                    v.ReadOnly = true;
                    displayHomework.Controls.Add(v);

                    v = new TextBox();
                    v.Name = "hwBoxes";
                    v.Tag = i;
                    v.Location = new Point(452, 50 + addAmount);
                    v.Size = new System.Drawing.Size(147, 20);
                    v.Text = homeworkArray[i][5];
                    v.ReadOnly = true;
                    displayHomework.Controls.Add(v);

                    v = new TextBox();
                    v.Name = "hwBoxes";
                    v.Tag = i;
                    v.Location = new Point(605, 50 + addAmount);
                    v.Size = new System.Drawing.Size(131, 20);
                    v.Text = homeworkArray[i][1];
                    v.ReadOnly = true;
                    displayHomework.Controls.Add(v);

                    v = new TextBox();
                    v.Name = "hwBoxes";
                    v.Tag = i;
                    v.Location = new Point(742, 50 + addAmount);
                    v.Size = new System.Drawing.Size(79, 20);
                    v.Text = homeworkArray[i][2];
                    v.ReadOnly = true;
                    displayHomework.Controls.Add(v);
                }
            }
        }

        /* ----------------------------------------------------------ENTRY MENU FUNCTIONS--------------------------------------------------------- */
        private void insertEntry_Click(object sender, EventArgs e)
        {

            // Time and Date validation
            if (entryDateBox.Text != "" && entryTimeBox.Text != "")
            {
                DateTime temp;
                string dateCheck = entryDateBox.Text + " " + entryTimeBox.Text;

                if (!DateTime.TryParse(dateCheck, out temp))
                {
                    // Error prompting
                    HWDisplay.Text = "You have entered an invalid date or time. Try correcting formatting.";
                    HWDisplay.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Get real time
                DateTime now = DateTime.UtcNow;
                // Remove miliseconds
                now = new DateTime((now.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
                // Subtract current time from new time
                TimeSpan difference = temp.Subtract(now);
                // Check if time is valid
                if(difference.TotalSeconds < 0)
                {
                    // Error prompting
                    HWDisplay.Text = "You have entered an invalid date or time. Try correcting formatting.";
                    HWDisplay.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            // Validate the entry boxes
            if (entryTitleBox.Text != "" && entryDateBox.Text != "" && entryTimeBox.Text != "" && entryResourcesBox.Text != "" && entryDescriptionBox.Text != "")
            {
                // Create new entry in homework array with array containing boxes text
                List<string> entry = new List<string>();
                homeworkArray.Add(entry);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add(entryTitleBox.Text);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add(entryDateBox.Text);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add(entryTimeBox.Text);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add(entryResourcesBox.Text);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add(entryDescriptionBox.Text);
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add("Not Started");
                homeworkArray.ElementAt(homeworkArray.Count - 1).Add("temp");

                // Success prompting
                HWDisplay.Text = "You have successfully created a new homework entry.";
                HWDisplay.ForeColor = System.Drawing.Color.Green;
                clearEntryTextBoxes();
            }
            else
            {
                // Error prompting
                HWDisplay.Text = "You have entered invalid information. Please try again.";
                HWDisplay.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void clearEntryBoxes_Click(object sender, EventArgs e)
        {
            clearEntryTextBoxes();
        }

        public void clearEntryTextBoxes()
        {
            // Loops through the textboxes and clears them of any content.
            for (int i = newEntryPanel.Controls.Count - 1; i >= 0; i--)
            {
                if (newEntryPanel.Controls[i] is TextBox)
                {
                    newEntryPanel.Controls[i].Text = "";
                }
            }
        }

        /* ----------------------------------------------------------VIEW MENU FUNCTIONS--------------------------------------------------------- */
        private void viewBackButton_Click(object sender, EventArgs e)
        {
            viewHomeworkPanel.Visible = false;
            displayHomework.Visible = true;
            HWDisplay.Text = "";
            refreshHomework();
        }

        private void viewUpdateButton_Click(object sender, EventArgs e)
        {
            Button cb = (Button)sender;
            if (viewTitleBox.Text != "" && viewDueDateBox.Text != "" && viewTimeBox.Text != "" && viewResourcesBox.Text != "" && viewDescriptionBox.Text != "" && viewStatusBox.Text != "")
            {

                var i = Convert.ToInt32(cb.Tag);
                homeworkArray[i][0] = viewTitleBox.Text;
                homeworkArray[i][1] = viewDueDateBox.Text;
                homeworkArray[i][2] = viewTimeBox.Text;
                homeworkArray[i][3] = viewResourcesBox.Text;
                homeworkArray[i][4] = viewDescriptionBox.Text;
                homeworkArray[i][5] = viewStatusBox.Text;

                // Success prompting
                HWDisplay.Text = "You have successfully updated the homework entry.";
                HWDisplay.ForeColor = System.Drawing.Color.Green;
            } else
            {
                // Error prompting
                HWDisplay.Text = "You have entered invalid information. Please try again.";
                HWDisplay.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void viewDeleteButton_Click(object sender, EventArgs e)
        {
            Button cb = (Button)sender;
            var i = Convert.ToInt32(cb.Tag);
            homeworkArray.RemoveAt(i);
            HWDisplay.Text = "Entry has successfully been deleted.";
            HWDisplay.ForeColor = System.Drawing.Color.Green;

            viewHomeworkPanel.Visible = false;
            displayHomework.Visible = true;
            refreshHomework();
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            // Open text file or create one to write to
            System.IO.File.WriteAllText("homework.txt", "");
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("homework.txt");
            string output = "";

            // Loop through array and convert each entry to a single string then write to file
            for (int i = 0; i < homeworkArray.Count; i++)
            {
                for (int j = 0; j < homeworkArray[i].Count; j++)
                {
                    output += homeworkArray[i][j].ToString() + ",";
                }
                streamWriter.WriteLine(output);
                output = "";
            }
            streamWriter.Close();

            notifyIcon1.Icon = null;
            notifyIcon1.Dispose();
            notifyIcon1 = null;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkDueTime_Tick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                for(var i = 0; i < homeworkArray.Count; i++)
                {
                    DateTime temp;
                    string dateCheck = homeworkArray[i][1] + " " + homeworkArray[i][2];

                    if (DateTime.TryParse(dateCheck, out temp))
                    {
                        DateTime now = DateTime.UtcNow;
                        now = new DateTime((now.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
                        TimeSpan difference = temp.Subtract(now);

                        // 24 HOURS NOTIFICATION
                        if(difference.TotalMinutes >= 1380 && difference.TotalMinutes <= 1440)
                        {
                            if (homeworkArray[i][6] != "24hours")
                            {
                                notifyIcon1.ShowBalloonTip(1000, "Important notice", "Homework: " + homeworkArray[i][0] + " is due within 24 hours.", ToolTipIcon.Info);
                                notifyIcon1.Tag = i;
                                homeworkArray[i][6] = "24hours";
                                break;
                            }
                        }

                        // 12 HOURS NOTIFICATION
                        else if(difference.TotalMinutes >= 720 && difference.TotalMinutes <= 780)
                        {
                            if(homeworkArray[i][6] != "12hours")
                            {
                                notifyIcon1.ShowBalloonTip(1000, "Important notice", "Homework: " + homeworkArray[i][0] + " is due within 12 hours.", ToolTipIcon.Info);
                                notifyIcon1.Tag = i;
                                homeworkArray[i][6] = "12hours";
                                break;
                            }
                        }

                        // 6 HOURS NOTIFICATION
                        else if (difference.TotalMinutes >= 360 && difference.TotalMinutes <= 420)
                        {
                            if (homeworkArray[i][6] != "6hours")
                            {
                                notifyIcon1.ShowBalloonTip(1000, "Important notice", "Homework: " + homeworkArray[i][0] + " is due within 6 hours.", ToolTipIcon.Info);
                                notifyIcon1.Tag = i;
                                homeworkArray[i][6] = "6hours";
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            var id = notifyIcon1.Tag;
            viewHomeworkPanel.Visible = true;
            displayHomework.Visible = false;


            // Load entry data
            var i = Convert.ToInt32(id);
            viewTitleBox.Text = homeworkArray[i][0];
            viewStatusBox.Text = homeworkArray[i][5];
            viewDueDateBox.Text = homeworkArray[i][1];
            viewTimeBox.Text = homeworkArray[i][2];
            viewResourcesBox.Text = homeworkArray[i][3];
            viewDescriptionBox.Text = homeworkArray[i][4];
            viewUpdateButton.Tag = i;
            viewDeleteButton.Tag = i;
            viewDueInLabel.Text = "Homework Due In:";
        }

        private void homeworkDueRefresh_Tick(object sender, EventArgs e)
        {
            if(viewHomeworkPanel.Visible == true)
            {
                DateTime temp;
                string dateCheck = viewDueDateBox.Text + " " + viewTimeBox.Text;

                if (DateTime.TryParse(dateCheck, out temp))
                {
                    DateTime now = DateTime.UtcNow;
                    now = new DateTime((now.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
                    TimeSpan difference = temp.Subtract(now);

                    if (difference.TotalDays > 0)
                    {
                        viewDueInLabel.Text = "Homework Due In: " + difference.ToString("%d") + " day(s) and " + difference.ToString("%h") + " hour(s)";
                    }
                    else if(difference.TotalHours > 0)
                    {
                        viewDueInLabel.Text = "Homework Due In: " + difference.ToString("%h") + " hour(s) and " + difference.ToString("%m") + " minutes(s)";
                    }
                    else if(difference.TotalMinutes > 0)
                    {
                        viewDueInLabel.Text = "Homework Due In: " + difference.ToString("%m") + " minutes(s) and " + difference.ToString("%s") + " seconds(s)";
                    }
                    else
                    {
                        viewDueInLabel.Text = "Homework Due In: LATE";
                    }
               }
               else
               {
                viewDueInLabel.Text = "Homework Due In: Unknown";
               }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        private void homePanel_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void homePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void homePanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
