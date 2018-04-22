using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.License
{
    class LicenseModals
    {
        public static string[] GetLicenseModals()
        {
            int i = 0;
            string r = "";
            string[] topModals = new string[2];
            r = UnvaryingSagacity.AccountOfBank.Environment.TOPMODALNAME_1;
            topModals[i] = r;
            i++;
            r = UnvaryingSagacity.AccountOfBank.Environment.TOPMODALNAME_2;
            topModals[i] = r;
            //i++;
            //r = UnvaryingSagacity.VoucherSuitSchemePrinter.Environment.TOPMODALNAME_3;
            return topModals;
        }
    }
}
