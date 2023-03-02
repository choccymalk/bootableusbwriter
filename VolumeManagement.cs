using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Management;

namespace VolumeManagement
{
    #region Volume Management Win32 API Wrapper
    public class VolumeAPI
    {
        public struct VolumeInformation
        {
            public string Identifier;
            public string Name;
            public string FileSystem;
            public uint SerialNumber;
            public uint Flags;
            public uint MaximumComponentLength;
            public string MountPath;
            public int SCSIPort;
            public int SCSIBus;
            public int SCSITargetId { get; set; }

            public override string ToString()
            {
                return string.Format("Volume: {0}\nName: {1}\nSystem: {2}\nSNr: {3}\nFlags: {4}\nMCL: {5}\nPath: {6}\nSCSI {7}.{8}.{9}",Identifier,Name,FileSystem,SerialNumber,Flags,MaximumComponentLength,MountPath, SCSIBus,SCSIPort,SCSITargetId);
            }

           

            public override bool Equals(object obj)
            {
                try
                {
                    var vi = (VolumeInformation)obj;
                    if (((VolumeInformation)obj).Identifier == Identifier) return true;
                }
                catch
                {
                }

                return false;
            }

        }

        private const int MAX_PATH = 260;

        // Finding Volumes
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindFirstVolume(StringBuilder volumeName, UInt32 vnLength);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        private static extern bool FindNextVolume(IntPtr handle, StringBuilder volumeName, UInt32 vnLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindVolumeClose(IntPtr handle);

        //Retreiving Information
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string volume, StringBuilder volumeName, UInt32 vnLength, out UInt32 volumeSerialNumber, out UInt32 maximumComponentLength, out UInt32 fileSystemFlags, StringBuilder fileSystemName, UInt32 fsnLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumePathNamesForVolumeName(string volumeName, StringBuilder volumePathNames, UInt32 vpnLength, out UInt32 returnLength);


        public static IEnumerable<string> GetVolumeNames()
        {    
            var sbOut = new StringBuilder(MAX_PATH);
            var pHandle = FindFirstVolume(sbOut, MAX_PATH);

            var retList = new List<string>();
            do
            {
                if(!retList.Contains(sbOut.ToString()))
                    retList.Add(sbOut.ToString());
            }
            while (FindNextVolume(pHandle, sbOut, MAX_PATH));

            FindVolumeClose(pHandle);
            return retList;

        }

        public static List<VolumeInformation> GetVolumeInformation()
        {
            var retList = new List<VolumeInformation>();
            foreach (var volume in GetVolumeNames())
            {
                retList.Add(GetVolumeInformation(volume));
            }
            return retList;
        }

        private static ManagementBaseObject GetDiskDriveFromDriveLetter(string driveLetter)
        {
            ManagementObjectCollection disks = new ManagementObjectSearcher("select * from Win32_LogicalDisk where Name='" + driveLetter + "'").Get();

            foreach (ManagementObject disk in disks)
            {
                foreach (ManagementObject partition in disk.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementBaseObject diskDrive in partition.GetRelated("Win32_DiskDrive"))
                    {                        
                        return diskDrive;
                    }
                }
            }
            throw new ArgumentException("cannot trace disk, partition or drive for " + driveLetter);
        }

        public static VolumeInformation GetVolumeInformation(string volume)
        {          
            var sbName = new StringBuilder(MAX_PATH);
            var sbSystem = new StringBuilder(MAX_PATH);

            var ret = new VolumeInformation
                        {                       
                            Identifier = volume,
                            MountPath = GetVolumePath(volume)
                        };

            GetVolumeInformation(volume, sbName, MAX_PATH, out ret.SerialNumber, out ret.MaximumComponentLength, out ret.Flags, sbSystem, MAX_PATH);
               // throw new IOException("Failed retreiving Volume Information for "+volume);

            

            ret.Name = sbName.ToString();
            ret.FileSystem = sbSystem.ToString();

            //get additional WMI info
            if (ret.MountPath.Length >= 2)
                try 
	            {
                    var disk = GetDiskDriveFromDriveLetter(ret.MountPath.Substring(0, 2));
                    ret.SCSIPort = (UInt16)disk["SCSIPort"];
                    ret.SCSIBus = (int)(UInt32)disk["SCSIBus"];
                    ret.SCSITargetId = (UInt16)disk["SCSITargetId"];
	            }
	            catch(Exception)
	            {
                    ret.SCSIPort = -1;
                    ret.SCSIBus = -1;
                    ret.SCSITargetId = -1;
	            }
                            
            else
                ret.SCSIPort = -1;

            return ret;
                        
        }

