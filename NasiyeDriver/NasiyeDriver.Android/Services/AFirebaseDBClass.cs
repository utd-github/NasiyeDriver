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
using NasiyeDriver.Models;

[assembly: Dependency(typeof(AFirebaseDBClass))]

namespace NasiyeDriver.Droid.Services
{
    class AFirebaseDBClass : IFirebaseDBInterface
    {
        Dictionary<string, DatabaseReference> DatabaseReferences;
        FirebaseAuth mAuth = FirebaseAuth.GetInstance(MainActivity.app);


        Dictionary<string, IValueEventListener> ValueEventListeners;
        Dictionary<string, IValueEventListener> DValueEventListeners;
        Dictionary<string, IValueEventListener> RValueEventListeners;
        Dictionary<string, IValueEventListener> TValueEventListeners;
        Dictionary<string, IValueEventListener> PValueEventListeners;
        Dictionary<string, IValueEventListener> CPValueEventListeners;


        public AFirebaseDBClass()
        {
            DatabaseReferences = new Dictionary<string, DatabaseReference>();

            ValueEventListeners = new Dictionary<string, IValueEventListener>();
            DValueEventListeners = new Dictionary<string, IValueEventListener>();
            RValueEventListeners = new Dictionary<string, IValueEventListener>();
            TValueEventListeners = new Dictionary<string, IValueEventListener>();
            PValueEventListeners = new Dictionary<string, IValueEventListener>();
            CPValueEventListeners = new Dictionary<string, IValueEventListener>();
        }

