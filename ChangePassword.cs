using Microsoft.VisualBasic.FileIO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace SimpleSelenium
{
    public class ChangePassword
    {
        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Invalid Parameter");
                return;
            }

            var path = @args[0];
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                // csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string username = fields[0];
                    string oldPass = fields[1];
                    string newPass = fields[2];
                    Console.WriteLine($"{username}, {oldPass}, {newPass}");
                    RunChangePassword(username, newPass, oldPass);
                }
            }

            Console.WriteLine();
        }

        private static void RunChangePassword(string currentUser, string newPass, string oldPass)
        {
            Console.WriteLine($"current user: {currentUser}");
            var driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://cpwm.ais.co.th/ApplicationUser/ChangePassword");
            Console.WriteLine($"current url: {driver.Url}");

            var title = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[3]"));
            if (null != title && title.Text == "Change Password")
            {
                var username = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[4]/div/input"));
                if (null != username)
                {
                    username.SendKeys(currentUser);
                    System.Threading.Thread.Sleep(1000);
                }

                var password = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[5]/div[1]/input"));
                var passwordview = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[5]/div[1]/span"));
                if (null != password && null != passwordview)
                {
                    passwordview.Click();
                    System.Threading.Thread.Sleep(1000);
                    password.SendKeys(oldPass);
                    System.Threading.Thread.Sleep(1000);
                }
                var new_password = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[6]/div/input"));
                var new_passwordview = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[6]/div/span"));
                if (null != password && null != passwordview)
                {
                    new_passwordview.Click();
                    System.Threading.Thread.Sleep(1000);
                    new_password.SendKeys(newPass);
                    System.Threading.Thread.Sleep(1000);
                }
                var c_new_password = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[7]/div[1]/input"));
                var c_new_passwordview = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[7]/div[1]/span"));
                if (null != password && null != passwordview)
                {
                    c_new_passwordview.Click();
                    System.Threading.Thread.Sleep(1000);
                    c_new_password.SendKeys(newPass);
                    System.Threading.Thread.Sleep(1000);
                }

                var submit = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[9]/button"));
                if (null != submit && submit.Text == "Submit")
                {
                    //c_new_password.Clear();
                    submit.Click();
                }

                try
                {
                    var errorlegion = driver.FindElement(By.XPath("/html/body/form/div/div/div/div[2]/div[8]/div"));
                    wait.Until(d => errorlegion.Displayed);
                }
                catch (StaleElementReferenceException se)
                {
                    Console.WriteLine($"changed url: {driver.Url}");
                    // Console.WriteLine(se.ToString());
                    var success = driver.FindElement(By.XPath("/html/body/div/div/div/div[2]/div[2]/div[2]"));
                    if (null != success)
                    {
                        if (success.Displayed)
                        {
                            Console.WriteLine($"{success.Text}");
                            System.Threading.Thread.Sleep(3000);
                            driver.Quit();
                        }
                    }
                }
                catch (WebDriverTimeoutException we)
                {
                    System.Threading.Thread.Sleep(3000);
                    Console.WriteLine("Quiting With Error");
                    Console.WriteLine(we.ToString());
                    driver.Quit();
                }
            }
            else
            {
                Console.WriteLine("Invalid Page");
                driver.Quit();
            }
        }
    }
}
