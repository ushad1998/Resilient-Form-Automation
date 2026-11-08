# Resilient Form Automation Using C# and Selenium

## Project Overview

This project automates the CloudQA Automation Practice Form using C# and Selenium WebDriver.

The automation identifies form fields using their visible labels and HTML relationships instead of depending entirely on fixed IDs, names, or CSS classes.

## Website

Default URL:  
https://app.cloudqa.io/home/AutomationPracticeForm

The URL can also be configured using the `FORM_URL` environment variable.

## Technologies Used

- C#
- .NET 10
- Selenium WebDriver
- Selenium Support
- Google Chrome

## Automated Test Scenarios

The automation performs the following actions:

1. Identifies the First Name field and enters `Usha`.
2. Finds and selects the Female radio button.
3. Identifies the State dropdown and selects `India`.
4. Verifies that all three fields contain the expected values.
5. Prints PASS or FAIL results in the terminal.

## Resilience Features

The implementation uses:

- Visible label text to identify form controls.
- HTML label associations using the `for` attribute.
- Nested element and sibling relationships as alternative locator strategies.
- Flexible element identification when IDs and names change.
- Container identification to distinguish actual fields from duplicate or decoy fields.
- A configurable URL to support different test pages.

## Prerequisites

Install the following:

- .NET SDK 10.0
- Google Chrome
- Visual Studio Code (or another C# IDE)

Selenium Manager can manage the ChromeDriver installation when supported by the installed Selenium version.

## Installation

Clone or download the project and open its folder in the terminal.

Install the required NuGet packages if they are not already installed:

```powershell
dotnet add package Selenium.WebDriver
dotnet add package Selenium.Support
```

Restore dependencies:

```powershell
dotnet restore
```

## How to Run

Build the project:

```powershell
dotnet build
```

Run using the default CloudQA URL:

```powershell
dotnet run
```

To run against a different page, configure the URL:

```powershell
$env:FORM_URL = "https://example.com/your-test-page"
dotnet run
```

To return to the default URL:

```powershell
Remove-Item Env:FORM_URL -ErrorAction SilentlyContinue
dotnet run
```

## Expected Output

```text
--- AUTOMATION RESULTS ---
First Name: PASS
Gender: PASS
State: PASS

--- SELECTED ELEMENT DETAILS ---
First Name ID: fname
Gender ID: female
Gender value: Female
State ID: state
```

The actual element IDs may differ when running against modified test pages.

## Test Coverage

The implementation has been tested against:

- Original CloudQA practice form.
- Modified field IDs and names.
- Reordered form fields.
- Duplicate and decoy fields.
- Modified HTML structures and nested elements.

**Limitations:** Dedicated iframe and Shadow DOM handling, including nested Shadow DOM and iframe-inside-Shadow-DOM scenarios, has not been implemented or tested.

## Project Structure

```text
Resilent-Form-Automation/
│
├── Program.cs
├── Resilent-Form-Automation.csproj
├── README.md
└── bin/
└── obj/
```

## Conclusion

This project demonstrates resilient browser form automation using C# and Selenium WebDriver, with flexible element identification, configurable test URLs, and automated result verification.