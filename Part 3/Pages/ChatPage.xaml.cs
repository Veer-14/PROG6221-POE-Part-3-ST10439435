using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Part_3.Pages
{
    public partial class ChatPage : Page
    {
        Dictionary<string, string> userMemory = new Dictionary<string, string>();
        string currentTopic = "";
        string pendingTaskTitle = null;
        string pendingTaskDescription = null;
        bool waitingForDescription = false;
        Random random = new Random();

        List<CyberTask> taskList = new List<CyberTask>();

        Dictionary<string, string[]> randomResponses = new Dictionary<string, string[]>
        {
            { "phishing tips", new[] { "Phishing emails often look urgent...", "Don't click suspicious links...", "Look out for grammar errors..." } },
            { "password tips", new[] { "Use a passphrase...", "Avoid using the same password...", "Use a password manager..." } },
            { "data tips", new[] { "Don't share personal details...", "Encrypt sensitive files...", "Be cautious on social media..." } },
            { "scam tips", new[] { "If it sounds too good...", "Never share OTPs...", "Scammers may pretend..." } },
            { "malware tips", new[] { "Avoid unknown attachments...", "Update antivirus...", "Use a firewall..." } },
            { "privacy tips", new[] { "Review app permissions...", "Use incognito mode or VPN...", "Disable location sharing..." } }
        };

        Dictionary<string, string> sentimentResponses = new Dictionary<string, string>
        {
            { "worried", "It's completely understandable..." },
            { "frustrated", "I understand your frustration..." },
            { "curious", "That's a great attitude!" }
        };

        Dictionary<string, string> keywordMap = new Dictionary<string, string>
        {
            { "password", "password tips" },
            { "phishing", "phishing tips" },
            { "data", "data tips" },
            { "scam", "scam tips" },
            { "malware", "malware tips" },
            { "privacy", "privacy tips" }
        };

        public ChatPage()
        {
            InitializeComponent();
            GreetUser();
        }

        private async void GreetUser()
        {
            AppendText("ChatBot: Welcome to your Cybersecurity Awareness Assistant!\n");
            await Task.Delay(800);
            AppendText("ChatBot: What’s your name?\n");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            string lowerInput = input.ToLower();
            UserInput.Text = "";

            if (string.IsNullOrWhiteSpace(input)) return;

            AppendText($"\nYou: {input}\n");

            // Step 1: Capture user name
            if (!userMemory.ContainsKey("name"))
            {
                userMemory["name"] = input;
                AppendText($"ChatBot: Nice to meet you, {input}! What's your favorite cybersecurity topic?\n");
                return;
            }

            // Step 2: Capture favorite topic
            if (!userMemory.ContainsKey("interest"))
            {
                userMemory["interest"] = input;
                AppendText($"ChatBot: Got it! You like {input}. You can now ask for tips or add tasks!\n");
                return;
            }

            // Exit
            if (lowerInput == "exit")
            {
                AppendText("ChatBot: Stay safe! Goodbye.\n");
                Application.Current.Shutdown();
                return;
            }

            // Step 3: Handle description if waiting
            if (waitingForDescription && pendingTaskTitle != null)
            {
                pendingTaskDescription = input;
                waitingForDescription = false;

                taskList.Add(new CyberTask
                {
                    Title = pendingTaskTitle,
                    Description = pendingTaskDescription,
                    ReminderDate = null
                });

                AppendText($"ChatBot: Description saved for '{pendingTaskTitle}'. Would you like to set a reminder? (e.g., Remind me in 3 days)\n");
                return;
            }

            // View tasks
            if (lowerInput == "view tasks" || lowerInput.Contains("show tasks"))
            {
                if (taskList.Count == 0)
                {
                    AppendText("ChatBot: You have no tasks yet.\n");
                }
                else
                {
                    AppendText("ChatBot: Here's a list of your tasks:\n");
                    foreach (var task in taskList)
                    {
                        string reminder = task.ReminderDate.HasValue
                            ? $" (Reminder: {task.ReminderDate.Value.ToShortDateString()})"
                            : "";
                        AppendText($"- {task.Title}: {task.Description}{reminder}\n");
                    }
                }
                return;
            }

            // Add task
            if (lowerInput.StartsWith("add task"))
            {
                string taskTitle = input.Substring(8).Trim();
                if (string.IsNullOrEmpty(taskTitle))
                {
                    AppendText("ChatBot: Please provide a task title after 'add task'.\n");
                    return;
                }

                pendingTaskTitle = taskTitle;
                pendingTaskDescription = null;
                waitingForDescription = true;

                AppendText($"ChatBot: Task '{taskTitle}' created. Please provide a description for this task.\n");
                return;
            }

            // Set reminder
            if (lowerInput.StartsWith("remind me in") && pendingTaskTitle != null)
            {
                Match match = Regex.Match(lowerInput, @"remind me in (\d+) (day|days|week|weeks)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int number = int.Parse(match.Groups[1].Value);
                    string unit = match.Groups[2].Value;

                    DateTime reminderDate = DateTime.Now;
                    if (unit.StartsWith("day"))
                        reminderDate = reminderDate.AddDays(number);
                    else if (unit.StartsWith("week"))
                        reminderDate = reminderDate.AddDays(number * 7);

                    var task = taskList.Find(t => t.Title == pendingTaskTitle);
                    if (task != null)
                    {
                        task.ReminderDate = reminderDate;
                        AppendText($"ChatBot: Reminder set for {reminderDate.ToShortDateString()}.\n");
                    }

                    pendingTaskTitle = null;
                    return;
                }

                AppendText("ChatBot: I couldn't understand the reminder format. Try 'Remind me in 3 days'.\n");
                return;
            }

            // Sentiment
            foreach (var feeling in sentimentResponses)
            {
                if (lowerInput.Contains(feeling.Key))
                {
                    AppendText($"ChatBot: {sentimentResponses[feeling.Key]}\n");
                    return;
                }
            }

            // Tips
            foreach (var keyword in keywordMap)
            {
                if (lowerInput.Contains(keyword.Key))
                {
                    currentTopic = keyword.Value;
                    string[] tips = randomResponses[keyword.Value];
                    string selectedTip = tips[random.Next(tips.Length)];
                    AppendText($"ChatBot: {selectedTip}\n");
                    return;
                }
            }

            AppendText("ChatBot: I'm not sure how to help with that. Try 'add task', 'view tasks', or ask for tips like 'phishing'.\n");
        }

        private void AppendText(string message)
        {
            ChatOutput.Text += message;
        }

        public class CyberTask
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
        }
    }
}
