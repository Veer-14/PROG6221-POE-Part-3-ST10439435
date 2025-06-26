# 🛡️ CyberSecurity Awareness Assistant Chatbot (WPF GUI Version)

This is a modern, WPF-based cybersecurity awareness chatbot application built in C#. It educates users about core cybersecurity concepts like strong passwords, phishing, and data privacy, and now includes new features introduced in **Part 3** of the Practical Outcome Evaluation (POE).

## ✅ Part 3 Features Implemented

### 1. 🗂️ Task Management with Reminders
- Users can add tasks by specifying:
  - **Title**
  - **Description**
  - **Reminder** (in natural phrases like "Remind me to update my password in 3 days")
- Tasks can be:
  - **Viewed**
  - **Deleted**
  - **Marked complete** (optional enhancement)
- Each task can optionally include a **reminder date**, and tasks are stored in-memory.

### 2. ❓ Cybersecurity Quiz
- Includes **10 well-researched questions**.
- Uses **multiple choice** and **true/false** formats.
- **One question is displayed at a time**.
- **Immediate feedback** is given after every answer (correct/incorrect with explanation).
- At the end, a **summary score** and evaluation message is shown (e.g., "Excellent Awareness!", "Needs Improvement").

### 3. 💬 Natural Language Processing (NLP) Simulation
- Expanded keyword detection from Parts 1 & 2.
- Recognizes **keywords in full phrases or commands**, e.g.:
  - "Add a reminder to check my firewall"
  - "Can you show me phishing tips?"
  - "Delete task about VPN"
- Flexible NLP logic allows the chatbot to understand a wider variety of phrasings.
- Regular expressions and string detection used to simulate NLP behavior.

### 4. 📜 Activity Log / Chat History
- Tracks and displays key user interactions, including:
  - Tasks created & reminders set
  - Quiz attempts & scores
  - Keywords/topics discussed
- Users can type `"show activity log"` or `"what have you done"` to see their **recent 5–10 actions**.
- Optionally supports pagination for older history using `"show more"`.

---

## 🖥️ GUI Technology
- **WPF (Windows Presentation Foundation)** used for the GUI interface.
- Modern UI design with:
  - Flat button styles
  - Sidebar navigation
  - Dark theme layout

---

## 🔧 Requirements
- [.NET 5.0 SDK](https://dotnet.microsoft.com/download) or newer
- Windows 10/11 (required for WPF support)
- Visual Studio 2019/2022 (recommended)

---

## ▶️ How to Run the Application

1. **Clone or download** this repository to your computer.
2. Open the solution in **Visual Studio**.
3. Set the startup project to the WPF application.
4. Press `F5` to **build and run** the project.

---

## 📝 Features from Part 1 & Part 2 (Still Included)
- Sentiment analysis (responds to words like "worried", "frustrated", etc.)
- Personalized responses based on user’s name and favorite topic
- Cybersecurity tips across multiple topics:
  - Passwords
  - Phishing
  - Malware
  - Data protection
  - Privacy
  - Scams
- Typing simulation effect (console version only — no longer used in GUI)
- Audio greeting using `.wav` file (optional for Part 1 only)

---

## 🗃️ File Structure
- `/Pages/ChatPage.xaml` – Main chatbot interaction UI
- `/Pages/QuizGamePage.xaml` – Quiz page with score tracking
- `MainWindow.xaml` – Entry layout with navigation
- `App.xaml` – Application setup and styling

---

## 👨‍💻 Authors
This project was created as part of the **Programming 2A POE** assignment.

---

## 📌 Notes
- All features are now implemented in a **WPF GUI**, not console.
- Console submissions will receive zero per POE requirement.
- This version includes basic NLP simulation, not full AI/NLU — but handles varied user input effectively using keyword mapping and regex.

