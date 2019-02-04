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

        private static readonly byte[] CryptoKey = Encoding.UTF8.GetBytes("CoursesDownloaderSafeCredentials");

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
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var credential = DecryptCredential(line);
                        if (credential.Target == target)
                        {
                            return (credential.UserName, credential.Password);
                        }
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


        private static void SaveCredentials(IEnumerable<Credential> credentials)
        {
            using (var fileStream = File.Open(CredentialsPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var credential in credentials)
                    {
                        var encryptedCredential = EncryptCredential(credential);
                        streamWriter.WriteLine(encryptedCredential);
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
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var credential = DecryptCredential(line);
                        credentials.Add(credential);
                    }
                }
            }

            return credentials;
        }

        #region Crypto Helping Methods
        
        private static string EncryptCredential(Credential credential)
        {
            var encryptedString = Encrypt(credential.ToString(), CryptoKey);
            return encryptedString;
        }

        private static Credential DecryptCredential(string encryptedCredential)
        {
            var decrypted = Decrypt(encryptedCredential, CryptoKey);
            var credentialParts = decrypted.Split('\n');
            var credential = new Credential(credentialParts[0], credentialParts[1], credentialParts[2]);
            return credential;
        }

        #endregion

        #region Crypto

        private static string Encrypt(string text, byte[] cryptoKey)
        {
            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(cryptoKey, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        private static string Decrypt(string encryptedText, byte[] cryptoKey)
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(cryptoKey, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        #endregion
    }
}