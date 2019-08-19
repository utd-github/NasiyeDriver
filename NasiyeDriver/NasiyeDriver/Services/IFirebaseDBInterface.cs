using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NasiyeDriver.Models;

namespace NasiyeDriver.Services
{
    public interface IFirebaseDBInterface
    {
        // Profile methods
        void GetUserProfile(String uid);
      
        // Get online
        Task<bool> GetOnline(string uid);
        Task<bool> GetOffline(string uid);
        Task<bool> UpdateDriverLocation(string uid, object location);

        // Listen to request
        void ListenToquest(string uid);

        //Cancel request
        Task<bool> CancelReuest(string uid);
        // Accept Req
        Task<bool> AcceptrReq(string uid);

    }
}
