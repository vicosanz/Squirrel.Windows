using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Squirrel.SimpleSplat;

namespace Squirrel
{
    public class GitHubFileDownloader : IFileDownloader, IEnableLogger
    {
        private string gitUser;
        private string gitRepo;
        private Release latest;
        private GitHubClient github;

        public GitHubFileDownloader(GitHubClient github, string gitUser, string gitRepo, Octokit.Release latest)
        {
            this.github = github;
            this.gitUser = gitUser;
            this.gitRepo = gitRepo;
            this.latest = latest;
        }

        public async Task DownloadFile(string url, string targetFile, Action<int> progress)
        {
            try
            {
                this.Log().Info("Downloading file: " + url);
                await this.WarnIfThrows(
                    async () =>
                    {
                        var response = await github.Connection.Get<object>(new Uri(url), new Dictionary<string, string>(), "application/octet-stream");
                        File.WriteAllBytes(targetFile, (byte[])response.HttpResponse.Body);
                        progress(100);
                    },
                    "Failed downloading URL: " + url);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> DownloadUrl(string url)
        {
            try
            {
                this.Log().Info("Downloading url: " + url);


                return await this.WarnIfThrows(async () =>
                {
                    var response = await github.Connection.GetRaw(new Uri(url), new Dictionary<string, string>());
                    return (byte[])response.HttpResponse.Body;

                },
                    "Failed to download url: " + url);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetDownloadUrl(ReleaseEntry releaseEntry)
        {
            string result = latest.Assets.Where(x => string.Equals(x.Name, releaseEntry.Filename, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Url;
            return result;
        }
    }
}
