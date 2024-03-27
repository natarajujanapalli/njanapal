using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
    public class RegEdit
    {
        public string GetRegistryKey(string virtualMachine, string registryKey)
        {
            string RegistryKeyValue = string.Empty;

            try
            {
                //using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\MySQL AB\\MySQL Connector\\Net"))
                //{
                //    if (key != null)
                //    {
                //        Object o = key.GetValue("Version");
                //        if (o != null)
                //        {
                //            Version version = new Version(o as String);  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                //                                                         //do what you like with version
                //        }
                //    }
                //}
                String path = "SOFTWARE\\Microsoft\\Virtual~\\Guest\\Parameters";

                RegistryKey rkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "IN-ISECBLD");
                RegistryKey rkeyVirtualMachine = rkey.OpenSubKey("SOFTWARE\\Microsoft\\Virtual Machine\\Guest\\Parameters");

                RegistryKey rkeySoftware = rkey.OpenSubKey("SOFTWARE");
                RegistryKey rkeyMicrosoft = rkeySoftware.OpenSubKey("Microsoft");
                RegistryKey rkeyVM = rkeyMicrosoft.OpenSubKey("Virtual Machine");
                //RegistryKey rkeyGuest = rkeyVirtualMachine.OpenSubKey("Guest");
                //RegistryKey rkeyParameters = rkeyGuest.OpenSubKey("Parameters");

                //String[] ValueNames = rkeyParameters.GetValueNames();
                //foreach (string name in ValueNames)
                //{
                //    //MessageBox.Show(name + ": " + rkeyVersions.GetValue(name).ToString());
                //    var temp = name + ": " + rkeyParameters.GetValue(name).ToString();
                //}

                //string remoteName = "IN-ISECBLD";
                string remoteName = "IN-SECW11";

                RegistryKey environmentKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, remoteName).OpenSubKey("SOFTWARE");
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }

            return RegistryKeyValue;
        }
    }
}
