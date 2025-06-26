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

        Dictionary<string, string[]> randomResponses = new Dictionary<string, string[]>
        {
            { "phishing tips", new[] { "Phishing emails often look urgent to trick you.", "Don't click suspicious links in emails.", "Look out for grammar errors in scam emails." } },
            { "password tips", new[] { "Use a passphrase instead of a single word.", "Avoid using the same password for everything.", "Use a password manager to stay safe." } },
            { "data tips", new[] { "Don't share personal details on untrusted sites.", "Encrypt sensitive files before storing them online.", "Be cautious about what you post on social media." } },
            { "scam tips", new[] { "If it sounds too good to be true, it probably is.", "Never share your OTP or PIN with anyone.", "Scammers may pretend to be someone you trust." } },
            { "malware tips", new[] { "Avoid opening unknown attachments.", "Keep your antivirus software updated.", "Use a firewall to block unwanted access." } },
            { "privacy tips", new[] { "Review app permissions regularly.", "Use incognito mode or VPN when needed.", "Disable unnecessary location sharing." } }
        };

        Dictionary<string, string> sentimentResponses = new Dictionary<string, string>
        {
            { "worried", "It's completely understandable to feel worried. Cybersecurity can be scary, but you're taking the right steps." },
            { "frustrated", "I understand your frustration. Let's take things one step at a time." },
            { "curious", "That's a great attitude! Curiosity is the first step to learning." }
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

            // Step 3: Handle description input
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

            // Step 4: NLP-Based Reminder Handling
            Regex fullRemindRegex = new Regex(@"remind me( to)? (.+?) in (\d+) (day|days|week|weeks)", RegexOptions.IgnoreCase);
            Regex simpleRemindRegex = new Regex(@"remind me in (\d+) (day|days|week|weeks)", RegexOptions.IgnoreCase);

            Match fullMatch = fullRemindRegex.Match(lowerInput);
            Match simpleMatch = simpleRemindRegex.Match(lowerInput);

            if (fullMatch.Success)
            {
                string action = fullMatch.Groups[2].Value.Trim();
                int number = int.Parse(fullMatch.Groups[3].Value);
                string unit = fullMatch.Groups[4].Value;

                DateTime reminderDate = DateTime.Now;
                if (unit.StartsWith("day"))
                    reminderDate = reminderDate.AddDays(number);
                else if (unit.StartsWith("week"))
                    reminderDate = reminderDate.AddDays(number * 7);

                string taskTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    action.Length > 30 ? action.Substring(0, 30) : action
                );

                taskList.Add(new CyberTask
                {
                    Title = taskTitle,
                    Description = action,
                    ReminderDate = reminderDate
                });

                AppendText($"ChatBot: New task '{taskTitle}' added with a reminder for {reminderDate.ToShortDateString()}.\n");
                return;
            }
            else if (simpleMatch.Success && pendingTaskTitle != null)
            {
                int number = int.Parse(simpleMatch.Groups[1].Value);
                string unit = simpleMatch.Groups[2].Value;

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
                        string reminder = task.ReminderDate.HasValue
                            ? $" (Reminder: {task.ReminderDate.Value.ToShortDateString()})"
                            : "";
                        AppendText($"- {task.Title}: {task.Description}{reminder}\n");
                    }
                }
                return;
            }

            // Step 6: Delete Tasks
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
                    AppendText($"ChatBot: Task '{taskToRemove.Title}' has been deleted.\n");
                }
                else
                {
                    AppendText($"ChatBot: I couldn't find any task matching '{keywordToDelete}'.\n");
                }

                return;
            }

            // Step 7: Add Task NLP
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
                AppendText($"ChatBot: Task '{title}' created. Please provide a description for this task.\n");
                return;
            }

            // Step 8: Sentiment detection
            foreach (var feeling in sentimentResponses)
            {
                if (lowerInput.Contains(feeling.Key))
                {
                    AppendText($"ChatBot: {sentimentResponses[feeling.Key]}\n");
                    return;
                }
            }

            // Step 9: Cybersecurity tips
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

            // Step 10: Exit
            if (lowerInput == "exit")
            {
                AppendText("ChatBot: Stay safe! Goodbye.\n");
                Application.Current.Shutdown();
                return;
            }

            // Step 11: Fallback
            AppendText("ChatBot: I'm not sure how to help with that. Try 'add task', 'view tasks', or ask for tips like 'phishing'.\n");
        }

        private void AppendText(string message)
        {
            ChatOutput.Text += message;
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
