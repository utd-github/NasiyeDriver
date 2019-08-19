using Android.Runtime;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using GoogleGson;
using Java.Util;
using NasiyeDriver.Droid.Services;
using NasiyeDriver.Droid.Models;
using NasiyeDriver.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;

[assembly: Dependency(typeof(AFirebaseDBClass))]

namespace NasiyeDriver.Droid.Services
{
    class AFirebaseDBClass : IFirebaseDBInterface
    {
        public static int SucessCode = 0;
        public static int ErrorCode = 0;

        FirebaseAuth mAuth = FirebaseAuth.GetInstance(MainActivity.app);

        FirebaseDatabase DBInstance = FirebaseDatabase.GetInstance(MainActivity.app);

        // Profile methods
        public void GetUserProfile(string uid)
        {
            DatabaseReference userref = DBInstance.GetReference("drivers");
            userref.Child(uid).AddValueEventListener(new UserProfileValListener());
        }

        // Get online Methods
        public Task<bool> GetOnline(string uid)
        {
            var users = DBInstance.GetReference("drivers").Child(uid);

            return Task.FromResult(users.Child("Status").SetValue("Online").IsComplete);
        }

        public Task<bool> UpdateDriverLocation(string uid, object location)
        {
            var users = DBInstance.GetReference("drivers").Child(uid);
            var locationref = users.Child("Location");

            Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

            HashMap dataHashMap = new HashMap();

            string objJsonString = JsonConvert.SerializeObject(location);

            Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

            dataHashMap = jsonObj.JavaCast<HashMap>();

            return Task.FromResult(locationref.SetValue(dataHashMap).IsSuccessful);
        }

        public Task<bool> GetOffline(string uid)
        {
            var users = DBInstance.GetReference("drivers");

            return Task.FromResult(users.Child(uid).Child("Status").SetValue("Offline").IsSuccessful);
        }

        public void ListenToquest(string uid)
        {
            var users = DBInstance.GetReference("requests");

            users.Child(uid).AddValueEventListener(new RequestVuleListener());

        }

        public Task<bool> CancelReuest(string uid)
        {
            var requests = DBInstance.GetReference("requests");

            return Task.FromResult(requests.Child(uid).Child("Status").SetValue("Canceled").IsSuccessful);
        }

        public Task<bool> AcceptrReq(string uid)
        {
            var requests = DBInstance.GetReference("requests");

            return Task.FromResult(requests.Child(uid).Child("Status").SetValue("Accepted").IsSuccessful);
        }



        // LIsteners
        private class UserProfileValListener : Java.Lang.Object, IValueEventListener
        {

            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                HashMap map = snapshot.Value.JavaCast<HashMap>();

                Gson gson = new GsonBuilder().Create();
                string userprofile = gson.ToJson(map);


                if (userprofile != null)
                {
                    MessagingCenter.Send<object, string>(this, "profile", userprofile);
                }

            }
        }

        private class RequestVuleListener : Java.Lang.Object, IValueEventListener
        {
            public void OnCancelled(DatabaseError error)
            {
                
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    HashMap map = snapshot.Value.JavaCast<HashMap>();

                    Gson gson = new GsonBuilder().Create();
                    string reqstring = gson.ToJson(map);

                    if (reqstring != null)
                    {
                        MessagingCenter.Send<object, string>(this, "request", reqstring);
                    }
                }
                
            }
        }
    }
}