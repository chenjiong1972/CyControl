using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.Core.Printer
{
    public class CustomPageEventArgs : EventArgs
    {
        internal CustomPageEventArgs(){
            Cancel = false;
            HasMorePages = false;
        }

        public bool HasMorePages { get; set; }
        
        public bool Cancel{get;set;}

        public PrintData pageContent { get; set; }
    }


    public class StartPageEventArgs : EventArgs 
    {
        private int pageNum;
        private int physicalPageNum;
        private bool cancel;
        private int currPrintDataIndex;

        internal StartPageEventArgs(int PageNum, int PhysicalPageNum, int CurrPrintDataIndex, bool Cancel)
        {
            pageNum = PageNum;
            physicalPageNum = PhysicalPageNum;
            cancel = Cancel;
            currPrintDataIndex = CurrPrintDataIndex;
        }

        public int PageNum
        { get { return pageNum; } }

        public int PhysicalPageNum
        { get { return physicalPageNum; } }

        public int CurrPrintDataIndex
        { get { return currPrintDataIndex; } }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }

    public class EndPageEventArgs : EventArgs
    {
        private int pageNum;
        private int physicalPageNum;
        private bool cancel;
        private bool endDoc;
        private bool endCurrPrintData;
        private bool newPhysicalPage;

        internal EndPageEventArgs(int PageNum, int PhysicalPageNum, bool Cancel, bool EndDoc, bool EndCurrPrintData, bool NewPhysicalPage)
        {
            pageNum = PageNum;
            physicalPageNum = PhysicalPageNum;
            cancel = Cancel;
            endDoc = EndDoc;
            endCurrPrintData = EndCurrPrintData;
            newPhysicalPage = NewPhysicalPage;
        }

        public int PageNum
        { get { return pageNum; } }

        public int PhysicalPageNum
        { get { return physicalPageNum; } }

        public bool EndDoc
        {
            get { return endDoc; }
            set { endDoc = value; }
        }
        public bool EndCurrPrintData
        {
            get { return endCurrPrintData; }
            set { endCurrPrintData = value; }
        }
        public bool NewPhysicalPage
        {
            get { return newPhysicalPage; }
            set { newPhysicalPage = value; }
        }
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
}
