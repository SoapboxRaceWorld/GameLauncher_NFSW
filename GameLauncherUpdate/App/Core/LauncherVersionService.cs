using GameLauncherUpdater.App.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameLauncherUpdater.App.Core
{
    class LauncherVersionService
    {
        private GitHubClient _gitHubClient;

        public LauncherVersionService()
        {
            _gitHubClient = new GitHubClient();
        }

        public async Task<LatestDownloadableRelease> GetLatestVersion()
        {
            var lastRelease = await _gitHubClient.GetLastReleaseDownloadLink("SoapboxRaceWorld", "GameLauncher_NFSW");

            if (lastRelease == null)
                return null;

            var launcherAsset = lastRelease.Assets.FirstOrDefault(t => t.Name == "Release_" + ParseVersion(lastRelease.TagName) + ".zip");

            if (launcherAsset == null)
                return null;

            return new LatestDownloadableRelease
            {
                Url = launcherAsset.Url,
                Version = ParseVersion(lastRelease.TagName)
            };
        }

        private static Version ParseVersion(string gitTag)
        {
            var regex = new Regex(@"v(?<major>\d{1,})(\.(?<minor>\d{1,}))?(\.(?<patch>\d{1,}))?(\.(?<build>\d{1,}))?");
            var match = regex.Match(gitTag);

            if (!match.Success)
                throw new Exception($"Tag {gitTag} has an unrecognized version format");

            var major = GetMatchFromGroupOrDefault(match.Groups, "major", "0");
            var minor = GetMatchFromGroupOrDefault(match.Groups, "minor", "0");
            var patch = GetMatchFromGroupOrDefault(match.Groups, "patch", "0");
            var build = GetMatchFromGroupOrDefault(match.Groups, "build", "0");

            return new Version($"{major}.{minor}.{patch}.{build}");
        }

        private static string GetMatchFromGroupOrDefault(GroupCollection group, string name, string defautlValue)
        {
            return group[name].Success ? group[name].ToString() : defautlValue;
        }
    }
}
