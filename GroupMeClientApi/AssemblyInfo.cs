using System.Reflection;

// AssemblyVersion = full version info, major.minor.patch
[assembly: AssemblyVersion(GroupMeClientApi.AssemblyInfo.SimpleVersion)]

// FileVersion = full version info, major.minor.patch
[assembly: AssemblyFileVersion(GroupMeClientApi.AssemblyInfo.SimpleVersion)]

// InformationalVersion = full version + branch + commit sha.
[assembly: AssemblyInformationalVersion(GroupMeClientApi.AssemblyInfo.InformationalVersion)]

namespace GroupMeClientApi
{
    /// <summary>
    /// <see cref="AssemblyInfo"/> documents the build version of the GroupMe Client API.
    /// </summary>
    public class AssemblyInfo
    {
        /// <summary>
        /// Simple release-like version number, like 4.0.1.0.
        /// </summary>
        public const string SimpleVersion = ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "." + ThisAssembly.Git.BaseVersion.Patch + "." + ThisAssembly.Git.Commits;

        /// <summary>
        /// Full version, plus branch and commit short sha, like 4.0.1.0-39cf84e-branch.
        /// </summary>
        public const string InformationalVersion = SimpleVersion + "-" + ThisAssembly.Git.Commit + "+" + ThisAssembly.Git.Branch;
    }
}