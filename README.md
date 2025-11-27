# CloudQA Selenium Test Automation

This project contains automated tests for the CloudQA practice form using C# and Selenium WebDriver with **resilient locator strategies** that remain stable even when HTML attributes or element positions change.

## 🎯 Test Coverage

The project includes tests for three different field types:

1. **First Name** (Text Input) - `TestFirstNameField()`
2. **Gender** (Radio Buttons) - `TestGenderSelection()`
3. **Hobbies** (Checkboxes) - `TestHobbiesSelection()`

## 🛡️ Resilient Locator Strategy

### Why Resilient Locators?

Traditional Selenium tests often break when:
- Element IDs or names change
- CSS classes are modified
- Elements are repositioned in the DOM
- HTML structure is refactored

### Our Solution: Multi-Fallback Approach

The `ResilientLocator` class implements a **cascading fallback strategy** that tries multiple methods to find elements:

```
Priority 1: Label Text (Most Resilient)
    ↓ (if fails)
Priority 2: Placeholder Attribute
    ↓ (if fails)
Priority 3: Name Attribute
    ↓ (if fails)
Priority 4: ID Attribute
    ↓ (if fails)
Priority 5: CSS Selector
```

#### Why Label Text is Most Resilient

Label text is the **most stable locator** because:
- ✅ It's user-facing content that rarely changes
- ✅ It's independent of implementation details
- ✅ It works regardless of element position
- ✅ It survives HTML refactoring
- ✅ It's accessible and semantic

### Example Usage

```csharp
// Traditional approach (fragile):
var input = driver.FindElement(By.Id("firstName"));  // Breaks if ID changes

// Resilient approach (robust):
var input = locator.FindInputByLabel(
    labelText: "First Name",      // Primary: Label text
    placeholder: "First Name",    // Fallback 1: Placeholder
    name: "firstname"             // Fallback 2: Name attribute
);
```

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Chrome browser (ChromeDriver is automatically managed)

## 🚀 Running the Tests

### 1. Restore Dependencies

```bash
cd /Users/tushar/Desktop/AssignmentcloudQA
dotnet restore
```

### 2. Run All Tests

```bash
dotnet test
```

### 3. Run with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### 4. Run Specific Test Category

```bash
# Run only text input tests
dotnet test --filter "Category=TextInput"

# Run only radio button tests
dotnet test --filter "Category=RadioButton"

# Run only checkbox tests
dotnet test --filter "Category=Checkbox"
```

## 📁 Project Structure

```
AssignmentcloudQA/
├── CloudQATests.csproj          # Project file with dependencies
├── ResilientLocator.cs          # Resilient element locator class
├── CloudQAFormTests.cs          # Test methods for three fields
└── README.md                    # This file
```

## 🔍 Test Details

### Test 1: First Name Field (`TestFirstNameField`)
- **Type**: Text input
- **Actions**: Clear field, enter text, verify value
- **Assertions**: 
  - Value matches input
  - Field is visible
  - Field is enabled

### Test 2: Gender Selection (`TestGenderSelection`)
- **Type**: Radio buttons
- **Actions**: Select Male, then select Female
- **Assertions**:
  - Selected radio is checked
  - Previously selected radio is unchecked
  - Only one radio can be selected at a time

### Test 3: Hobbies Selection (`TestHobbiesSelection`)
- **Type**: Checkboxes
- **Actions**: Select Reading and Music, then deselect Reading
- **Assertions**:
  - Multiple checkboxes can be selected
  - Checkboxes can be toggled
  - Independent selection state

## 🧪 Verifying Resilience

To verify that tests remain stable despite HTML changes:

1. **Run the tests normally** - All should pass
2. **Modify the HTML** (using browser DevTools):
   - Change element IDs
   - Change CSS classes
   - Reorder elements
   - Modify attributes
3. **Re-run the tests** - They should still pass!

The tests will continue working as long as the **label text** remains the same (which is the user-facing content).

## 📦 Dependencies

- **NUnit 3.14.0** - Testing framework
- **Selenium.WebDriver 4.16.2** - Browser automation
- **WebDriverManager 2.17.2** - Automatic driver management
- **Microsoft.NET.Test.Sdk 17.8.0** - Test execution

## 🎓 Key Concepts Demonstrated

1. **Page Object Pattern** (via ResilientLocator class)
2. **Multiple Locator Strategies** (fallback chain)
3. **Explicit Waits** (WebDriverWait)
4. **Test Organization** (Setup/TearDown, Categories)
5. **Comprehensive Assertions** (multiple checks per test)
6. **Clean Code Practices** (clear naming, comments, logging)

