using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Squirrel.Json;

namespace Squirrel
{
    public sealed partial class UpdateManager
    {
        public static async Task<UpdateManager> GitHubUpdateManager(
            string gitUser,
            string gitRepo,
            string applicationName = null,
            string rootDirectory = null,
            IFileDownloader urlDownloader = null)
        {
            var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Squirrel", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            var latest = await github.Repository.Release.GetLatest(gitUser, gitRepo);
            string latestReleaseUrl = null;
            if (urlDownloader is null)
            {
                urlDownloader = new GitHubFileDownloader(github, gitUser, gitRepo, latest);
                latestReleaseUrl = latest.Assets.Where(x => string.Equals(x.Name, "RELEASES", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().BrowserDownloadUrl;
            }
            return new UpdateManager(latestReleaseUrl, applicationName, rootDirectory, urlDownloader);
        }
    }
}