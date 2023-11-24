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
            var userManager = new UserManager();

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"----- Main Menu -----");
                Console.WriteLine("Select your role:");
                Console.WriteLine("1. Student");
                Console.WriteLine("2. Personal Supervisor (PS)");
                Console.WriteLine("3. Senior Tutor (ST)");
                Console.WriteLine("4. Exit");
                Console.WriteLine();
                Console.WriteLine("Enter your choice:");

                string choice = Console.ReadLine();

                Console.Clear();

                // Reprint the main menu header for context
                Console.WriteLine($"----- Main Menu -----");
                Console.WriteLine();

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
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine("\nPress any key to return to menu...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    public enum UserType
    {
        Student,
        PersonalSupervisor,
        SeniorTutor
    }


    public class UserManager
    {
        public void HandleProfile(string fileName, UserType userType)
        {
            var profiles = ProfileIO.LoadProfiles(fileName);

            Console.WriteLine("Select a profile:");
            for (int i = 0; i < profiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {profiles[i].Name}");
            }
            Console.WriteLine($"{profiles.Count + 1}. Create New Profile");
            Console.WriteLine();
            Console.WriteLine("Enter your choice:");

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > profiles.Count + 1)
            {
                Console.WriteLine("Invalid choice. Please select again.");
            }

            Console.Clear();

            // Reprint the profile header for context
            Console.WriteLine("Select a profile:");
            Console.WriteLine();

            if (choice == profiles.Count + 1)
            {
                Console.WriteLine("Enter your name:");
                string name = Console.ReadLine();

                Console.WriteLine("Set your password:");
                string password = Console.ReadLine();

                var newProfile = new UserProfile { Name = name, Password = password };
                profiles.Add(newProfile);
                ProfileIO.SaveProfiles(fileName, profiles);

                Console.WriteLine($"Profile {name} created!");

                // If the new profile is for a student, activate the ChangePersonalSupervisor function
                if (userType == UserType.Student)
                {
                    var newStudent = new Student { Name = name };
                    Menus.ChangePersonalSupervisor(newStudent);
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                Menus.DisplayMenu(userType, newProfile);
            }
            else
            {
                // Validate password before granting access
                string passwordInput = "";
                while (passwordInput != profiles[choice - 1].Password)
                {
                    Console.WriteLine($"Enter password for {profiles[choice - 1].Name}:");
                    passwordInput = Console.ReadLine();
                }
                Console.WriteLine($"Access granted for {profiles[choice - 1].Name}!");

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                Menus.DisplayMenu(userType, profiles[choice - 1]);
            }
        }
    }

    public class Menus
    {
        public static void DisplayMenu(UserType userType, UserProfile profile)
        {
            switch (userType)
            {
                case UserType.Student:
                    // Convert the UserProfile to a Student object. 
                    // For simplicity, we're creating a new Student object here.
                    var student = new Student { Name = profile.Name };
                    StudentMenu(student, profile);
                    break;
                case UserType.PersonalSupervisor:
                    PersonalSupervisorMenu(profile);
                    break;
                case UserType.SeniorTutor:
                    SeniorTutorMenu(profile);
                    break;
            }
        }


        public static void StudentMenu(Student student, UserProfile userProfile)
        {
            // Fetch the supervisor from the file
            string supervisorName = ProfileIO.LoadStudentSupervisor(student.Name);
            if (supervisorName != null)
            {
                student.Supervisor = new PersonalSupervisor { Name = supervisorName };
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"----- Welcome {student.Name} -----");
                Console.WriteLine($"Your Personal Supervisor: {student.Supervisor?.Name ?? "None assigned yet"}");
                Console.WriteLine("----- Student Menu -----");
                Console.WriteLine("1. Book an appointment");
                Console.WriteLine("2. View upcoming meetings");
                Console.WriteLine("3. Give feedback");
                Console.WriteLine("4. Change personal supervisor");
                Console.WriteLine("5. Log your feeling for today");
                Console.WriteLine("6. View your feeling logs");
                Console.WriteLine("7. Change password");
                Console.WriteLine("8. Log out");
                Console.WriteLine();
                Console.WriteLine("Enter your choice:");

                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.Clear();

                    // Reprint the menu header
                    Console.WriteLine($"----- Welcome {student.Name} -----");
                    Console.WriteLine($"Your Personal Supervisor: {student.Supervisor?.Name ?? "None assigned yet"}");
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1:
                            BookAppointment(student);
                            break;
                        case 2:
                            DisplayUpcomingAppointments(student.Name);
                            break;
                        case 3:
                            GiveFeedback();
                            break;
                        case 4:
                            ChangePersonalSupervisor(student);
                            break;
                        case 5:
                            LogFeeling(student);
                            break;
                        case 6:
                            ViewFeelingLogs(student.Name);
                            break;
                        case 7:
                            ChangePassword(userProfile, "students.txt");
                            break;
                        case 8:
                            return; // log out
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid number.");
                }
            }
        }


        public static void PersonalSupervisorMenu(UserProfile profile)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"----- Welcome {profile.Name} -----");
                Console.WriteLine("----- Personal Supervisor Menu -----");
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

                    // Reprint the menu header
                    Console.WriteLine($"----- Welcome {profile.Name} -----");
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1:
                            ViewStudentList(profile);
                            break;
                        case 2:
                            ScheduleMeeting(profile);
                            break;
                        case 3:
                            ChooseStudentAndViewLogs(profile);
                            break;
                        case 4:
                            DisplayUpcomingAppointments(profile.Name);
                            break;
                        case 5:
                            ChangePassword(profile, "ps.txt");
                            break;
                        case 6:
                            return; // log out
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid number.");
                }
            }
        }




        public static void SeniorTutorMenu(UserProfile profile)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine($"----- Welcome {profile.Name} -----");
                Console.WriteLine("----- Senior Tutor Menu -----");
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

                    // Reprint the senior tutor menu header for context
                    Console.WriteLine($"----- Welcome {profile.Name} -----");
                    Console.WriteLine("----- Senior Tutor Menu -----");
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1:
                            ViewAllLogs();
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 2:
                            ViewAllMeetings();
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 3:
                            ChangePassword(profile, "st.txt");
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                        case 4:
                            return; // log out
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            Console.WriteLine("\nPress any key to return to menu...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid number.");
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
            }
        }

        public static void ViewAllLogs()
        {
            if (!File.Exists("feelings_log.txt"))
            {
                Console.WriteLine("No feeling logs available.");
                return;
            }

            var lines = File.ReadAllLines("feelings_log.txt");
            var allLogs = new List<Tuple<DateTime, string>>();

            foreach (var line in lines)
            {
                // Extract the DateTime from the line
                var datePart = line.Substring(0, 10);  // Given the format "yyyy-MM-dd"
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logTime))
                {
                    allLogs.Add(Tuple.Create(logTime, line));
                }
            }

            if (allLogs.Count == 0)
            {
                Console.WriteLine("No feeling logs available.");
            }
            else
            {
                Console.WriteLine("All Feeling Logs:");

                // Sort in descending order (most recent first) and display
                foreach (var log in allLogs.OrderByDescending(tuple => tuple.Item1))
                {
                    Console.WriteLine(log.Item2);
                }
            }
        }

        public static void ViewAllMeetings()
        {
            if (!File.Exists("appointments.txt"))
            {
                Console.WriteLine("No scheduled appointments.");
                return;
            }

            var lines = File.ReadAllLines("appointments.txt");
            var allMeetings = new List<Tuple<DateTime, string>>();

            foreach (var line in lines)
            {
                // Extract the DateTime from the line
                var datePart = line.Substring(0, 16);  // Assuming the format "yyyy-MM-dd HH:mm"
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime appointmentTime))
                {
                    allMeetings.Add(Tuple.Create(appointmentTime, line));
                }
            }

            if (allMeetings.Count == 0)
            {
                Console.WriteLine("No scheduled appointments.");
            }
            else
            {
                Console.WriteLine("All Scheduled Appointments:");

                // Sort and display
                foreach (var meeting in allMeetings.OrderBy(tuple => tuple.Item1))
                {
                    Console.WriteLine(meeting.Item2);
                }
            }
        }


        public static void ChooseStudentAndViewLogs(UserProfile supervisorProfile)
        {
            // Fetch the list of students under the supervisor
            List<Student> studentsUnderSupervisor = ProfileIO.LoadStudentsUnderSupervisor(supervisorProfile.Name);

            if (!studentsUnderSupervisor.Any())
            {
                Console.WriteLine("You have no students assigned to you.");
                return;
            }

            Console.WriteLine("Select a student to view feeling logs:");

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

            Student selectedStudent = studentsUnderSupervisor[choice - 1];
            ViewFeelingLogs(selectedStudent.Name);
        }
        public static void LogFeeling(Student student)
        {
            Console.WriteLine($"Logging feeling for {student.Name}");

            int feelingRating;
            do
            {
                Console.Write("On a scale from 1-10, how are you feeling today? ");
            }
            while (!int.TryParse(Console.ReadLine(), out feelingRating) || feelingRating < 1 || feelingRating > 10);

            // Write the feeling log to a file
            string logEntry = $"{DateTime.Now:yyyy-MM-dd} - {student.Name} - {feelingRating}\n";
            File.AppendAllText("feelings_log.txt", logEntry);

            Console.WriteLine($"Your feeling for today has been logged as {feelingRating}/10.");
        }

        public static void ViewFeelingLogs(string userName)
        {
            if (!File.Exists("feelings_log.txt"))
            {
                Console.WriteLine("No feeling logs available.");
                return;
            }

            var lines = File.ReadAllLines("feelings_log.txt");
            var userFeelings = new List<string>();

            foreach (var line in lines)
            {
                if (line.Contains(userName))
                {
                    userFeelings.Add(line);
                }
            }

            if (userFeelings.Count == 0)
            {
                Console.WriteLine("No feeling logs for this user.");
            }
            else
            {
                Console.WriteLine($"{userName}'s Feelings Logs:");
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
                Console.WriteLine("No upcoming appointments.");
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
                Console.WriteLine("No upcoming appointments.");
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
            if (student.Supervisor == null)
            {
                Console.WriteLine("You don't have a personal supervisor assigned. Please contact the administration.");
                return;
            }

            Console.WriteLine($"Booking appointment with {student.Supervisor.Name}");
            DateTime? appointmentTime = null;

            while (appointmentTime == null)
            {
                Console.Write("Please enter the date for the appointment (yyyy-MM-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    continue;
                }

                if (date.Date < DateTime.Now.Date)
                {
                    Console.WriteLine("The date should be in the future. Please try again.");
                    continue;
                }

                // Generate a list of available time slots for the specified date
                List<DateTime> availableTimeSlots = GenerateAvailableTimeSlots(date);

                if (availableTimeSlots.Count == 0)
                {
                    Console.WriteLine("No available time slots for the selected date. Please choose another date.");
                    continue;
                }

                Console.WriteLine("Available time slots:");
                for (int i = 0; i < availableTimeSlots.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {availableTimeSlots[i]:HH:mm}");
                }

                Console.Write("Select a time slot from the list: ");
                if (int.TryParse(Console.ReadLine(), out int selectedSlot) && selectedSlot >= 1 && selectedSlot <= availableTimeSlots.Count)
                {
                    appointmentTime = availableTimeSlots[selectedSlot - 1];
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please select a valid time slot.");
                }
            }

            // Write the appointment to a file
            File.AppendAllText("appointments.txt", $"{appointmentTime:yyyy-MM-dd HH:mm} - {student.Supervisor.Name} with {student.Name}\n");
            Console.WriteLine($"Your appointment with {student.Supervisor.Name} is booked for {appointmentTime:yyyy-MM-dd HH:mm}");
        }

        private static List<DateTime> GenerateAvailableTimeSlots(DateTime date)
        {
            List<DateTime> availableTimeSlots = new List<DateTime>();

            for (DateTime time = date.AddHours(9); time <= date.AddHours(17); time = time.AddMinutes(30))
            {
                if (!IsTimeSlotTaken(time))
                {
                    availableTimeSlots.Add(time);
                }
            }

            return availableTimeSlots;
        }


        private static bool IsTimeSlotTaken(DateTime time)
        {
            if (!File.Exists("appointments.txt"))
                return false;

            foreach (var line in File.ReadAllLines("appointments.txt"))
            {
                if (DateTime.TryParseExact(line.Split(' ')[0] + " " + line.Split(' ')[1], "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out DateTime existingTime) && existingTime == time)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<Student> ViewStudentList(UserProfile profile)
        {
            var students = ProfileIO.LoadStudentsUnderSupervisor(profile.Name);
            if (students.Count == 0)
            {
                Console.WriteLine("No students assigned to you currently.");
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
                Console.WriteLine("You have no students assigned to you.");
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

        public static void GiveFeedback()
        {

        }
        private static void ViewOverallProgress() { }
        private static void ViewSupervisorInteractions() { }
        public static void ChangePassword(UserProfile profile, string fileName)
        {
            Console.WriteLine("Enter your current password:");
            string currentPassword = Console.ReadLine();

            if (currentPassword != profile.Password)
            {
                Console.WriteLine("Incorrect password. Returning to menu.");
                return;
            }

            Console.WriteLine("Enter your new password:");
            string newPassword = Console.ReadLine();
            profile.Password = newPassword;

            var profiles = ProfileIO.LoadProfiles(fileName);
            var userToUpdate = profiles.FirstOrDefault(p => p.Name == profile.Name);
            if (userToUpdate != null)
            {
                userToUpdate.Password = newPassword;
                ProfileIO.SaveProfiles(fileName, profiles);
                Console.WriteLine("Password successfully updated!");
            }
        }
        public static void ChangePersonalSupervisor(Student student)
        {
            Console.WriteLine("Select a new personal supervisor:");

            var availableSupervisors = ProfileIO.LoadProfiles("ps.txt").Select(p => new PersonalSupervisor { Name = p.Name }).ToList();

            for (int i = 0; i < availableSupervisors.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableSupervisors[i].Name}");
            }

            int choice;
            do
            {
                Console.Write("Enter your choice: ");
            }
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > availableSupervisors.Count);

            student.Supervisor = availableSupervisors[choice - 1];
            Console.WriteLine($"Your personal supervisor is now {student.Supervisor.Name}!");

            // Save the change to the file
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
        public string Status { get; set; }
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