        private DatabaseReference GetDatabaseReference(string nodeKey)
        {
            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                return DatabaseReferences[nodeKey];
            }
            else
            {
                DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);
                DatabaseReferences[nodeKey] = dr;
                return dr;
            }
        }

        public void RemoveGetDrivers<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);



            if (dr != null)
            {
                ValueEventListener<T> listener = new ValueEventListener<T>(action);
                dr.AddValueEventListener(listener);

                ValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetDrivers<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            if (dr != null)
            {
                DValueEventListener<T> listener = new DValueEventListener<T>(action);
                dr.OrderByChild("Status").EqualTo("Online").AddValueEventListener(listener);
                if (DValueEventListeners != null)
                {
                    DValueEventListeners.Remove(nodeKey);
                }

                DValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetCheckProfile<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);

            if (dr != null)
            {
                CPValueEventListener<T> listener = new CPValueEventListener<T>(action);

                dr.AddValueEventListener(listener);

                if (CPValueEventListeners != null)
                {
                    CPValueEventListeners.Remove(nodeKey);
                }
                CPValueEventListeners.Add(nodeKey, listener);
            }
        }
        
        public void GetProfile<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);

            if (dr != null)
            {
                PValueEventListener<T> listener = new PValueEventListener<T>(action);

                dr.AddValueEventListener(listener);

                if (PValueEventListeners != null)
                {
                    PValueEventListeners.Remove(nodeKey);
                }
                PValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void SetValue(string nodeKey, object obj, Action onSuccess = null, Action<string> onError = null)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference(nodeKey);
            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(obj);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                dr.SetValue(dataHashMap);
            }

        }

        public void GetOnline(string uid)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("drivers");
            if (dr != null)
            {
                var driverref = dr.Child(uid);
                _ = driverref.Child("Status").SetValue("Online").IsComplete;
            }
        }

        public void GetOffline(string user)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("drivers");
            if (dr != null)
            {
                var driverref = dr.Child(user);
                _ = driverref.Child("Status").SetValue("Offline").IsComplete;
            }
        }

        public void GetBusy(string user)
        {
          
                DatabaseReference dr = FirebaseDatabase.Instance.GetReference("drivers");
                if (dr != null)
                {
                    var driverref = dr.Child(user);
                    _ = driverref.Child("Status").SetValue("Busy").IsComplete;
                }
         }

        public void UpdateTripDriverLocation(string uid, object local)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("drivers");
            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(local);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                dr.Child(uid).Child("Location").SetValue(dataHashMap);
            }
        }

        public void GetRequests<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);
            if (dr != null)
            {

                RValueEventListener<T> listener = new RValueEventListener<T>(action);

                dr.AddValueEventListener(listener);

                if (RValueEventListeners != null)
                {
                    RValueEventListeners.Remove(nodeKey);
                }

                RValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetTrips<T>(string nodeKey, Action<T> action = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);

            if (dr != null)
            {
                TValueEventListener<T> listener = new TValueEventListener<T>(action);
                dr.AddValueEventListener(listener);

                if (TValueEventListeners != null)
                {
                    TValueEventListeners.Remove(nodeKey);
                }
                TValueEventListeners.Add(nodeKey, listener);
            }
        }

        public void GetSavedTrips<T>(string nodeKey, Action<T> OnValueEvent = null)
        {
            DatabaseReference dr = GetDatabaseReference(nodeKey);

            if (dr != null)
            {
                var user = mAuth.CurrentUser;

                if (user != null)
                {
                    TValueEventListener<T> listener = new TValueEventListener<T>(OnValueEvent);

                    dr.OrderByChild("Driver/Key").EqualTo(user.Uid).AddValueEventListener(listener);

                    TValueEventListeners.Add(nodeKey, listener);
                }
            }
        }

        public void AcceptrReq(string uid)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("requests");
            if (dr != null)
            {
                var request = dr.Child(uid);
                request.Child("Status").SetValue("Accepted");
            }
        }

        public void CancelRequest(string uid)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("requests");
            if (dr != null)
            {
                var request = dr.Child(uid);
                request.Child("Status").SetValue("Canceled");
            }
        }

        public void UpdateTripInfo(string key, string v)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
            if (dr != null)
            {
                var request = dr.Child(key);
                request.Child("Status").SetValue(v);
            }
        }

        public void UpdateTripDuration(string key, string v)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
            if (dr != null)
            {
                var duration = dr.Child(key);
                duration.Child("Duration").SetValue(v);
            }
        }

        public void UpdateTripLocation(string key, object v)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
            if (dr != null)
            {
                var duration = dr.Child(key);

                string objJsonString = JsonConvert.SerializeObject(v);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                duration.Child("Location").SetValue(dataHashMap);
            }
        }

        public  void UpdatePausedTime(string key, string v)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
            if (dr != null)
            {
              
                var duration = dr.Child(key);
                duration.Child("PauseTime").SetValue(v);
            }
        }

         public  void UpdateTripDistance(string key, string v)
        {
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
            if (dr != null)
            {
              
                var duration = dr.Child(key);
                duration.Child("Distance").SetValue(v);
            }
        }


        public  void UpdateTripAmount(string key, string v)
        {
                DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");
                if (dr != null)
                {
              
                    var duration = dr.Child(key);
                    duration.Child("Amount").SetValue(v);
                }
        }



        public void SubmitFeedback(object feedback)
        {

            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("feedbacks");

            if (dr != null)
            {
                string objJsonString = JsonConvert.SerializeObject(feedback);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();

                var Key = dr.Push().Key;

                var feedbackref = dr.Child(Key);

                feedbackref.SetValue(dataHashMap);
            }

        }

        public async void SubmitTripRating(string key, double rating, string comment)
        {
            // Get Ref
            DatabaseReference dr = FirebaseDatabase.Instance.GetReference("trips");


            if (dr != null)
            {
                var useruid = await new AFirebaseAuthClass().GetCurrentUser();
                GetOnline(useruid);

                var trip = dr.Child(key);
                var user = trip.Child("User");
                var info = trip.Child("Info");

                info.SetValue(comment);
                user.Child("Rating").SetValue(rating);
            }
        }

        public async Task<string> StartTrip(string key, object trip)
        {
            DatabaseReference tripref = FirebaseDatabase.Instance.GetReference("trips");
            DatabaseReference driverref = FirebaseDatabase.Instance.GetReference("drivers");

            DatabaseReference reqref = FirebaseDatabase.Instance.GetReference("requests").Child(key);

            string uid = await new AFirebaseAuthClass().GetCurrentUser();

            var Key = tripref.Push().Key;
            // update request
            reqref.Child("Key").SetValue(Key);
            // With key
            if(tripref != null)
            {
                string objJsonString = JsonConvert.SerializeObject(trip);

                Gson gson = new GsonBuilder().SetPrettyPrinting().Create();

                HashMap dataHashMap = new HashMap();

                Java.Lang.Object jsonObj = gson.FromJson(objJsonString, dataHashMap.Class);

                dataHashMap = jsonObj.JavaCast<HashMap>();
                tripref.Child(Key).SetValue(dataHashMap);
                tripref.Child(Key).Child("Key").SetValue(Key);

                reqref.Child("Status").SetValue("Accepted");
                driverref.Child(uid).Child("Trip").SetValue(uid);

                return Key;
            }
            // start trip
            return null;
        }

        // Remove from listeners from dictionary
        
        public void RemoveGetRequests(string nodeKey)
        {
            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(RValueEventListeners[nodeKey]);

                    if (RValueEventListeners.ContainsKey(nodeKey))
                    {
                        RValueEventListeners.Remove(nodeKey);

                    }
                }
            }

        }

        public void RemoveGetProfile(string nodeKey)
        {
            DatabaseReference dr = DatabaseReferences[nodeKey];


            if (dr != null)
            {
                dr.RemoveEventListener(DValueEventListeners[nodeKey]);
                DValueEventListeners.Remove(nodeKey);
            }
        }

        public void RemoveGetCheckProfile(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(CPValueEventListeners[nodeKey]);
                    if (CPValueEventListeners.ContainsKey(nodeKey))
                    {
                        CPValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }

        public void RemoveGetDrivers(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(DValueEventListeners[nodeKey]);

                    if (DValueEventListeners.ContainsKey(nodeKey))
                    {
                        DValueEventListeners.Remove(nodeKey);
                    }
                }
            }
        }

        public void RemoveGetSavedTrips(string nodeKey)
        {

            if (DatabaseReferences.ContainsKey(nodeKey))
            {
                DatabaseReference dr = DatabaseReferences[nodeKey];

                if (dr != null)
                {
                    dr.RemoveEventListener(TValueEventListeners[nodeKey]);
                    if (TValueEventListeners.ContainsKey(nodeKey))
                    {
                        TValueEventListeners.Remove(nodeKey);

                    }
                }
            }
        }

    }

    public class DValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public DValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
              // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class RValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public RValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class TValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public TValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class CPValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public CPValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class PValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public PValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }

    public class ValueEventListener<T> : Java.Lang.Object, IValueEventListener
    {
        public Action<T> action;

        public ValueEventListener(Action<T> action)
        {
            this.action = action;
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists() && snapshot.HasChildren)
            {
                HashMap dataHashMap = snapshot.Value.JavaCast<HashMap>();
                Gson gson = new GsonBuilder().Create();
                string data = gson.ToJson(dataHashMap);
                // Try to deserialize :
                try
                {
                    T chatItems = JsonConvert.DeserializeObject<T>(data);
                    action(chatItems);
                }
                catch
                {

                }
            }
            else
            {
                T item = default(T);
                action(item);
            }
        }
    }
}