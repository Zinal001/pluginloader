using System;
using System.IO;
using System.Net;

namespace PluginLoader
{
    public static class VersionInfo
    {
        private readonly static string gitRepo = "Bosek/pluginloader";
        private readonly static string gitURL = $"https://api.github.com/repos/{gitRepo}/git/refs/heads/master";
        public readonly static string GitLink = $"https://github.com/{gitRepo}";
        public static string GetVersionString()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var major = assembly.GetName().Version.Major;
            var minor = assembly.GetName().Version.Minor;
            var commit = GetCommitHash();

            return $"{major}.{minor}_{commit}";
        }

        public static string GetCommitHash()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.commithash.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadLine();
            }
        }

        public static string GetLatestCommitHash()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(gitURL);
                request.UserAgent = gitRepo;
                var response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic data = serializer.DeserializeObject(reader.ReadToEnd());
                    return ((string)data["object"]["sha"]).Substring(0, 7);
                }
            }
            catch (Exception)
            {
                return GetCommitHash();
            }
        }
    }
}
