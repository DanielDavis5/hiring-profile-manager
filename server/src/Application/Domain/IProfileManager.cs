using System.Collections.Generic;
using HiringManagerServer.Models;

namespace HiringManagerServer.Application.Domain
{
    public interface IProfileManager
    {
        IEnumerable<ProfileAbstract> GetAvailableProfiles();

        bool GetProfile(long profileId, out Profile profile);
    }
}