        public static string GetVolumePath(string volume)
        {
           uint retLen;
            var sbOut = new StringBuilder(MAX_PATH);

            if (!GetVolumePathNamesForVolumeName(volume, sbOut, MAX_PATH, out retLen))
                return "[NONE]";//throw new IOException("Failed retreiving Volume Paths");

            return sbOut.ToString();
        }
    }
    #endregion

    #region VolumeManager

    public class VolumeManager:IDisposable
    {
        //required for insert or remove events of volumes
        internal class VolumeMonitor : NativeWindow
        {
            const int WM_DEVICECHANGE = 0x0219;

            VolumeManager hostManager;

            public enum DeviceEvent : int
            {
                Arrival = 0x8000,           //DBT_DEVICEARRIVAL
                QueryRemove = 0x8001,       //DBT_DEVICEQUERYREMOVE
                QueryRemoveFailed = 0x8002, //DBT_DEVICEQUERYREMOVEFAILED
                RemovePending = 0x8003,     //DBT_DEVICEREMOVEPENDING
                RemoveComplete = 0x8004,    //DBT_DEVICEREMOVECOMPLETE
                Specific = 0x8005,          //DBT_DEVICEREMOVECOMPLETE
                Custom = 0x8006,            //DBT_CUSTOMEVENT
                NodeChanged = 0x7           //DBT_DEVNODES_CHANGED
            }

            public enum DeviceType : int
            {
                OEM = 0x00000000,           //DBT_DEVTYP_OEM
                DeviceNode = 0x00000001,    //DBT_DEVTYP_DEVNODE
                Volume = 0x00000002,        //DBT_DEVTYP_VOLUME
                Port = 0x00000003,          //DBT_DEVTYP_PORT
                Net = 0x00000004            //DBT_DEVTYP_NET
            }

            public enum VolumeFlags : int
            {
                Media = 0x0001,             //DBTF_MEDIA
                Net = 0x0002                //DBTF_NET
            }

            public struct BroadcastHeader   //_DEV_BROADCAST_HDR 
            {
                public int Size;            //dbch_size
                public DeviceType Type;     //dbch_devicetype
                private int Reserved;       //dbch_reserved
            }

            public struct Volume            //_DEV_BROADCAST_VOLUME 
            {
                public int Size;            //dbcv_size
                public DeviceType Type;     //dbcv_devicetype
                private int Reserved;       //dbcv_reserved
                public int Mask;            //dbcv_unitmask
                public int Flags;           //dbcv_flags
            }

            public VolumeMonitor(VolumeManager host)
            {
                hostManager = host;
            }

            protected override void WndProc(ref Message aMessage)
            {
              //  DeviceEvent devEvent;

                base.WndProc(ref aMessage);
            /*    if (aMessage.Msg == WM_DEVICECHANGE && hostManager.Enabled)
                {
                    devEvent = (DeviceEvent)aMessage.WParam.ToInt32();
                    if (devEvent == DeviceEvent.Arrival || devEvent == DeviceEvent.RemoveComplete || devEvent == DeviceEvent.NodeChanged)
                    {
                        hostManager.RefreshList(null, null);                        
                    }
                }*/
            }
        }

        public void Refresh()
        {
            RefreshList(null, null);
        }

        private VolumeMonitor monitor;

        public delegate void VolumeAction(VolumeAPI.VolumeInformation volume);

        public event VolumeAction OnVolumeInserted;
        public event VolumeAction OnVolumeRemoved;
        public event VolumeAction OnVolumeChanged;

        private bool enabled = false;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
               

                if (!enabled && value)
                {
                    if (usePolling)
                    {
                        pollTimer.Start();
                    }
                    else
                    {

                        mewModify.Start();
                        mewCreate.Start();
                        mewDelete.Start();
                    }
                    if (monitor.Handle == IntPtr.Zero) { monitor.AssignHandle(handle); }                 
                }
                if (enabled && !value)
                {
                    if (usePolling)
                    {
                        pollTimer.Stop(); 
                    }
                    else
                    {
                        
                       // mewModify.Stop();
                       // mewCreate.Stop();
                       // mewDelete.Stop();
                    }
                    if (monitor.Handle != IntPtr.Zero) { monitor.ReleaseHandle(); }
                }

