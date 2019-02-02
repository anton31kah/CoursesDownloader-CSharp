using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Client.Helpers;
using CoursesDownloader.Common.ExtensionMethods;
using CoursesDownloader.SharedVariables;
using CredentialManagement;
using HtmlAgilityPack;

namespace CoursesDownloader.Client
{
    public static class CoursesClient
    {
        private const string CASTarget = "https://cas.finki.ukim.mk/";
        private static ProgressMessageHandler _downloadProgressTrackingHandler;

        private static DateTime LoginTime { get; set; }
        public static HttpClient SessionClient { get; private set; }
        
        public static void AddEvent(EventHandler<HttpProgressEventArgs> downloadProgressEvent)
        {
            _downloadProgressTrackingHandler.HttpReceiveProgress += downloadProgressEvent;
        }

        public static void RemoveEvent(EventHandler<HttpProgressEventArgs> downloadProgressEvent)
        {
            _downloadProgressTrackingHandler.HttpReceiveProgress -= downloadProgressEvent;
        }

        private static async Task CreateSession()
        {
            _downloadProgressTrackingHandler = new ProgressMessageHandler(new HttpClientHandler());

            SessionClient = new HttpClient(_downloadProgressTrackingHandler)
            {
                BaseAddress = new Uri("http://courses.finki.ukim.mk/"),
            };

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

                CredentialUtil.SetCredentials(CASTarget, username, password, PersistanceType.Enterprise);
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

                    FindSessKey(await response.GetTextNow()); 
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
            if (SessionClient == null || LoginTime == default(DateTime))
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

        public static async Task<HttpClient> LazyRefresh()
        {
            await Init();
            return SessionClient;
        }

        public static void Dispose()
        {
            _downloadProgressTrackingHandler.Dispose();
            SessionClient.Dispose();
        }
    }
}
