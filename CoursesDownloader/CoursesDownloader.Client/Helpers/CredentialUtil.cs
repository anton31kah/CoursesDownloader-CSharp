using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CoursesDownloader.Common.ExtensionMethods;

namespace CoursesDownloader.Client.Helpers
{
    public static class CredentialUtil
    {
        /* File format:
         * when we encrypt the credential we get byte[]
         * each byte is 8 bits, and each bit is made to look like a 4 digit hex number
         * so 130d is 10000010b, stored as 0001 0000 0000 0000 0000 0000 0001 0000
         * we store these separated by zero lines
         * and credentials are separated by one lines
         *
         * zero line 0000 0000 0000 0000 0000 0000 0000 0000
         * byte 0001 0000 0000 0000 0000 0000 0001 0000
         * zero line
         * byte
         * ...
         * zero line
         * byte
         * zero
         * one line 1111 1111 1111 1111 1111 1111 1111 1111
         * ... and the same is repeated
         */

        private class Credential
        {
            public string Target { get; }
            public string UserName { get; }
            public string Password { get; }

            public Credential(string target, string userName = null, string password = null)
            {
                Target = target;
                UserName = userName;
                Password = password;
            }

            public override string ToString()
            {
                return new[] {Target, UserName, Password}.Join();
            }
        }

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("CoursesDownloaderCredentials");

        private static readonly string CredentialsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CoursesDownloader");
        private static readonly string CredentialsPath = Path.Combine(CredentialsDirectory, "credentials");

        public static (string Username, string Password) GetCredential(string target)
        {
            Directory.CreateDirectory(CredentialsDirectory);

            if (!File.Exists(CredentialsPath))
            {
                return (null, null);
            }
            
            using (var fileStream = File.Open(CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    Credential credential = null;
                    var bytes = new List<byte>();

                    var zeroLine = true; // first line is zero line

                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // break line
                        if (line.Where(c => c != ' ').All(c => c == '1'))
                        {
                            credential = DecryptCredential(bytes);
                            if (credential.Target == target)
                            {
                                break;
                            }
                            continue;
                        }

                        if (!zeroLine)
                        {
                            var byteNumber = LineToByte(line);
                            bytes.Add(byteNumber);
                        }

                        zeroLine = !zeroLine;
                    }

                    if (credential != null)
                    {
                        return (credential.UserName, credential.Password);
                    }
                }
            }

            return (null, null);
        }

        public static void SetCredentials(string target, string username, string password)
        {
            Directory.CreateDirectory(CredentialsDirectory);

            var credentials = GetAllCredentials();

            var foundCredential = credentials.FirstOrDefault(cred => cred.Target == target);
            
            if (foundCredential == null) // create new credential
            {   
                credentials.Add(new Credential(target, username, password));
            }
            else if(foundCredential.UserName == username && foundCredential.Password == password) // no need to do anything
            {   
                return;
            }
            else // overwrite
            {
                var foundCredentialIndex = credentials.IndexOf(foundCredential);
                credentials[foundCredentialIndex] = new Credential(target, username, password);
            }

            SaveCredentials(credentials);
        }

        public static void RemoveCredentials(string target)
        {
            Directory.CreateDirectory(CredentialsDirectory);

            var credentials = GetAllCredentials();

            var foundCredential = credentials.FirstOrDefault(cred => cred.Target == target);

            if (foundCredential == null) // no need to do anything
            {
                return;
            }

            credentials.Remove(foundCredential);
            
            SaveCredentials(credentials);
        }


        private static void SaveCredentials(List<Credential> credentials)
        {
            using (var fileStream = File.Open(CredentialsPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    var zeroLine = Enumerable.Repeat("0000", 8).Join(" ");
                    var oneLine = Enumerable.Repeat("1111", 8).Join(" ");

                    foreach (var credential in credentials)
                    {
                        var encryptCredentialBytes = EncryptCredential(credential);
                        foreach (var credentialByte in encryptCredentialBytes)
                        {
                            var byteLine = ByteToLine(credentialByte);

                            streamWriter.WriteLine(zeroLine);
                            streamWriter.WriteLine(byteLine);
                        }

                        streamWriter.WriteLine(zeroLine);
                        streamWriter.WriteLine(oneLine);
                    }
                }
            }
        }

        private static List<Credential> GetAllCredentials()
        {
            Directory.CreateDirectory(CredentialsDirectory);

            var credentials = new List<Credential>();

            if (!File.Exists(CredentialsPath))
            {
                return credentials;
            }

            using (var fileStream = File.Open(CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var bytes = new List<byte>();

                    var zeroLine = true; // first line is zero line

                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // break line
                        if (line.Where(c => c != ' ').All(c => c == '1'))
                        {
                            var credential = DecryptCredential(bytes);
                            credentials.Add(credential);
                            bytes.Clear();
                            continue;
                        }

                        if (!zeroLine)
                        {
                            var byteNumber = LineToByte(line);
                            bytes.Add(byteNumber);
                        }

                        zeroLine = !zeroLine;
                    }
                }
            }

            return credentials;
        }

        #region Crypto Helping Methods
        
        private static string ByteToLine(byte credentialByte)
        {
            var byteLine = Convert.ToString(credentialByte, 2)
                .PadLeft(8, '0')
                .Select(c => $"{c}".PadLeft(4, '0'))
                .Join(" ");
            return byteLine;
        }

        private static byte LineToByte(string line)
        {
            var binaryDigits = line.Split(' ');
            var binaryNumber = binaryDigits.Select(d => d[3]).Join("");
            var byteNumber = Convert.ToByte(binaryNumber, 2);
            return byteNumber;
        }

        private static byte[] EncryptCredential(Credential credential)
        {
            var bytes = Encoding.UTF8.GetBytes(credential.ToString());
            var encryptedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
            return encryptedBytes;
        }

        private static Credential DecryptCredential(List<byte> bytes)
        {
            var decrypted = ProtectedData.Unprotect(bytes.ToArray(), Entropy, DataProtectionScope.CurrentUser);
            var credentialJoined = Encoding.UTF8.GetString(decrypted);
            var credentialParts = credentialJoined.Split('\n');
            var credential = new Credential(credentialParts[0], credentialParts[1], credentialParts[2]);
            return credential;
        }
        
        #endregion
    }
}