using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using HiringManagerServer.Models;

namespace HiringManagerServer.Application.Domain
{
    class NativeDirectoryProfileManager : IProfileManager
    {
        private readonly string _profilesDir;
        private readonly object _reloadLock = new Object();
        private readonly object _reloadTaskRwLock = new Object();
        private Task<Profile[]> _reloadTask;
        private Profile[] _profilesCache = new Profile[0];

        public NativeDirectoryProfileManager(IConfiguration configuration, IHostEnvironment env)
        {
            _profilesDir = Path.Combine(env.ContentRootPath, configuration["ProfilePath"]);
        }

        public IEnumerable<ProfileAbstract> GetAvailableProfiles()
        {
            return Reload().Select(ProfileAbstract.Clone);
        }

        public bool GetProfile(long profileId, out Profile profile)
        {
            profile = this._profilesCache.FirstOrDefault(x => x.ProfileId == profileId)
                ?? this.ReloadAndCache().FirstOrDefault(x => x.ProfileId == profileId);
            return profile != null;
        }

        private Profile[] ReloadAndCache()
        {
            var reloadResult = this.Reload();
            this._profilesCache = reloadResult;
            return reloadResult;
        }

        private Profile[] Reload()
        {
            if (Monitor.TryEnter(this._reloadLock))
            {
                lock (this._reloadTaskRwLock)
                {
                    _reloadTask = Task.Run(() => ParseProfiles(this._profilesDir).ToArray());
                }

                Profile[] loaded;
                try
                {
                    loaded = _reloadTask.GetAwaiter().GetResult();
                }
                catch (Exception error)
                {
                    Monitor.Exit(this._reloadLock);
                    throw error;
                }

                Monitor.Exit(this._reloadLock);
                return loaded;
            }
            else
            {
                Task<Profile[]> currentReloadTask;
                lock (this._reloadTaskRwLock)
                {
                    currentReloadTask = this._reloadTask;
                }
                return currentReloadTask.Result;
            }
        }

        private static IEnumerable<Profile> ParseProfiles(string path)
        {
            foreach (var dir in System.IO.Directory.GetFiles(path, "*.json"))
            {
                Profile profile;
                try
                {
                    profile = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(dir));
                }
                catch
                {
                    continue;
                }

                if (profile != null)
                {
                    yield return profile;
                }
            }
        }
    }
}