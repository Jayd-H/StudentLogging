using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace StudentLogging
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Initialize user management logic
            var userManager = new UserManager();

            // Loop to continuously display the main menu
            while (true)
            {
                Console.Clear(); // Clears the console for menu display
                DisplayMainMenu(); // Displays the main menu options

                string choice = Console.ReadLine();
                Console.Clear();
                DisplayMainMenu(); // Redisplay menu header after input

                // Process user input
                switch (choice)
                {
                    case "1":
                        userManager.HandleProfile("students.txt", UserType.Student);
                        break;
                    case "2":
                        userManager.HandleProfile("ps.txt", UserType.PersonalSupervisor);
                        break;
                    case "3":
                        userManager.HandleProfile("st.txt", UserType.SeniorTutor);
                        break;
                    case "4":
                        // Exit the application
                        return;
                    default:
                        DisplayErrorMessage("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        // Method to display the main menu
        private static void DisplayMainMenu()
        {
            Console.WriteLine($"----- Main Menu -----");
            Console.WriteLine("Select your role:");
            Console.WriteLine("1. Student");
            Console.WriteLine("2. Personal Supervisor (PS)");
            Console.WriteLine("3. Senior Tutor (ST)");
            Console.WriteLine("4. Exit");
            Console.WriteLine();
            Console.WriteLine("Enter your choice:");
        }

        // Method to display error messages in red
        public static void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor(); // Reset to default color
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }

    // Enum representing different types of users in the system
    public enum UserType
    {
        Student,
        PersonalSupervisor,
        SeniorTutor
    }

    public class UserManager
    {
        // Method to handle user profile operations based on file and user type
        public void HandleProfile(string fileName, UserType userType)
        {
            // Load user profiles from the specified file
            var profiles = ProfileIO.LoadProfiles(fileName);

            // Display profile selection menu
            Console.WriteLine("Select a profile or enter '0' to go back:");
            for (int i = 0; i < profiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {profiles[i].Name}");
            }
            // Option to create a new profile
            Console.WriteLine($"{profiles.Count + 1}. Create New Profile");
            Console.WriteLine();
            Console.WriteLine("Enter your choice:");

            int choice;
            // Validate user input for profile selection
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > profiles.Count + 1)
            {
                Program.DisplayErrorMessage("Invalid choice. Please select again.");
            }

            if (choice == 0)
            {
                return; // Return to previous menu if 0 is chosen
            }

            Console.Clear();

            // Display profile selection header
            Console.WriteLine("Select a profile:");
            Console.WriteLine();

            if (choice == profiles.Count + 1)
            {
                // Process for creating a new profile
                Console.WriteLine("Enter your name:");
                string name = Console.ReadLine();

                Console.WriteLine("Set your password:");
                string password = Console.ReadLine();

                // Create and add new user profile
                var newProfile = new UserProfile { Name = name, Password = password };
                profiles.Add(newProfile);
                ProfileIO.SaveProfiles(fileName, profiles);

                Console.WriteLine($"Profile {name} created!");

                // Special handling for student profiles
                if (userType == UserType.Student)
                {
                    var newStudent = new Student { Name = name };
                    Menus.ChangePersonalSupervisor(newStudent);
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                // Display user-specific menu for the new profile
                Menus.DisplayMenu(userType, newProfile);
            }
            else
            {
                // Password validation for existing profiles
                string passwordInput = "";
                while (passwordInput != profiles[choice - 1].Password)
                {
                    Console.WriteLine($"Enter password for {profiles[choice - 1].Name}:");
                    passwordInput = Console.ReadLine();
                }
                Console.WriteLine($"Access granted for {profiles[choice - 1].Name}!");

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                // Display user-specific menu for the selected profile
                Menus.DisplayMenu(userType, profiles[choice - 1]);
            }
        }
    }


    public class Menus
    {
        // Display the appropriate menu based on user type and profile
        public static void DisplayMenu(UserType userType, UserProfile profile)
        {
            switch (userType)
            {
                case UserType.Student:
                    // Conversion from UserProfile to Student, initializing a new Student instance
                    var student = new Student { Name = profile.Name };
                    StudentMenu(student, profile);
                    break;
                case UserType.PersonalSupervisor:
                    // Display menu for Personal Supervisors
                    PersonalSupervisorMenu(profile);
                    break;
                case UserType.SeniorTutor:
                    // Display menu for Senior Tutors
                    SeniorTutorMenu(profile);
                    break;
            }
        }

        // Menu specific to students, offering various student-related actions
        public static void StudentMenu(Student student, UserProfile userProfile)
        {
            // Load the name of the student's personal supervisor
            string supervisorName = ProfileIO.LoadStudentSupervisor(student.Name);
            if (supervisorName != null)
            {
                student.Supervisor = new PersonalSupervisor { Name = supervisorName };
            }

            while (true)
            {
                Console.Clear();
                // Display student-specific menu header
                Console.WriteLine($"----- Welcome {student.Name} -----");
                Console.WriteLine($"Your Personal Supervisor: {student.Supervisor?.Name ?? "None assigned yet"}");
                Console.WriteLine("----- Student Menu -----");
                Console.WriteLine("1. Book an appointment");
                Console.WriteLine("2. View upcoming meetings");
                Console.WriteLine("3. Change personal supervisor");
                Console.WriteLine("4. Log your feeling for today");
                Console.WriteLine("5. View your feeling logs");
                Console.WriteLine("6. Change password");
                Console.WriteLine("7. Log out");
                Console.WriteLine();
                Console.WriteLine("Enter your choice:");

                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.Clear();
                    // Re-display menu header after user input
                    Console.WriteLine($"----- Welcome {student.Name} -----");
                    Console.WriteLine($"Your Personal Supervisor: {student.Supervisor?.Name ?? "None assigned yet"}");
                    Console.WriteLine();

                    // Handling student menu choices
                    switch (choice)
                    {
                        case 1:
                            // Function to book an appointment
                            BookAppointment(student);
                            break;
                        case 2:
                            DisplayUpcomingAppointments(student.Name);
                            break;
                        case 3:
                            ChangePersonalSupervisor(student);
                            break;
                        case 4:
                            LogFeeling(student);
                            break;
                        case 5:
                            ViewFeelingLogs(student.Name);
                            break;
                        case 6:
                            ChangePassword(userProfile, "students.txt");
                            break;
                        case 7:
                            return; // Log out
                        default:
                            Program.DisplayErrorMessage("Invalid choice. Please try again.");
                            break;
                    }
                    Console.ReadKey();
                }
                catch (FormatException)
                {
                    // Handle invalid input format
                    Console.WriteLine("Please enter a valid number.");
                }
            }
        }

        public static void PersonalSupervisorMenu(UserProfile profile)
        {
            while (true)
            {
                Console.Clear();
                // Display the Personal Supervisor menu header
                Console.WriteLine($"----- Welcome {profile.Name} -----");
                Console.WriteLine("----- Personal Supervisor Menu -----");
                // Menu options for the Personal Supervisor
                Console.WriteLine("1. View student list");
                Console.WriteLine("2. Schedule meeting");
                Console.WriteLine("3. View student's feeling logs");
                Console.WriteLine("4. View upcoming meetings");
                Console.WriteLine("5. Change password");
                Console.WriteLine("6. Log out");
                Console.WriteLine();
                Console.WriteLine("Enter your choice:");

                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.Clear();

                    // Re-display the menu header after receiving input
                    Console.WriteLine($"----- Welcome {profile.Name} -----");
                    Console.WriteLine();

                    // Handle menu choice selection
                    switch (choice)
                    {
                        case 1:
                            // Function to view the list of assigned students
                            ViewStudentList(profile);
                            break;
                        case 2:
                            // Function to schedule a meeting with a student
                            ScheduleMeeting(profile);
                            break;
                        case 3:
                            // Function to view a specific student's feeling logs
                            ChooseStudentAndViewLogs(profile);
                            break;
                        case 4:
                            // Function to display upcoming meetings for the supervisor
                            DisplayUpcomingAppointments(profile.Name);
                            break;
                        case 5:
                            // Function to change the supervisor's password
                            ChangePassword(profile, "ps.txt");
                            break;
                        case 6:
                            return; // Log out from the menu
                        default:
                            // Handle invalid menu choice
                            Program.DisplayErrorMessage("Invalid choice. Please try again.");
                            break;
                    }
                    Console.ReadKey();
                }
                catch (FormatException)
                {
                    // Handle incorrectly formatted input
                    Program.DisplayErrorMessage("Please enter a valid number.");
                }
            }
        }

        public static void SeniorTutorMenu(UserProfile profile)
        {
            while (true)
            {
                Console.Clear();
                // Display the Senior Tutor menu header
                Console.WriteLine($"----- Welcome {profile.Name} -----");
                Console.WriteLine("----- Senior Tutor Menu -----");
                // Menu options for the Senior Tutor
                Console.WriteLine("1. View overall feeling logs");
                Console.WriteLine("2. View overall meetings");
                Console.WriteLine("3. Change password");
                Console.WriteLine("4. Log out");
                Console.WriteLine();
                Console.WriteLine("Enter your choice:");

                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.Clear();

                    // Reprint the menu header for context after selection
                    Console.WriteLine($"----- Welcome {profile.Name} -----");
                    Console.WriteLine("----- Senior Tutor Menu -----");
                    Console.WriteLine();

                    // Handle menu choice selection
                    switch (choice)
                    {
                        case 1:
                            // Function to view aggregated feeling logs
                            ViewAllLogs();
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 2:
                            // Function to view all scheduled meetings
                            ViewAllMeetings();
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 3:
                            // Function to change the senior tutor's password
                            ChangePassword(profile, "st.txt");
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 4:
                            return; // Log out from the menu
                        default:
                            // Handle invalid menu choice
                            Program.DisplayErrorMessage("Invalid choice. Please try again.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (FormatException)
                {
                    // Handle incorrectly formatted input
                    Program.DisplayErrorMessage("Please enter a valid number.");
                    Console.ReadKey();
                }
            }
        }


        public static void ViewAllLogs()
        {
            // Check if the feelings log file exists
            if (!File.Exists("feelings_log.txt"))
            {
                Program.DisplayErrorMessage("No feeling logs available.");
                return;
            }

            var lines = File.ReadAllLines("feelings_log.txt");
            var allLogs = new List<Tuple<DateTime, string>>();

            foreach (var line in lines)
            {
                // Extract the DateTime from each line, assuming a standard format
                var datePart = line.Substring(0, 10);  // Format "yyyy-MM-dd"
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logTime))
                {
                    // Add parsed log entry to the list
                    allLogs.Add(Tuple.Create(logTime, line));
                }
            }

            // Check if any logs were parsed successfully
            if (allLogs.Count == 0)
            {
                Program.DisplayErrorMessage("No feeling logs available.");
            }
            else
            {
                Console.WriteLine("All Feeling Logs:");

                // Sort logs in descending order by date and display
                foreach (var log in allLogs.OrderByDescending(tuple => tuple.Item1))
                {
                    Console.WriteLine(log.Item2);
                }
            }
        }

        public static void ViewAllMeetings()
        {
            // Check if the appointments file exists
            if (!File.Exists("appointments.txt"))
            {
                Program.DisplayErrorMessage("No scheduled appointments.");
                return;
            }

            var lines = File.ReadAllLines("appointments.txt");
            var allMeetings = new List<Tuple<DateTime, string>>();

            foreach (var line in lines)
            {
                // Extract the DateTime from each line, assuming a standard format
                var datePart = line.Substring(0, 16);  // Format "yyyy-MM-dd HH:mm"
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime appointmentTime))
                {
                    // Add parsed appointment to the list
                    allMeetings.Add(Tuple.Create(appointmentTime, line));
                }
            }

            // Check if any appointments were parsed successfully
            if (allMeetings.Count == 0)
            {
                Program.DisplayErrorMessage("No scheduled appointments.");
            }
            else
            {
                Console.WriteLine("All Scheduled Appointments:");

                // Sort appointments in ascending order by date and display
                foreach (var meeting in allMeetings.OrderBy(tuple => tuple.Item1))
                {
                    Console.WriteLine(meeting.Item2);
                }
            }
        }



        public static void ChooseStudentAndViewLogs(UserProfile supervisorProfile)
        {
            // Load list of students supervised by the given supervisor
            List<Student> studentsUnderSupervisor = ProfileIO.LoadStudentsUnderSupervisor(supervisorProfile.Name);

            // Check if there are any students assigned
            if (!studentsUnderSupervisor.Any())
            {
                Program.DisplayErrorMessage("You have no students assigned to you.");
                return;
            }

            Console.WriteLine("Select a student to view feeling logs:");

            // Display a list of students for selection
            for (int i = 0; i < studentsUnderSupervisor.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {studentsUnderSupervisor[i].Name}");
            }

            int choice;
            // Ensure valid input for selecting a student
            do
            {
                Console.Write("Enter your choice: ");
            }
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > studentsUnderSupervisor.Count);

            // Get the selected student
            Student selectedStudent = studentsUnderSupervisor[choice - 1];
            // View feeling logs for the selected student
            ViewFeelingLogs(selectedStudent.Name);
        }

        public static void LogFeeling(Student student)
        {
            Console.WriteLine($"Logging feeling for {student.Name}");

            int feelingRating;
            // Prompt for a feeling rating between 1 and 10
            do
            {
                Console.Write("On a scale from 1-10, how are you feeling today? ");
            }
            while (!int.TryParse(Console.ReadLine(), out feelingRating) || feelingRating < 1 || feelingRating > 10);

            // Format and append the feeling log to the file
            string logEntry = $"{DateTime.Now:yyyy-MM-dd} - {student.Name} - {feelingRating}\n";
            File.AppendAllText("feelings_log.txt", logEntry);

            Console.WriteLine($"Your feeling for today has been logged as {feelingRating}/10.");
        }

        public static void ViewFeelingLogs(string userName)
        {
            // Check if the feelings log file exists
            if (!File.Exists("feelings_log.txt"))
            {
                Program.DisplayErrorMessage("No feeling logs available.");
                return;
            }

            var lines = File.ReadAllLines("feelings_log.txt");
            var userFeelings = new List<string>();

            // Filter logs for the specified user
            foreach (var line in lines)
            {
                if (line.Contains(userName))
                {
                    userFeelings.Add(line);
                }
            }

            // Check if there are any logs for the user
            if (userFeelings.Count == 0)
            {
                Program.DisplayErrorMessage("No feeling logs for this user.");
            }
            else
            {
                Console.WriteLine($"{userName}'s Feelings Logs:");
                // Display each log entry for the user
                foreach (var log in userFeelings)
                {
                    Console.WriteLine(log);
                }
            }
        }

        public static void DisplayUpcomingAppointments(string userName)
        {
            if (!File.Exists("appointments.txt"))
            {
                Program.DisplayErrorMessage("No upcoming appointments.");
                return;
            }

            var lines = File.ReadAllLines("appointments.txt");
            var upcomingMeetings = new List<Tuple<DateTime, string>>();

            foreach (var line in lines)
            {
                if (line.Contains(userName))
                {
                    // Extract the DateTime from the line
                    var datePart = line.Substring(0, 16);  // Assuming the format "yyyy-MM-dd HH:mm"
                    if (DateTime.TryParseExact(datePart, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime appointmentTime))
                    {
                        upcomingMeetings.Add(Tuple.Create(appointmentTime, line));
                    }
                }
            }

            if (upcomingMeetings.Count == 0)
            {
                Program.DisplayErrorMessage("No upcoming appointments.");
            }
            else
            {
                Console.WriteLine("Upcoming Appointments:");

                // Sort and display
                foreach (var meeting in upcomingMeetings.OrderBy(tuple => tuple.Item1))
                {
                    Console.WriteLine(meeting.Item2);
                }
            }
        }



        public static void BookAppointment(Student student)
        {
            // Check if the student has a personal supervisor assigned
            if (student.Supervisor == null)
            {
                Program.DisplayErrorMessage("You don't have a personal supervisor assigned. Please contact the administration.");
                return;
            }

            Console.WriteLine($"Booking appointment with {student.Supervisor.Name}");
            DateTime? appointmentTime = null;

            // Loop until a valid appointment time is selected
            while (appointmentTime == null)
            {
                Console.Write("Please enter the date for the appointment (yyyy-MM-dd): ");
                // Validate date input
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    Program.DisplayErrorMessage("Invalid date format. Please try again.");
                    continue;
                }

                // Check if the selected date is in the future
                if (date.Date < DateTime.Now.Date)
                {
                    Program.DisplayErrorMessage("The date should be in the future. Please try again.");
                    continue;
                }

                // Generate available time slots for the given date
                List<DateTime> availableTimeSlots = GenerateAvailableTimeSlots(date);

                // Handle case where no slots are available
                if (availableTimeSlots.Count == 0)
                {
                    Program.DisplayErrorMessage("No available time slots for the selected date. Please choose another date.");
                    continue;
                }

                // Display available time slots
                Console.WriteLine("Available time slots:");
                for (int i = 0; i < availableTimeSlots.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {availableTimeSlots[i]:HH:mm}");
                }

                Console.Write("Select a time slot from the list: ");
                // Validate selected time slot
                if (int.TryParse(Console.ReadLine(), out int selectedSlot) && selectedSlot >= 1 && selectedSlot <= availableTimeSlots.Count)
                {
                    appointmentTime = availableTimeSlots[selectedSlot - 1];
                }
                else
                {
                    Program.DisplayErrorMessage("Invalid selection. Please select a valid time slot.");
                }
            }

            // Save the booked appointment to a file
            File.AppendAllText("appointments.txt", $"{appointmentTime:yyyy-MM-dd HH:mm} - {student.Supervisor.Name} with {student.Name}\n");
            Console.WriteLine($"Your appointment with {student.Supervisor.Name} is booked for {appointmentTime:yyyy-MM-dd HH:mm}");
        }

        private static List<DateTime> GenerateAvailableTimeSlots(DateTime date)
        {
            // Create a list to hold available time slots
            List<DateTime> availableTimeSlots = new List<DateTime>();

            // Generate time slots within working hours (9 AM to 5 PM)
            for (DateTime time = date.AddHours(9); time <= date.AddHours(17); time = time.AddMinutes(30))
            {
                // Check if the time slot is already taken
                if (!IsTimeSlotTaken(time))
                {
                    availableTimeSlots.Add(time);
                }
            }

            return availableTimeSlots;
        }

        private static bool IsTimeSlotTaken(DateTime time)
        {
            // Check if the appointments file exists
            if (!File.Exists("appointments.txt"))
                return false;

            // Check each line in the file to see if the time slot is already booked
            foreach (var line in File.ReadAllLines("appointments.txt"))
            {
                // Extract the date and time from the line and compare
                if (DateTime.TryParseExact(line.Split(' ')[0] + " " + line.Split(' ')[1], "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out DateTime existingTime) && existingTime == time)
                {
                    return true; // The time slot is taken
                }
            }
            return false; // The time slot is available
        }


        private static List<Student> ViewStudentList(UserProfile profile)
        {
            var students = ProfileIO.LoadStudentsUnderSupervisor(profile.Name);
            if (students.Count == 0)
            {
                Program.DisplayErrorMessage("No students assigned to you currently.");
                return new List<Student>();
            }

            Console.WriteLine($"Students under {profile.Name}:");
            foreach (var student in students)
            {
                Console.WriteLine(student.Name);
            }

            return students;
        }

        public static void ScheduleMeeting(UserProfile supervisorProfile)
        {
            // First, fetch the list of students under the supervisor
            List<Student> studentsUnderSupervisor = ProfileIO.LoadStudentsUnderSupervisor(supervisorProfile.Name);

            if (!studentsUnderSupervisor.Any())
            {
                Program.DisplayErrorMessage("You have no students assigned to you.");
                return;
            }

            Console.WriteLine("Select a student to schedule a meeting with:");

            for (int i = 0; i < studentsUnderSupervisor.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {studentsUnderSupervisor[i].Name}");
            }

            int choice;
            do
            {
                Console.Write("Enter your choice: ");
            }
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > studentsUnderSupervisor.Count);

            // Use the selected student for the meeting
            Student selectedStudent = studentsUnderSupervisor[choice - 1];

            // Now, book the appointment as you would for a student
            BookAppointment(selectedStudent);

            Console.WriteLine($"Meeting scheduled with {selectedStudent.Name}.");
        }

        public static void ChangePassword(UserProfile profile, string fileName)
        {
            Console.WriteLine("Enter your current password:");
            string currentPassword = Console.ReadLine();

            // Verify the current password
            if (currentPassword != profile.Password)
            {
                Program.DisplayErrorMessage("Incorrect password. Returning to menu.");
                return;
            }

            Console.WriteLine("Enter your new password:");
            string newPassword = Console.ReadLine();
            // Update the password in the user's profile
            profile.Password = newPassword;

            // Load existing profiles from the file
            var profiles = ProfileIO.LoadProfiles(fileName);
            // Find the user's profile to update
            var userToUpdate = profiles.FirstOrDefault(p => p.Name == profile.Name);
            if (userToUpdate != null)
            {
                // Update the password and save the profiles back to the file
                userToUpdate.Password = newPassword;
                ProfileIO.SaveProfiles(fileName, profiles);
                Console.WriteLine("Password successfully updated!");
            }
        }

        public static void ChangePersonalSupervisor(Student student)
        {
            Console.WriteLine("Select a new personal supervisor:");

            // Load all available personal supervisors
            var availableSupervisors = ProfileIO.LoadProfiles("ps.txt").Select(p => new PersonalSupervisor { Name = p.Name }).ToList();

            // Display the list of available supervisors for selection
            for (int i = 0; i < availableSupervisors.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableSupervisors[i].Name}");
            }

            int choice;
            // Ensure valid input for selecting a supervisor
            do
            {
                Console.Write("Enter your choice: ");
            }
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > availableSupervisors.Count);

            // Assign the selected supervisor to the student
            student.Supervisor = availableSupervisors[choice - 1];
            Console.WriteLine($"Your personal supervisor is now {student.Supervisor.Name}!");

            // Save the new supervisor-student pairing to the file
            ProfileIO.SaveStudentSupervisorPair(student.Name, student.Supervisor.Name);
        }
    }

    public static class ProfileIO
    {
        public static List<UserProfile> LoadProfiles(string fileName)
        {
            var profiles = new List<UserProfile>();
            if (!File.Exists(fileName))
                return profiles;

            foreach (var line in File.ReadAllLines(fileName))
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    profiles.Add(new UserProfile { Name = parts[0], Password = parts[1] });
                }
            }
            return profiles;
        }

        public static void SaveProfiles(string fileName, List<UserProfile> profiles)
        {
            var lines = new List<string>();
            foreach (var profile in profiles)
            {
                lines.Add($"{profile.Name}:{profile.Password}");
            }
            File.WriteAllLines(fileName, lines);
        }
        public static List<Student> LoadStudentsUnderSupervisor(string supervisorName)
        {
            var students = new List<Student>();
            if (!File.Exists("students_under_ps.txt"))
                return students;

            foreach (var line in File.ReadAllLines("students_under_ps.txt"))
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[1] == supervisorName)
                {
                    students.Add(new Student { Name = parts[0], Supervisor = new PersonalSupervisor { Name = parts[1] } });
                }
            }
            return students;
        }
        public static void SaveStudentSupervisorPair(string studentName, string supervisorName)
        {
            var lines = new List<string>();

            if (File.Exists("students_under_ps.txt"))
            {
                lines = File.ReadAllLines("students_under_ps.txt").ToList();

                // Find and remove the existing line for the student
                var existingLine = lines.FirstOrDefault(line => line.StartsWith(studentName + ":"));
                if (existingLine != null)
                {
                    lines.Remove(existingLine);
                }
            }

            // Add the new supervisor for the student
            lines.Add($"{studentName}:{supervisorName}");

            File.WriteAllLines("students_under_ps.txt", lines);
        }

        public static string LoadStudentSupervisor(string studentName)
        {
            if (!File.Exists("students_under_ps.txt"))
                return null;

            foreach (var line in File.ReadAllLines("students_under_ps.txt"))
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == studentName)
                {
                    return parts[1];
                }
            }
            return null;
        }
    }

    public class UserProfile
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public PersonalSupervisor Supervisor { get; set; } // reference to a PersonalSupervisor
    }
    public class PersonalSupervisor
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class SeniorTutor
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
