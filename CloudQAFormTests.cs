using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace CloudQATests
{
    [TestFixture]
    public class CloudQAFormTests
    {
        private IWebDriver? _driver;
        private ResilientLocator? _locator;
        private const string FormUrl = "https://app.cloudqa.io/home/AutomationPracticeForm";

        [SetUp]
        public void Setup()
        {
            // Setup ChromeDriver using WebDriverManager (automatically downloads correct driver version)
            new DriverManager().SetUpDriver(new ChromeConfig());
            
            // Initialize Chrome driver with options
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            
            // Initialize resilient locator with 15 second timeout
            _locator = new ResilientLocator(_driver, TimeSpan.FromSeconds(15));
            
            // Navigate to the form
            _driver.Navigate().GoToUrl(FormUrl);
        }

        [TearDown]
        public void TearDown()
        {
            // Close browser after each test
            _driver?.Quit();
            _driver?.Dispose();
        }

        /// <summary>
        /// Test 1: First Name Field
        /// Tests text input functionality using resilient label-based locator.
        /// This test will work even if the input's id, name, or position changes.
        /// </summary>
        [Test]
        [Category("TextInput")]
        public void TestFirstNameField()
        {
            // Arrange
            const string testFirstName = "John";
            
            // Act - Find element using resilient locator (by label text)
            var firstNameInput = _locator!.FindInputByLabel(
                labelText: "First Name",
                placeholder: "First Name",  // Fallback option
                name: "firstname"            // Additional fallback
            );
            
            // Clear any existing value and enter test data
            firstNameInput.Clear();
            firstNameInput.SendKeys(testFirstName);
            
            // Assert - Verify the value was entered correctly
            Assert.That(firstNameInput.GetAttribute("value"), Is.EqualTo(testFirstName),
                "First Name field should contain the entered value");
            
            // Additional assertion - verify field is displayed and enabled
            Assert.That(firstNameInput.Displayed, Is.True, 
                "First Name field should be visible");
            Assert.That(firstNameInput.Enabled, Is.True, 
                "First Name field should be enabled");
            
            Console.WriteLine($"✓ Successfully entered and verified First Name: {testFirstName}");
        }

        /// <summary>
        /// Test 2: Gender Radio Button Selection
        /// Tests radio button selection using resilient label-based locator.
        /// This test will work even if radio button attributes or positions change.
        /// </summary>
        [Test]
        [Category("RadioButton")]
        public void TestGenderSelection()
        {
            // Arrange
            const string genderOption = "Male";
            
            // Act - Find radio button using resilient locator (by label text)
            var maleRadioButton = _locator!.FindRadioByLabelAndValue(
                groupLabel: "Gender",
                optionText: genderOption
            );
            
            // Ensure element is clickable
            _locator.WaitForClickable(maleRadioButton);
            
            // Click the radio button
            maleRadioButton.Click();
            
            // Assert - Verify the radio button is selected
            Assert.That(maleRadioButton.Selected, Is.True,
                $"{genderOption} radio button should be selected");
            
            // Additional test: Select a different option and verify
            var femaleRadioButton = _locator.FindRadioByLabelAndValue(
                groupLabel: "Gender",
                optionText: "Female"
            );
            femaleRadioButton.Click();
            
            // Verify new selection
            Assert.That(femaleRadioButton.Selected, Is.True,
                "Female radio button should be selected");
            Assert.That(maleRadioButton.Selected, Is.False,
                "Male radio button should be deselected when Female is selected");
            
            Console.WriteLine($"✓ Successfully selected and verified Gender radio buttons");
        }

        /// <summary>
        /// Test 3: Hobbies Checkbox Selection
        /// Tests checkbox selection using resilient label-based locator.
        /// This test will work even if checkbox attributes or positions change.
        /// </summary>
        [Test]
        [Category("Checkbox")]
        public void TestHobbiesSelection()
        {
            // Arrange
            string[] hobbiesToSelect = { "Reading", "Cricket" };
            
            // Act & Assert - Select multiple hobbies
            foreach (var hobby in hobbiesToSelect)
            {
                // Find checkbox using resilient locator (by label text)
                var hobbyCheckbox = _locator!.FindCheckboxByLabel(hobby);
                
                // Ensure element is clickable
                _locator.WaitForClickable(hobbyCheckbox);
                
                // Verify initially unchecked (or get current state)
                bool initialState = hobbyCheckbox.Selected;
                
                // Click to select
                hobbyCheckbox.Click();
                
                // Verify checkbox is now selected
                Assert.That(hobbyCheckbox.Selected, Is.True,
                    $"{hobby} checkbox should be selected after clicking");
                
                Console.WriteLine($"✓ Successfully selected hobby: {hobby}");
            }
            
            // Additional test: Verify multiple checkboxes can be selected simultaneously
            var readingCheckbox = _locator!.FindCheckboxByLabel("Reading");
            var cricketCheckbox = _locator.FindCheckboxByLabel("Cricket");
            
            Assert.That(readingCheckbox.Selected && cricketCheckbox.Selected, Is.True,
                "Multiple hobbies should be selectable at the same time");
            
            // Test unselecting
            readingCheckbox.Click();
            Assert.That(readingCheckbox.Selected, Is.False,
                "Reading checkbox should be deselected after clicking again");
            Assert.That(cricketCheckbox.Selected, Is.True,
                "Cricket checkbox should remain selected");
            
            Console.WriteLine($"✓ Successfully verified multiple checkbox selection and deselection");
        }
    }
}
