using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace CloudQATests
{
    /// <summary>
    /// Provides resilient element location strategies that work even when HTML attributes or positions change.
    /// Uses multiple fallback strategies to find elements reliably.
    /// </summary>
    public class ResilientLocator
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ResilientLocator(IWebDriver driver, TimeSpan timeout)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, timeout);
        }

        /// <summary>
        /// Finds an input element by trying multiple strategies in order of reliability.
        /// Strategy order: Label text → Placeholder → Name → ID → CSS selector
        /// </summary>
        public IWebElement FindInputByLabel(string labelText, string? placeholder = null, string? name = null, string? id = null)
        {
            try
            {
                // Strategy 1: Find by associated label text (most resilient)
                // This works because labels are user-facing and rarely change
                return _wait.Until(driver =>
                {
                    try
                    {
                        var label = driver.FindElement(By.XPath($"//label[contains(normalize-space(text()), '{labelText}')]"));
                        var forAttribute = label.GetAttribute("for");
                        if (!string.IsNullOrEmpty(forAttribute))
                        {
                            return driver.FindElement(By.Id(forAttribute));
                        }
                        // If no 'for' attribute, try finding input as next sibling or child
                        return label.FindElement(By.XPath("following-sibling::input | .//input"));
                    }
                    catch
                    {
                        // Fallback strategies
                        if (!string.IsNullOrEmpty(placeholder))
                        {
                            try { return driver.FindElement(By.XPath($"//input[@placeholder='{placeholder}']")); }
                            catch { }
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            try { return driver.FindElement(By.Name(name)); }
                            catch { }
                        }
                        if (!string.IsNullOrEmpty(id))
                        {
                            try { return driver.FindElement(By.Id(id)); }
                            catch { }
                        }
                        throw new NoSuchElementException($"Could not find input with label '{labelText}'");
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"Timeout waiting for input with label '{labelText}'");
            }
        }

        /// <summary>
        /// Finds a radio button by label text and value.
        /// Uses label text relationship which is resilient to attribute changes.
        /// </summary>
        public IWebElement FindRadioByLabelAndValue(string groupLabel, string optionText)
        {
            return _wait.Until(driver =>
            {
                try
                {
                    // Strategy 1: Try finding by ID (often the lowercase version of the text)
                    try
                    {
                        var radioById = driver.FindElement(By.Id(optionText.ToLower()));
                        if (radioById.GetAttribute("type") == "radio")
                        {
                            return radioById;
                        }
                    }
                    catch { }
                    
                    // Strategy 2: Find by text that appears after the input
                    // Look for text node containing the option text, then find preceding radio input
                    try
                    {
                        var xpath = $"//text()[contains(normalize-space(.), '{optionText}')]/preceding-sibling::input[@type='radio'][1]";
                        return driver.FindElement(By.XPath(xpath));
                    }
                    catch { }
                    
                    // Strategy 3: Find the radio input associated with the option text via label
                    // This approach uses the visible text which is stable
                    try
                    {
                        var radioLabel = driver.FindElement(By.XPath($"//label[contains(normalize-space(text()), '{optionText}')]"));
                        
                        // Try to find associated input via 'for' attribute
                        var forAttribute = radioLabel.GetAttribute("for");
                        if (!string.IsNullOrEmpty(forAttribute))
                        {
                            return driver.FindElement(By.Id(forAttribute));
                        }
                        
                        // Fallback: find input as sibling or child
                        return radioLabel.FindElement(By.XPath("preceding-sibling::input[@type='radio'] | following-sibling::input[@type='radio'] | .//input[@type='radio']"));
                    }
                    catch { }
                    
                    // Strategy 4: Last resort - find by value attribute matching the option text
                    try
                    {
                        return driver.FindElement(By.XPath($"//input[@type='radio' and @value='{optionText}']"));
                    }
                    catch
                    {
                        throw new NoSuchElementException($"Could not find radio button for '{optionText}' in group '{groupLabel}'");
                    }
                }
                catch
                {
                    throw new NoSuchElementException($"Could not find radio button for '{optionText}' in group '{groupLabel}'");
                }
            });
        }

        /// <summary>
        /// Finds a checkbox by its associated label text.
        /// Resilient to attribute and position changes.
        /// </summary>
        public IWebElement FindCheckboxByLabel(string labelText)
        {
            return _wait.Until(driver =>
            {
                try
                {
                    // Find label containing the text
                    var label = driver.FindElement(By.XPath($"//label[contains(normalize-space(text()), '{labelText}')]"));
                    
                    // Try 'for' attribute first
                    var forAttribute = label.GetAttribute("for");
                    if (!string.IsNullOrEmpty(forAttribute))
                    {
                        return driver.FindElement(By.Id(forAttribute));
                    }
                    
                    // Fallback: find checkbox as sibling or child
                    return label.FindElement(By.XPath("preceding-sibling::input[@type='checkbox'] | following-sibling::input[@type='checkbox'] | .//input[@type='checkbox']"));
                }
                catch
                {
                    // Last resort: try finding by value
                    try
                    {
                        return driver.FindElement(By.XPath($"//input[@type='checkbox' and @value='{labelText}']"));
                    }
                    catch
                    {
                        throw new NoSuchElementException($"Could not find checkbox with label '{labelText}'");
                    }
                }
            });
        }

        /// <summary>
        /// Waits for an element to be clickable.
        /// </summary>
        public IWebElement WaitForClickable(IWebElement element)
        {
            return _wait.Until(driver =>
            {
                if (element.Displayed && element.Enabled)
                {
                    return element;
                }
                throw new ElementNotInteractableException("Element not clickable yet");
            });
        }
    }
}
