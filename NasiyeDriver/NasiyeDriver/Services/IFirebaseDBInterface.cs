using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NasiyeDriver.Models;

namespace NasiyeDriver.Services
{
    public interface IFirebaseDBInterface
    {
        void SetValue(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null);
        void UpdateTripDriverLocation(string uid, object local);
        void SubmitTripRating(string key, double rating, string comment);


        void GetProfile<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetCheckProfile<T>(string nodeKey, Action<T> OnValueEvent = null);

        void GetDrivers<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetRequests<T>(string nodeKey, Action<T> OnValueEvent = null);
        void GetOnline(string uid);
        void GetOffline(string user);
        void GetSavedTrips<T>(string nodeKey, Action<T> OnValueEvent = null);

        //

        void RemoveGetRequests(string nodeKey);
        void RemoveGetProfile(string nodeKey);
        void RemoveGetCheckProfile(string nodeKey);
        void RemoveGetDrivers(string nodeKey);
        void RemoveGetSavedTrips(string nodeKey);

        //

        void CancelRequest(string uid);
        void UpdateTripInfo(string key, string v);
        void UpdateTripLocation(string key, object v);
        void UpdateTripDuration(string key, string v);
        void UpdateTripDistance(string key, string v);
        void UpdatePausedTime(string key, string v);
        void UpdateTripAmount(string key, string v);



        Task<string> StartTrip(string key, object trip);
        void GetBusy(string user);
        void GetTrips<T>(string nodeKey, Action<T> OnValueEvent = null);
        void SubmitFeedback(object feedback);
    }
}
