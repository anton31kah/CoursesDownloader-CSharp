using CredentialManagement;

namespace CoursesDownloader.Client.Helpers
{
    public static class CredentialUtil
    {
        public static (string Username, string Password) GetCredential(string target)
        {
            var credential = new Credential { Target = target };
            return !credential.Load() ? (null, null) : (credential.Username, credential.Password);
        }

        public static bool SetCredentials(string target, string username, string password, PersistanceType persistenceType)
        {
            return new Credential
            {
                Target = target,
                Username = username,
                Password = password,
                PersistanceType = persistenceType
            }.Save();
        }

        public static bool RemoveCredentials(string target)
        {
            return new Credential { Target = target }.Delete();
        }
    }
}