                enabled = value;
            }
        }

        internal void RefreshList(object sender, EventArgs e)
        {
            var newList = VolumeAPI.GetVolumeInformation();

            bool removed = false;
            //1st: check for ejected volumes
            var removeList = new List<VolumeAPI.VolumeInformation>();
            foreach (var volume in Volumes)
            {
                if (!newList.Contains(volume))
                {
                    //remember in remove list, as iterating and deleting will not work the same time
                    removeList.Add(volume);
                }

            }
            foreach (var volume in removeList)
            {
                Volumes.Remove(volume);
                OnVolumeRemoved(volume);
                removed = true;
            }

            if (removed) return; //finished work if a drive was removed

            bool inserted = false;
            //2nd: check for inserted volumes or changes
            foreach (var volume in newList)
            {
                //not yet in list?
                if (!Volumes.Contains(volume))
                {
                    Volumes.Add(volume);
                    OnVolumeInserted(volume);
                    inserted = true;
                }
                else
                {

                }
            }

            if (inserted) return; //finished work if a drive was inserted
        }

        internal void ListChanged(object sender, EventArgs e)
        {
            //okay, some volume changed
            bool changed = false;
            VolumeAPI.VolumeInformation nv = new VolumeAPI.VolumeInformation();
            foreach (var item in Volumes)
            {
                nv = VolumeAPI.GetVolumeInformation(item.Identifier);
                if (nv.Name != item.Name ||
                   nv.MountPath != item.MountPath ||
                   nv.SerialNumber != item.SerialNumber ||
                   nv.FileSystem != item.FileSystem ||
                   nv.Flags != item.Flags ||
                   nv.MaximumComponentLength != item.MaximumComponentLength)
                {
                    //something differs -> tell it
                    changed = true;
                    break;                   
                }
            }
            if (changed)
            {
                Volumes = VolumeAPI.GetVolumeInformation();
                OnVolumeChanged(nv);
            }
            

        }

        public List<VolumeAPI.VolumeInformation> Volumes
        {
            get;
            private set;
        }

        private IntPtr handle;

        ManagementEventWatcher mewModify = null;
        ManagementEventWatcher mewCreate = null;
        ManagementEventWatcher mewDelete = null;
        //use WMI events for triggering new/removed drives;
        public VolumeManager(IntPtr hWnd)
        {
            Volumes = new List<VolumeAPI.VolumeInformation>();
            Volumes.AddRange(VolumeAPI.GetVolumeInformation());
            monitor = new VolumeMonitor(this);
            handle = hWnd;

            //ignoring the event type, use own algoritm to determine what happened.
            //this is necessary as the "change" event can occur in different situations. dam-it!
            AddHandler(ref mewModify, "__InstanceModificationEvent", "Win32_DiskDrive", RefreshList);
            AddHandler(ref mewCreate, "__InstanceCreationEvent", "Win32_DiskDrive", RefreshList);
            AddHandler(ref mewDelete, "__InstanceDeletionEvent", "Win32_DiskDrive", RefreshList);
   
            //AddChangeHandler();

        }

        //poll for changes - as eventing is not functioning well in Win7
        private bool usePolling = false;

        public VolumeManager(int pollingInterval)
        {
            usePolling = true;
            Volumes = new List<VolumeAPI.VolumeInformation>();
            Volumes.AddRange(VolumeAPI.GetVolumeInformation());
            monitor = new VolumeMonitor(this);

            pollTimer = new System.Timers.Timer();
            pollTimer.AutoReset = true; //run more than once
            pollTimer.Interval = pollingInterval;
            PollingInterval = pollingInterval;
            pollTimer.Elapsed += new System.Timers.ElapsedEventHandler(RefreshList);           
        }
        public int PollingInterval { get; private set; }
        
        System.Timers.Timer pollTimer;


        void AddHandler(ref ManagementEventWatcher w, string eventClass, string targetClass, EventArrivedEventHandler handler)
        {

            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;
          
            q = new WqlEventQuery();
            q.EventClassName = eventClass;
            q.WithinInterval = new TimeSpan(0, 0, 3);
            q.Condition = "TargetInstance ISA '"+targetClass+"'";
            w = new ManagementEventWatcher(scope, q);
            w.EventArrived += handler;                
        }



        #region IDisposable Members

       public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(monitor.Handle!=IntPtr.Zero) 
                { 
                    monitor.ReleaseHandle(); 
                    monitor = null;
                }
                if (this.Enabled)
                    this.Enabled = false;

                //mewChange.Dispose();
               if(mewModify!=null)
                mewModify.Dispose();
            }
            disposed = true;         
        }
        private bool disposed = false;
        
        ~VolumeManager()      
        {
            Dispose(false);
        }
        
        #endregion
    }

    #endregion
}
