using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Client.Helpers.HttpClientAutoRedirect;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.SharedVariables;
using HtmlAgilityPack;

namespace CoursesDownloader.Client
{
    public static class CoursesClient
    {
        private const string DefaultCASTarget = "https://cas.finki.ukim.mk/";
        private static string CASTarget { get; set; } = DefaultCASTarget;
        private static string TempCASTarget;

        private static DateTime LoginTime { get; set; }
        public static HttpClientAutoRedirect SessionClient { get; private set; }
        
        private static async Task CreateSession()
        {
            CredentialUtil.ClearCredentialsKeep(CASTarget);

            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            SessionClient = new HttpClientAutoRedirect(httpClientHandler)
            {
                BaseAddress = new Uri("http://courses.finki.ukim.mk/"),
            };
            SessionClient.DefaultRequestHeaders.UserAgent.ParseAdd("CoursesDownloader-C# Console App");

            HttpResponseMessage login = null;

            while (true)
            {
                Console.WriteLine("Establishing connection with courses");

                try
                {
                    login = await SessionClient.GetAsync("http://courses.finki.ukim.mk/login/index.php");

                    if (!login.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Connection cannot be established");
                    var shouldRetry = MenuChooseItem.AskYesNoQuestion("Do you want to try again? [Y/N] ",
                        Console.Clear, 
                        () => { Environment.Exit(0); });

                    if (shouldRetry) // onYes
                    {
                        continue;
                    }
                }

                break;
            }

            Console.WriteLine("Preparing CAS login");

            var text = await login.Content.ReadAsStringAsync();
            login.Dispose();

            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            var hiddenInput = doc.DocumentNode.SelectNodes("//form//input[@type=\"hidden\"]");

            var loginData = new Dictionary<string, string>();

            foreach (var x in hiddenInput)
            {
                loginData[x.Attributes.First(t => t.Name == "name").Value] = x.Attributes.First(t => t.Name == "value").Value;
            }
            
            EnterCredentialsAgain:

            var (username, password) = CredentialUtil.GetCredential(CASTarget);

            while (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                username = ConsoleUtils.ReadLine("Please enter your CAS username >>> ", ConsoleIOType.Question);
                password = ConsoleUtils.ReadLine("Please enter your CAS password >>> ", ConsoleIOType.Question, true);
                
                CredentialUtil.SetCredentials(CASTarget, username, password);
            }

            loginData["username"] = username;
            loginData["password"] = password;

            using (var loginDataContent = new FormUrlEncodedContent(loginData.ToArray()))
            {
                Console.WriteLine("Logging into CAS");
                
                using (var response = await SessionClient.PostAsync(login.RequestMessage.RequestUri, loginDataContent))
                {
                    // if redirected to CAS, wrong password or username
                    if (response.RequestMessage.RequestUri.Host == new Uri(CASTarget).Host)
                    {
                        ConsoleUtils.WriteLine("The username or password you entered is incorrect. Please try again");
                        CredentialUtil.RemoveCredentials(CASTarget); // remove incorrect credentials
                        goto EnterCredentialsAgain;
                    }

                    FindSessKey(await response.Content.ReadAsStringAsync()); 
                }
            }
            
            LoginTime = DateTime.Now;
        }

        public static void FindSessKey(string responseContent)
        {
            SharedVars.SessKey = Regex.Match(responseContent, "(?<=sesskey=).{10}").Value;
        }

        private static async Task Init()
        {
            if (SessionClient == null || LoginTime == default)
            {
                await CreateSession();
                return;
            }

            var currentTime = DateTime.Now;
            var difference = currentTime - LoginTime;

            if (difference.TotalMinutes > 30)
            {
                await CreateSession();
            }
        }

        public static async Task<HttpClientAutoRedirect> LazyRefresh()
        {
            await Init();
            return SessionClient;
        }

        public static async Task<HttpClientAutoRedirect> TempLogInUser()
        {
            // a temp user is logged in, log them out first
            if (TempCASTarget != null && CASTarget == TempCASTarget)
            {
                TempLogOutUser();
            }

            TempCASTarget = DefaultCASTarget + "temp";
            CASTarget = TempCASTarget;

            await CreateSession();

            return SessionClient;
        }

        public static void TempLogOutUser()
        {
            if (TempCASTarget != null && CASTarget == TempCASTarget)
            {
                CredentialUtil.RemoveCredentials(TempCASTarget);
            }

            TempCASTarget = null;
            CASTarget = DefaultCASTarget;
            
            CredentialUtil.ClearCredentialsKeep(CASTarget);

            SessionClient.Dispose();
        }

        public static void Dispose()
        {
            // Just in case
            if (TempCASTarget != null)
            {
                CredentialUtil.RemoveCredentials(TempCASTarget);
                TempCASTarget = null;
            }
            
            CASTarget = DefaultCASTarget;

            CredentialUtil.ClearCredentialsKeep(CASTarget);

            SessionClient.Dispose();
        }
    }
}
