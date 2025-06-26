using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Part_3.Pages
{
    /// <summary>
    /// Interaction logic for QuizGamePage.xaml
    /// </summary>
    public partial class QuizGamePage : Page
    {
        private int currentQuestionIndex = 0;
        private int score = 0;

        public class QuizQuestion
        {
            public string QuestionText { get; set; }
            public List<string> Options { get; set; }
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; }
        }

        private List<QuizQuestion> questions = new List<QuizQuestion>
        {
            new QuizQuestion { QuestionText = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectIndex = 2,
                Explanation = "Reporting phishing emails helps prevent scams." },

            new QuizQuestion { QuestionText = "True or False: It's safe to use the same password for all your accounts.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 1,
                Explanation = "Using the same password puts all your accounts at risk." },

            new QuizQuestion { QuestionText = "What is a strong password made of?",
                Options = new List<string> { "Your name and birthdate", "Random words with symbols and numbers", "Only numbers", "123456" },
                CorrectIndex = 1,
                Explanation = "Strong passwords include random words, symbols, and numbers." },

            new QuizQuestion { QuestionText = "True or False: Antivirus software should be updated regularly.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 0,
                Explanation = "Regular updates protect you from new threats." },

            new QuizQuestion { QuestionText = "Which of the following is a form of social engineering?",
                Options = new List<string> { "Firewall", "Phishing", "Encryption", "VPN" },
                CorrectIndex = 1,
                Explanation = "Phishing is a social engineering technique." },

            new QuizQuestion { QuestionText = "How can you tell if a website is secure?",
                Options = new List<string> { "It has a lock icon and https", "It loads quickly", "It has ads", "You recognize the name" },
                CorrectIndex = 0,
                Explanation = "Secure websites use https and show a lock icon." },

            new QuizQuestion { QuestionText = "True or False: Public Wi-Fi is always safe for online banking.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 1,
                Explanation = "Public Wi-Fi is often unsecured and unsafe." },

            new QuizQuestion { QuestionText = "What is the purpose of two-factor authentication?",
                Options = new List<string> { "To double your login time", "To improve user experience", "To add an extra layer of security", "To confuse hackers" },
                CorrectIndex = 2,
                Explanation = "It adds an extra step to protect your account." },

            new QuizQuestion { QuestionText = "What should you do before clicking a link in an email?",
                Options = new List<string> { "Click immediately", "Hover over to check URL", "Forward to friends", "Ignore completely" },
                CorrectIndex = 1,
                Explanation = "Hovering helps detect fake or harmful links." },

            new QuizQuestion { QuestionText = "True or False: Software updates often contain security patches.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 0,
                Explanation = "Updates patch security vulnerabilities." }
        };

        public QuizGamePage()
        {
            InitializeComponent();
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var q = questions[currentQuestionIndex];
                QuestionTextBlock.Text = q.QuestionText;
                OptionButtonsPanel.Children.Clear();

                for (int i = 0; i < q.Options.Count; i++)
                {
                    var btn = new Button
                    {
                        Content = q.Options[i],
                        Tag = i,
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Padding = new Thickness(10)
                    };
                    btn.Click += Option_Click;
                    OptionButtonsPanel.Children.Add(btn);
                }
                FeedbackTextBlock.Text = "";
            }
            else
            {
                QuestionTextBlock.Text = $"Quiz Completed! You scored {score}/{questions.Count}.";
                FeedbackTextBlock.Text = score >= 8
                    ? "Great job! You're a cybersecurity pro!"
                    : "Keep learning to stay safe online!";
                OptionButtonsPanel.Children.Clear();
            }
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            var selectedButton = sender as Button;
            int selectedIndex = (int)selectedButton.Tag;
            var currentQuestion = questions[currentQuestionIndex];

            if (selectedIndex == currentQuestion.CorrectIndex)
            {
                score++;
                FeedbackTextBlock.Text = "Correct! " + currentQuestion.Explanation;
            }
            else
            {
                FeedbackTextBlock.Text = "Incorrect. " + currentQuestion.Explanation;
            }

            currentQuestionIndex++;
            Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(() => ShowQuestion()));
        }
    }

    public class CyberTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
    }
 }
