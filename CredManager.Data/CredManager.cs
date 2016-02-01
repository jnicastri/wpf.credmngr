using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;

namespace CredManager.Data
{
    public sealed class CredManager
    {
        private const string _DATASTORE_PATH = "_m_Ds\\DSet.bin";
        private const string _CREDSTORE_PATH = "_m_Ds\\CredStore.bin";

        private static object _objLock = new object();
        private static CredManager _instance = null;

        public static CredManager Instance
        {
            get
            {
                lock (_objLock)
                {
                    if (_instance == null)
                        _instance = new CredManager();

                    return _instance;
                }
            }
        }

        public bool IsAuthenticated { get; private set; }
        public ObservableCollection<CredItem> CredItems { get; set; }
        public int Count { get { return CredItems.Count; } }

        CredManager() 
        { 
            this.CredItems = new ObservableCollection<CredItem>();
            IsAuthenticated = false;
        }

        public bool Add(CredItem newItem)
        {
            CredItem existingItem = this.CredItems.Where(i => i.Name == newItem.Name).FirstOrDefault();

            if (existingItem == null)
            {
                this.CredItems.Add(newItem);
                this.SaveCurrentState();
                this.RefreshDataSource();
                return true;
            }
            else
                return false;
        }

        public bool Remove(string keyName)
        {
            CredItem itemToRemove = this.CredItems.Where(i => i.Name == keyName).FirstOrDefault();
            return itemToRemove == null ? false : RemoveKnownMatch(itemToRemove);
        }

        public bool Remove(CredItem credItemReference)
        {
            return (CredItems.Contains(credItemReference)) ? RemoveKnownMatch(credItemReference) : false;
        }

        private bool RemoveKnownMatch(CredItem item)
        {
            CredItems.Remove(item);
            SaveCurrentState();
            return true;
        }

        public bool SaveCurrentState()
        {
            bool operationState = false;
            try
            {
                using (Stream stream = File.Open(_DATASTORE_PATH, FileMode.Truncate))
                {
                    BinaryFormatter binFmtr = new BinaryFormatter();
                    binFmtr.Serialize(stream, this.CredItems);
                }
                operationState = true;
            }
            catch { }

            return operationState;
        }

        public bool AuthenticateCredAccess(string passKey)
        {
            bool operationState = false;
            string storedKey = String.Empty;
            try
            {
                using (Stream stream = File.Open(_CREDSTORE_PATH, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    storedKey = bin.Deserialize(stream).ToString();
                }
                operationState = true;
            }
            catch { return operationState; }

            if (operationState != false && storedKey != String.Empty)
            {
                if (storedKey.ToLower().Trim() == passKey.ToLower().Trim())
                {
                    LoadDataFromDisk();
                    IsAuthenticated = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public void RefreshDataSource()
        {
            LoadDataFromDisk();
        }

        private bool LoadDataFromDisk()
        {
            bool operationState = false;
            try
            {
                using (Stream stream = File.Open(_DATASTORE_PATH, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    this.CredItems = (ObservableCollection<CredItem>)bin.Deserialize(stream);
                }
                operationState = true;
            }
            catch { }
            return operationState;
        }
    }
}
