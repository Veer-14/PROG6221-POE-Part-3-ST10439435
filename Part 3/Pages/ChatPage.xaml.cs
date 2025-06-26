using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
        List<string> activityLog = new List<string>();

        Dictionary<string, string[]> randomResponses = new Dictionary<string, string[]>
{
    { "phishing tips", new[] {
        "Phishing emails often use urgency like 'Your account will be suspended' to make you panic and click. Always verify the sender.",
        "Before clicking a link, hover over it to see the true URL. If it looks suspicious or unfamiliar, don’t click it.",
        "Many phishing emails have poor grammar or strange formatting. Legitimate companies usually don’t make such mistakes."
    }},
    { "password tips", new[] {
        "Create a strong password using a passphrase like 'SunsetsAre@Beautiful123'. It's easy to remember and hard to guess.",
        "Avoid reusing the same password across multiple accounts. If one account is compromised, others can be too.",
        "Use a reputable password manager to store and generate complex passwords securely."
    }},
    { "data tips", new[] {
        "Be cautious about what you share online. Even your birthdate or pet’s name can be used to guess passwords.",
        "When using public Wi-Fi, avoid entering personal data unless you're on a VPN or secure connection.",
        "Encrypt sensitive files before uploading them to cloud storage. This adds another layer of protection."
    }},
    { "scam tips", new[] {
        "If something sounds too good to be true—like winning a prize you never entered—it’s likely a scam.",
        "Scammers often impersonate banks or government officials. Always contact the organization directly to verify.",
        "Never share your OTP, PIN, or password—even if the request seems official or urgent."
    }},
    { "malware tips", new[] {
        "Avoid downloading files or apps from untrusted sources. Always verify the origin of downloads.",
        "Keep your operating system and software updated. Security patches fix vulnerabilities malware can exploit.",
        "Install and regularly update antivirus and anti-malware tools. Set them to run scheduled scans automatically."
    }},
    { "privacy tips", new[] {
        "Check and limit app permissions regularly. Many apps request access to unnecessary personal data.",
        "Use privacy-focused search engines and browser extensions to limit tracking and data collection.",
        "When possible, use two-factor authentication and avoid linking multiple services to one login (e.g., Google or Facebook sign-ins)."
    }}
};

        Dictionary<string, string> sentimentResponses = new Dictionary<string, string>
{
    { "worried", "It's okay to feel worried. Cybersecurity can seem complex, but every small step you take strengthens your safety online. I'm here to guide you." },
    { "frustrated", "Frustration is part of learning something new. Let’s tackle one topic at a time and build your confidence in staying secure online." },
    { "curious", "Curiosity is a great trait when it comes to cybersecurity! Ask me about passwords, scams, malware, and more—I’ve got lots to share!" }
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
            await System.Threading.Tasks.Task.Delay(800);
            AppendText("ChatBot: What’s your name?\n");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            string lowerInput = input.ToLower();
            UserInput.Text = "";

            if (string.IsNullOrWhiteSpace(input)) return;

            AppendText($"\nYou: {input}\n");

            // Step 1: Capture Name
            if (!userMemory.ContainsKey("name"))
            {
                userMemory["name"] = input;
                AppendText($"ChatBot: Nice to meet you, {input}! What's your favorite cybersecurity topic?\n");
                return;
            }

            // Step 2: Capture Interest
            if (!userMemory.ContainsKey("interest"))
            {
                userMemory["interest"] = input;
                AppendText($"ChatBot: Got it! You like {input}. You can now ask for tips or add tasks!\n");
                return;
            }

            // Step 3: Handle task description
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

                LogActivity($"Task added: '{pendingTaskTitle}' with description.");
                AppendText($"ChatBot: Description saved for '{pendingTaskTitle}'. Would you like to set a reminder? (e.g., Remind me in 3 days)\n");
                return;
            }

            // Step 4: Reminder NLP
            Regex fullRemindRegex = new Regex(@"remind me( to)? (.+?) in (\d+) (day|days|week|weeks)", RegexOptions.IgnoreCase);
            Regex simpleRemindRegex = new Regex(@"remind me in (\d+) (day|days|week|weeks)", RegexOptions.IgnoreCase);

            Match fullMatch = fullRemindRegex.Match(lowerInput);
            Match simpleMatch = simpleRemindRegex.Match(lowerInput);

            if (fullMatch.Success)
            {
                string action = fullMatch.Groups[2].Value.Trim();
                int number = int.Parse(fullMatch.Groups[3].Value);
                string unit = fullMatch.Groups[4].Value;

                DateTime reminderDate = unit.StartsWith("day") ? DateTime.Now.AddDays(number) : DateTime.Now.AddDays(number * 7);
                string taskTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action.Length > 30 ? action.Substring(0, 30) : action);

                taskList.Add(new CyberTask
                {
                    Title = taskTitle,
                    Description = action,
                    ReminderDate = reminderDate
                });

                LogActivity($"Reminder task created: '{taskTitle}' on {reminderDate.ToShortDateString()}.");
                AppendText($"ChatBot: New task '{taskTitle}' added with a reminder for {reminderDate.ToShortDateString()}.\n");
                return;
            }
            else if (simpleMatch.Success && pendingTaskTitle != null)
            {
                int number = int.Parse(simpleMatch.Groups[1].Value);
                string unit = simpleMatch.Groups[2].Value;
                DateTime reminderDate = unit.StartsWith("day") ? DateTime.Now.AddDays(number) : DateTime.Now.AddDays(number * 7);

                var task = taskList.Find(t => t.Title == pendingTaskTitle);
                if (task != null)
                {
                    task.ReminderDate = reminderDate;
                    LogActivity($"Reminder set for task: '{task.Title}' on {reminderDate.ToShortDateString()}.");
                    AppendText($"ChatBot: Reminder set for {reminderDate.ToShortDateString()}.\n");
                }
                else
                {
                    AppendText("ChatBot: I couldn't find the task to set a reminder on.\n");
                }

                pendingTaskTitle = null;
                return;
            }

            // Step 5: View Tasks
            if (Regex.IsMatch(lowerInput, @"\b(view|show|list)\b.*\b(task|tasks|reminders|todos)\b"))
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
                        string reminder = task.ReminderDate.HasValue ? $" (Reminder: {task.ReminderDate.Value.ToShortDateString()})" : "";
                        AppendText($"- {task.Title}: {task.Description}{reminder}\n");
                    }
                }
                return;
            }

            // Step 6: Delete Task
            if (Regex.IsMatch(lowerInput, @"\b(delete|remove)\b.*\b(task|reminder|todo)\b"))
            {
                string keywordToDelete = ExtractKeywordAfterDelete(lowerInput);
                if (string.IsNullOrEmpty(keywordToDelete))
                {
                    AppendText("ChatBot: Please tell me which task to delete (e.g., 'delete task update password').\n");
                    return;
                }

                var taskToRemove = taskList.Find(t => t.Title.ToLower().Contains(keywordToDelete.ToLower()));
                if (taskToRemove != null)
                {
                    taskList.Remove(taskToRemove);
                    LogActivity($"Task deleted: '{taskToRemove.Title}'.");
                    AppendText($"ChatBot: Task '{taskToRemove.Title}' has been deleted.\n");
                }
                else
                {
                    AppendText($"ChatBot: I couldn't find any task matching '{keywordToDelete}'.\n");
                }

                return;
            }

            // Step 7: Show Activity Log
            if (Regex.IsMatch(lowerInput, @"\b(activity log|what have you done|show log|my history)\b"))
            {
                if (activityLog.Count == 0)
                {
                    AppendText("ChatBot: Your activity log is currently empty.\n");
                }
                else
                {
                    AppendText("ChatBot: Here's your recent activity:\n");
                    foreach (var entry in activityLog)
                    {
                        AppendText($"- {entry}\n");
                    }
                }
                return;
            }

            // Step 8: Add Task NLP
            if (Regex.IsMatch(lowerInput, @"\b(add|create|new)\b.*\b(task|reminder|todo)\b"))
            {
                string title = ExtractTaskTitle(input);
                if (string.IsNullOrEmpty(title))
                {
                    AppendText("ChatBot: Please tell me the task title you'd like to add.\n");
                    return;
                }

                pendingTaskTitle = title;
                waitingForDescription = true;
                LogActivity($"Started creating task: '{title}'.");
                AppendText($"ChatBot: Task '{title}' created. Please provide a description for this task.\n");
                return;
            }

            // Step 9: Sentiment
            foreach (var feeling in sentimentResponses)
            {
                if (lowerInput.Contains(feeling.Key))
                {
                    AppendText($"ChatBot: {sentimentResponses[feeling.Key]}\n");
                    LogActivity($"Sentiment detected: '{feeling.Key}'.");
                    return;
                }
            }

            // Step 10: Cybersecurity Tips
            foreach (var keyword in keywordMap)
            {
                if (lowerInput.Contains(keyword.Key))
                {
                    currentTopic = keyword.Value;
                    string[] tips = randomResponses[keyword.Value];
                    string selectedTip = tips[random.Next(tips.Length)];
                    AppendText($"ChatBot: {selectedTip}\n");
                    LogActivity($"Provided tip for '{keyword.Key}'.");
                    return;
                }
            }

            // Step 11: Exit
            if (lowerInput == "exit")
            {
                AppendText("ChatBot: Stay safe! Goodbye.\n");
                Application.Current.Shutdown();
                return;
            }

            // Step 12: Fallback
            AppendText("ChatBot: I'm not sure how to help with that. Try 'add task', 'view tasks', or ask for tips like 'phishing'.\n");
        }

        private void AppendText(string message)
        {
            ChatOutput.Text += message;
        }

        private void LogActivity(string description)
        {
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            activityLog.Add($"{timestamp} - {description}");
            if (activityLog.Count > 10)
                activityLog.RemoveAt(0);
        }

        private string ExtractTaskTitle(string input)
        {
            string lowered = input.ToLower();
            int idx = lowered.IndexOf("task");
            if (idx >= 0)
            {
                string title = input.Substring(idx + 4).Trim();
                return title;
            }
            return null;
        }

        private string ExtractKeywordAfterDelete(string input)
        {
            string[] deletePhrases = { "delete task", "remove task", "delete reminder", "remove reminder", "delete", "remove" };
            input = input.ToLower();

            foreach (var phrase in deletePhrases)
            {
                if (input.Contains(phrase))
                {
                    int index = input.IndexOf(phrase);
                    return input.Substring(index + phrase.Length).Trim();
                }
            }

            return null;
        }

        public class CyberTask
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
        }
    }
}
