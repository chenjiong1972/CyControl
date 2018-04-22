using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.Core.Printer
{
    public interface IPrintDataFull
    {
        bool Init(out PrinterDefineRowsInPage p);

        void FullPrintData(int RowsInPage, PrintDatas printDatas);
    }
}
