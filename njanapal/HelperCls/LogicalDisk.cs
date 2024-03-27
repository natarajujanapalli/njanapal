using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
    public class LogicalDisk
    {
        public String Access { get; set; }
        public String Availability { get; set; }
        public String BlockSize { get; set; }
        public String Caption { get; set; }
        public String Compressed { get; set; }
        public String ConfigManagerErrorCode { get; set; }
        public String ConfigManagerUserConfig { get; set; }
        public String CreationClassName { get; set; }
        public String Description { get; set; }
        public String DeviceID { get; set; }
        public String DriveType { get; set; }
        public String ErrorCleared { get; set; }
        public String ErrorDescription { get; set; }
        public String ErrorMethodology { get; set; }
        public String FileSystem { get; set; }
        public String FreeSpace { get; set; }
        public String FreeSpacePercentage { get; set; }

        public String InstallDate { get; set; }
        public String LastErrorCode { get; set; }
        public String MaximumComponentLength { get; set; }
        public String MediaType { get; set; }
        public String Name { get; set; }
        public String NumberOfBlocks { get; set; }
        public String PNPDeviceID { get; set; }
        public String[] PowerManagementCapabilities { get; set; }
        public String PowerManagementSupported { get; set; }
        public String ProviderName { get; set; }
        public String Purpose { get; set; }
        public String QuotasDisabled { get; set; }
        public String QuotasIncomplete { get; set; }
        public String QuotasRebuilding { get; set; }
        public String Size { get; set; }
        public String Status { get; set; }
        public String StatusInfo { get; set; }
        public String SupportsDiskQuotas { get; set; }
        public String SupportsFileBasedCompression { get; set; }
        public String SystemCreationClassName { get; set; }
        public String SystemName { get; set; }
        public String VolumeDirty { get; set; }
        public String VolumeName { get; set; }
        public String VolumeSerialNumber { get; set; }
    }

}
