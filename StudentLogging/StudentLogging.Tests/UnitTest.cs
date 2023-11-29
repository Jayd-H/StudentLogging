using NUnit.Framework;
using StudentLogging;
using System;
using System.IO;
using System.Linq;

namespace StudentLogging.Tests
{
    [TestFixture]
    public class ProfileIOTests
    {
        private const string TestFileName = "test_profile_io.txt";

        [SetUp]
        public void Setup()
        {
            // Ensure the test file is clean before each test
            File.WriteAllText(TestFileName, string.Empty);
        }

        [Test]
        public void SaveProfiles_WhenCalled_WritesToTheFile()
        {
            // Arrange
            var profiles = new List<UserProfile>
            {
                new UserProfile { Name = "TestUser", Password = "TestPassword" }
            };

            // Act
            ProfileIO.SaveProfiles(TestFileName, profiles);

            // Assert
            var fileContent = File.ReadAllLines(TestFileName);
            Assert.That(fileContent.Length, Is.EqualTo(1), "One profile should be written to the file.");
            Assert.That(fileContent[0], Does.Contain("TestUser"), "The file should contain the profile name.");
        }

        [Test]
        public void LoadProfiles_WhenCalled_ReturnsProfilesFromTheFile()
        {
            // Arrange
            string profileData = "TestUser:TestPassword";
            File.WriteAllText(TestFileName, profileData);

            // Act
            var profiles = ProfileIO.LoadProfiles(TestFileName);

            // Assert
            Assert.That(profiles, Has.Count.EqualTo(1), "One profile should be loaded from the file.");
            Assert.That(profiles[0].Name, Is.EqualTo("TestUser"), "The loaded profile should match the file data.");
        }

        [TearDown]
        public void CleanUp()
        {
            // Clean up after tests
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }
    }
}
