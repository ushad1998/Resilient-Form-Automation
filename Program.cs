using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;

string url = Environment.GetEnvironmentVariable("FORM_URL")
    ?? "https://app.cloudqa.io/home/AutomationPracticeForm";

var options = new ChromeOptions();
options.AddArgument("--start-maximized");
options.PageLoadStrategy = PageLoadStrategy.Eager;

using IWebDriver driver = new ChromeDriver(options);
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

try
{
    driver.Navigate().GoToUrl(url);

    // Identify the correct group containing all three fields.
    IWebElement container = wait.Until(d => FindMainContainer(d))
        ?? throw new Exception(
            "Could not identify one unambiguous group containing the required fields.");

    IWebElement firstName =
        FindLabelControl(container, "First Name", "input")
        ?? throw new Exception("First Name field not found.");

    IWebElement female =
        FindFemaleRadio(container)
        ?? throw new Exception("Female radio button not found.");

    IWebElement state =
        FindLabelControl(container, "State", "select")
        ?? throw new Exception("State dropdown not found.");

    // Enter First Name.
    firstName.Clear();
    firstName.SendKeys("Usha");

    // Select Female.
    female.Click();

    // Select India from the State dropdown.
    var dropdown = new SelectElement(state);
    dropdown.SelectByText("India");

    // Verify the results.
    bool namePassed = firstName.GetAttribute("value") == "Usha";
    bool genderPassed = female.Selected;
    bool statePassed =
        dropdown.SelectedOption.Text.Trim() == "India";

    Console.WriteLine("\n--- AUTOMATION RESULTS ---");
    Console.WriteLine($"First Name: {(namePassed ? "PASS" : "FAIL")}");
    Console.WriteLine($"Gender: {(genderPassed ? "PASS" : "FAIL")}");
    Console.WriteLine($"State: {(statePassed ? "PASS" : "FAIL")}");

    Console.WriteLine("\n--- SELECTED ELEMENT DETAILS ---");
    Console.WriteLine($"First Name ID: {firstName.GetAttribute("id")}");
    Console.WriteLine($"Gender ID: {female.GetAttribute("id")}");
    Console.WriteLine($"Gender value: {female.GetAttribute("value")}");
    Console.WriteLine($"State ID: {state.GetAttribute("id")}");

    Console.WriteLine("\nPress Enter to close browser.");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine("AUTOMATION FAILED: " + ex.Message);
    Console.ReadLine();
}
finally
{
    driver.Quit();
}


// Finds the smallest matching containers and selects the best candidate.
IWebElement? FindMainContainer(IWebDriver driver)
{
    var candidates = driver.FindElements(By.XPath("//*"))
        .Where(element =>
        {
            try
            {
                return element.Displayed &&
                    FindLabelControl(element, "First Name", "input") != null &&
                    FindFemaleRadio(element) != null &&
                    FindLabelControl(element, "State", "select") != null;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        })
        .ToList();

    // Keep only the smallest matching containers.
    var smallest = candidates.Where(candidate =>
        !candidates.Any(other =>
        {
            if (candidate.Equals(other))
                return false;

            return candidate.FindElements(By.XPath(".//*"))
                .Any(child => child.Equals(other));
        })
    ).ToList();

    if (smallest.Count == 0)
        return null;

    // Prefer a Female radio whose actual HTML value is "Female".
    var preferred = smallest.Where(container =>
    {
        var radio = FindFemaleRadio(container);

        return radio != null &&
            string.Equals(
                radio.GetAttribute("value"),
                "Female",
                StringComparison.OrdinalIgnoreCase);
    }).ToList();

    // If one candidate has the expected radio value, use it.
    if (preferred.Count == 1)
        return preferred[0];

    // If there is only one candidate, use it.
    if (smallest.Count == 1)
        return smallest[0];

    // Multiple equally plausible candidates: do not guess.
    return null;
}


// Finds an input or select using its visible label.
IWebElement? FindLabelControl(
    IWebElement scope,
    string labelText,
    string tagName)
{
    var labels = scope.FindElements(By.TagName("label"))
        .Where(label =>
            label.Displayed &&
            label.Text.Trim().Equals(
                labelText,
                StringComparison.OrdinalIgnoreCase));

    foreach (var label in labels)
    {
        // First try the label's "for" association.
        string? targetId = label.GetAttribute("for");

        if (!string.IsNullOrWhiteSpace(targetId))
        {
            var associated = scope.FindElements(By.Id(targetId))
                .FirstOrDefault(element =>
                    element.Displayed &&
                    element.TagName.Equals(
                        tagName,
                        StringComparison.OrdinalIgnoreCase));

            if (associated != null)
                return associated;
        }

        // Then check for a control nested inside the label.
        var nested = label.FindElements(By.XPath(".//" + tagName))
            .FirstOrDefault(element => element.Displayed);

        if (nested != null)
            return nested;

        // Finally, check the next sibling.
        var sibling = label.FindElements(
            By.XPath($"./following-sibling::{tagName}[1]"))
            .FirstOrDefault(element => element.Displayed);

        if (sibling != null)
            return sibling;
    }

    return null;
}


// Finds the Female radio using its visible label or text.
IWebElement? FindFemaleRadio(IWebElement scope)
{
    // Support the original page where Female is displayed in a span.
    var spans = scope.FindElements(By.TagName("span"))
        .Where(span =>
            span.Displayed &&
            span.Text.Trim().Equals(
                "Female",
                StringComparison.OrdinalIgnoreCase));

    foreach (var span in spans)
    {
        var radio = span.FindElements(
            By.XPath("./preceding-sibling::input[@type='radio'][1]"))
            .FirstOrDefault(element => element.Displayed);

        if (radio != null)
            return radio;
    }

    // Support pages where the radio is inside a label.
    var labels = scope.FindElements(By.TagName("label"))
        .Where(label =>
            label.Displayed &&
            label.Text.Trim().Equals(
                "Female",
                StringComparison.OrdinalIgnoreCase));

    foreach (var label in labels)
    {
        var radio = label.FindElements(
            By.XPath(".//input[@type='radio']"))
            .FirstOrDefault(element => element.Displayed);

        if (radio != null)
            return radio;
    }

    return null;
}