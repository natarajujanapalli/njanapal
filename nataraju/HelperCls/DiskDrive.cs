using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace nataraju
{
    internal class DiskDrive
    {
        public string Availability { get; set; } = string.Empty;
        public string BytesPerSector { get; set; } = string.Empty;
        public string Capabilities { get; set; } = string.Empty;
        public string CapabilityDescriptions { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string CompressionMethod { get; set; } = string.Empty;
        public string ConfigManagerErrorCode { get; set; } = string.Empty;
        public Boolean ConfigManagerUserConfig { get; set; } 
        public string CreationClassName { get; set; } = string.Empty;
        public string DefaultBlockSize { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DeviceID { get; set; } = string.Empty;
        public Boolean ErrorCleared { get; set; }
        public string ErrorDescription { get; set; } = string.Empty;
        public string ErrorMethodology { get; set; } = string.Empty;
        public string FirmwareRevision { get; set; } = string.Empty;
        public string Index { get; set; } = string.Empty;
        public DateTime InstallDate { get; set; }
        public string InterfaceType { get; set; } = string.Empty;
        public string LastErrorCode { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string MaxBlockSize { get; set; } = string.Empty;
        public string MaxMediaSize { get; set; } = string.Empty;
        public Boolean MediaLoaded { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string MinBlockSize { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Boolean NeedsCleaning { get; set; }
        public string NumberOfMediaSupported { get; set; } = string.Empty;
        public int Partitions { get; set; }
        public string PNPDeviceID { get; set; } = string.Empty;
        public string PowerManagementCapabilities { get; set; } = string.Empty;
        public Boolean PowerManagementSupported { get; set; }
        public string SCSIBus { get; set; } = string.Empty;
        public string SCSILogicalUnit { get; set; } = string.Empty;
        public string SCSIPort { get; set; } = string.Empty;
        public string SCSITargetId { get; set; } = string.Empty;
        public string SectorsPerTrack { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusInfo { get; set; } = string.Empty;
        public string SystemCreationClassName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string TotalCylinders { get; set; } = string.Empty;
        public string TotalHeads { get; set; } = string.Empty;
        public string TotalSectors { get; set; } = string.Empty;
        public string TotalTracks { get; set; } = string.Empty;
        public string TracksPerCylinder { get; set; } = string.Empty;


        public DiskDrive GetDiskDriveInfo(string remoteComputerNodeName)
        {
            DiskDrive _diskDrive = new DiskDrive();

            ConnectionOptions connection = new ConnectionOptions();

            ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_DiskDrive ");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject os in searcher.Get())
            {
                _diskDrive.Size = $"{(int)Math.Round(Convert.ToDouble(os["Size"].ToString()) / (1024 * 1024 * 1024))} GB";
                _diskDrive.Partitions = Convert.ToInt32(os["Partitions"].ToString());

                break;
            }

            return _diskDrive;
        }
    }
}
