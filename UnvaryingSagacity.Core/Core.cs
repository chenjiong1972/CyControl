using System;
using System.Threading;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.VisualBasic;

namespace UnvaryingSagacity.Core
{
	[Flags]
	public enum FlagsSetWindowPos : uint
	{
		SWP_NOSIZE          = 0x0001,
		SWP_NOMOVE          = 0x0002,
		SWP_NOZORDER        = 0x0004,
		SWP_NOREDRAW        = 0x0008,
		SWP_NOACTIVATE      = 0x0010,
		SWP_FRAMECHANGED    = 0x0020,
		SWP_SHOWWINDOW      = 0x0040,
		SWP_HIDEWINDOW      = 0x0080,
		SWP_NOCOPYBITS      = 0x0100,
		SWP_NOOWNERZORDER   = 0x0200, 
		SWP_NOSENDCHANGING  = 0x0400,
		SWP_DRAWFRAME       = 0x0020,
		SWP_NOREPOSITION    = 0x0200,
		SWP_DEFERERASE      = 0x2000,
		SWP_ASYNCWINDOWPOS  = 0x4000
	}

    public enum ShowWindowStyles : short
	{
		SW_HIDE             = 0,
		SW_SHOWNORMAL       = 1,
		SW_NORMAL           = 1,
		SW_SHOWMINIMIZED    = 2,
		SW_SHOWMAXIMIZED    = 3,
		SW_MAXIMIZE         = 3,
		SW_SHOWNOACTIVATE   = 4,
		SW_SHOW             = 5,
		SW_MINIMIZE         = 6,
		SW_SHOWMINNOACTIVE  = 7,
		SW_SHOWNA           = 8,
		SW_RESTORE          = 9,
		SW_SHOWDEFAULT      = 10,
		SW_FORCEMINIMIZE    = 11,
		SW_MAX              = 11
	}

    public enum WindowStyles : uint
	{
		WS_OVERLAPPED       = 0x00000000,
		WS_POPUP            = 0x80000000,
		WS_CHILD            = 0x40000000,
		WS_MINIMIZE         = 0x20000000,
		WS_VISIBLE          = 0x10000000,
		WS_DISABLED         = 0x08000000,
		WS_CLIPSIBLINGS     = 0x04000000,
		WS_CLIPCHILDREN     = 0x02000000,
		WS_MAXIMIZE         = 0x01000000,
		WS_CAPTION          = 0x00C00000,
		WS_BORDER           = 0x00800000,
		WS_DLGFRAME         = 0x00400000,
		WS_VSCROLL          = 0x00200000,
		WS_HSCROLL          = 0x00100000,
		WS_SYSMENU          = 0x00080000,
		WS_THICKFRAME       = 0x00040000,
		WS_GROUP            = 0x00020000,
		WS_TABSTOP          = 0x00010000,
		WS_MINIMIZEBOX      = 0x00020000,
		WS_MAXIMIZEBOX      = 0x00010000,
		WS_TILED            = 0x00000000,
		WS_ICONIC           = 0x20000000,
		WS_SIZEBOX          = 0x00040000,
		WS_POPUPWINDOW      = 0x80880000,
		WS_OVERLAPPEDWINDOW = 0x00CF0000,
		WS_TILEDWINDOW      = 0x00CF0000,
		WS_CHILDWINDOW      = 0x40000000
	}

    public enum WindowExStyles
	{
		WS_EX_DLGMODALFRAME     = 0x00000001,
		WS_EX_NOPARENTNOTIFY    = 0x00000004,
		WS_EX_TOPMOST           = 0x00000008,
		WS_EX_ACCEPTFILES       = 0x00000010,
		WS_EX_TRANSPARENT       = 0x00000020,
		WS_EX_MDICHILD          = 0x00000040,
		WS_EX_TOOLWINDOW        = 0x00000080,
		WS_EX_WINDOWEDGE        = 0x00000100,
		WS_EX_CLIENTEDGE        = 0x00000200,
		WS_EX_CONTEXTHELP       = 0x00000400,
		WS_EX_RIGHT             = 0x00001000,
		WS_EX_LEFT              = 0x00000000,
		WS_EX_RTLREADING        = 0x00002000,
		WS_EX_LTRREADING        = 0x00000000,
		WS_EX_LEFTSCROLLBAR     = 0x00004000,
		WS_EX_RIGHTSCROLLBAR    = 0x00000000,
		WS_EX_CONTROLPARENT     = 0x00010000,
		WS_EX_STATICEDGE        = 0x00020000,
		WS_EX_APPWINDOW         = 0x00040000,
		WS_EX_OVERLAPPEDWINDOW  = 0x00000300,
		WS_EX_PALETTEWINDOW     = 0x00000188,
		WS_EX_LAYERED			= 0x00080000
	}

    public enum Msgs
    {
        WA_ACTIVE = 1,
        WA_CLICKACTIVE = 2,
        WA_INACTIVE = 0,

        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_DELETEITEM = 0x002D,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSEWHEEL = 0x020A,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_DEVICECHANGE = 0x0219,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = 0x8000,
        WM_USER = 0x0400
    }

    public enum HitTest
	{
		HTERROR			= -2,
		HTTRANSPARENT   = -1,
		HTNOWHERE		= 0,
		HTCLIENT		= 1,
		HTCAPTION		= 2,
		HTSYSMENU		= 3,
		HTGROWBOX		= 4,
		HTSIZE			= 4,
		HTMENU			= 5,
		HTHSCROLL		= 6,
		HTVSCROLL		= 7,
		HTMINBUTTON		= 8,
		HTMAXBUTTON		= 9,
		HTLEFT			= 10,
		HTRIGHT			= 11,
		HTTOP			= 12,
		HTTOPLEFT		= 13,
		HTTOPRIGHT		= 14,
		HTBOTTOM		= 15,
		HTBOTTOMLEFT	= 16,
		HTBOTTOMRIGHT	= 17,
		HTBORDER		= 18,
		HTREDUCE		= 8,
		HTZOOM			= 9 ,
		HTSIZEFIRST		= 10,
		HTSIZELAST		= 17,
		HTOBJECT		= 19,
		HTCLOSE			= 20,
		HTHELP			= 21
	}

    public enum ScrollBars : uint
	{
		SB_HORZ = 0,
		SB_VERT = 1,
		SB_CTL = 2,
		SB_BOTH = 3
	}

    public enum GetWindowLongIndex : int
	{
		GWL_STYLE = -16,
		GWL_EXSTYLE = -20
	}

    // Hook Types  
    internal enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }


    /// <summary>
    /// 我的基类,定义了Name和Changed
    /// </summary>
    public class MyBaseClass
    {
        public MyBaseClass()
        {
        }
        public MyBaseClass(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public bool Changed { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public void CopyTo(MyBaseClass c)
        {
            c.Name = Name;
        }
    }

    /// <summary>
    /// ID为字符的简单类型
    /// </summary>
    public class SampleClass : MyBaseClass
    {
        public string ID { get; set; }

        public void CopyTo(SampleClass c)
        {
            base.CopyTo(c);
            c.ID = ID;
        }

        public override string ToString()
        {
            return ID + ", " + base.Name;
        }
    }

    /// <summary>
    /// ID为整数的简单类型
    /// </summary>
    public class SampleClass2 : MyBaseClass
    {
        public int ID { get; set; }
        public void CopyTo(SampleClass2 c)
        {
            base.CopyTo(c);
            c.ID = ID;
        }
    }

    /// <summary>
    /// ID为64位整数的简单类型
    /// </summary>
    public class SampleClass3 : MyBaseClass
    {
        public Int64 ID { get; set; }
        public void CopyTo(SampleClass3 c)
        {
            base.CopyTo(c);
            c.ID = ID;
        }
    }

    public class SampleClassTree<T> : SampleClass,IEnableFindInChildren <T>
    {
        private ListDictionary<T> _nodes = new ListDictionary<T>();
        
        public Int32 Level { get; set; }
        public string Parent { get; set; }

        /// <summary>
        /// 不复制下级的内容
        /// </summary>
        /// <param name="dst"></param>
        public void CopyTo(SampleClassTree<T> dst)
        {
            base.CopyTo(dst);
            dst.Level = Level;
            dst.Parent = Parent;

        }
        #region IEnableFindInChildren<T> 成员

        public ListDictionary<T> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// 内部起始索引为0,对外部为0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListDictionary<T>
    {
        const string KEYPREFIX = "~~~";
        System.Collections.Generic.List<string> listKey;// = new List<string>();
        System.Collections.Generic.Dictionary<string, T> listValue;// = new Dictionary<string, T>();

        public ListDictionary()
        {
            //Core.General.Check();
            if (!Core.General.Check())
                return;
            listKey = new List<string>();
            listValue = new Dictionary<string, T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < listKey.Count; i++)
            {
                yield return listValue[listKey[i]];
            }
        }

        public void Add(T item)
        {
            Add("", item, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">可以是整数字符串,Lenght不能小于1</param>
        /// <param name="item"></param>
        public void Add(string key, T item)
        {
            Add(key, item, -1);
        }
        public void Add(T item, int before)
        {
            Add("", item, before);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">可以是整数字符串,Lenght不能小于1</param>
        /// <param name="item"></param>
        /// <param name="before"></param>
        public void Add(string key, T item, int before)
        {
            string internalKey;
            if (key.Length <= 0)
                internalKey = GetInternalKey();
            else
                internalKey = GetInternalKey(key);
            if (before > 0)
                listKey.Insert(before, internalKey);
            else
                listKey.Add(internalKey);
            listValue.Add(internalKey, item);
        }

        public void Clear()
        {
            listKey.Clear();
            listValue.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">从0开始</param>
        public virtual void Remove(int index)
        {
            if (index >= 0 && index <= listKey.Count)
            {
                string key = listKey[index];
                if (listValue.ContainsKey(key))
                    listValue.Remove(key);
                listKey.RemoveAt(index);
            }
        }

        public void Remove(string key)
        {
            if (listKey.Contains(GetInternalKey(key)))
                listKey.Remove(GetInternalKey(key));
            if (listValue.ContainsKey(GetInternalKey(key)))
                listValue.Remove(GetInternalKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public virtual bool SetItem(int index, T newItem)
        {
            string key = listKey[index];
            return SetItem(key, newItem);
        }

        public bool SetItem(string key, T newItem)
        {
            if (listKey.Contains(GetInternalKey(key)))
            {
                listValue[GetInternalKey(key)] = newItem;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <returns></returns>
        public virtual T GetItem(int index)
        {
            return this[index];
        }

        public T GetItem(string key)
        {
            return this[key];
        }

        //已经存在 T Item 方法了
        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < listKey.Count)
                {
                    string key = listKey[index];
                    if (listValue.ContainsKey(key))
                        return listValue[key];
                    else
                        return default(T);
                }
                else
                    return default(T);
            }
            set
            {
                if (index >= 0 && index < listKey.Count)
                {
                    string key = listKey[index];
                    if (listValue.ContainsKey(key))
                        listValue[key] = value;

                }
            }
        }

        public T this[string key]
        {
            get
            {
                string innerKey = GetInternalKey(key);
                if (listValue.ContainsKey(innerKey))
                    return listValue[innerKey];
                else
                    return default(T);
            }
            set
            {
                string innerKey = GetInternalKey(key);
                if (listValue.ContainsKey(innerKey))
                    listValue[innerKey] = value;
            }
        }

        public T LastItem
        {
            get
            {
                int index = listKey.Count - 1;
                if (index >= 0 && index < listKey.Count)
                {
                    string key = listKey[index];
                    if (listValue.ContainsKey(key))
                        return listValue[key];
                    else
                        return default(T);
                }
                else
                    return default(T);
            }
            set
            {
                int index = listKey.Count - 1;
                if (index >= 0 && index < listKey.Count)
                {
                    string key = listKey[index];
                    if (listValue.ContainsKey(key))
                        listValue[key] = value;

                }
            }
        }

        public int Count { get { return listKey.Count; } }

        public virtual int Index(string key)
        {
            return listKey.IndexOf(GetInternalKey(key));
        }

        public virtual string Key(int index)
        {
            if (index >= 0 && index < listKey.Count)
            {
                return listKey[index - 1].Substring(KEYPREFIX.Length);
            }
            else
                return "";
        }

        public bool ContainsKey(string key)
        {
            return listValue.ContainsKey(GetInternalKey(key));
        }

        /// <summary>
        /// 根据前缀获得一个唯一的名称或Key
        /// </summary>
        /// <param name="preFix">前缀</param>
        /// <returns></returns>
        public string GetUniqueName(string preFix)
        {
            int i = 1;
            string s = preFix;

            while (ContainsKey(s))
            {
                s = preFix + i;
                i++;
            }
            return s;
        }

        private string GetInternalKey()
        {
            return Guid.NewGuid().ToString();
        }

        private string GetInternalKey(string outKey)
        {
            return KEYPREFIX + outKey;
        }

        /// <summary>
        /// 查找一个对象,并返回. 前提是T is SampleClass,且下级的ID字符中必须包含上级的ID
        /// 并实现接口 IEnableFindInChildren
        /// </summary>
        /// <param name="id">要找的id</param>
        /// <param name="inAllChildren">是否在所有的下级中搜索</param>
        /// <returns></returns>
        public T Find(string ID, bool inAllChildren)
        {
            if (ContainsKey(ID))
                return this[ID];
            else
            {
                if (inAllChildren)
                {
                    foreach (T obj in this)
                    {
                        if (obj is SampleClass)
                        {
                            SampleClass o = obj as SampleClass;
                            if (ID.IndexOf(o.ID) == 0)
                            {
                                T t = Find(ID, obj);
                                if (t != null)
                                    return t;
                            }
                        }
                        else
                        {
                            MessageBox.Show("对象[" + obj.GetType().FullName + "]没有从[SampleClass]继承");
                            break;
                        }
                    }
                }
                return default(T);
            }
        }

        private T Find(string ID, T obj)
        {
            if (obj is SampleClass)
            {
                if ((obj as SampleClass).ID == ID)
                    return obj;
            }
            if ((obj is IEnableFindInChildren<T>))
            {
                if ((obj as IEnableFindInChildren<T>).Nodes.ContainsKey(ID))
                {
                    return (obj as IEnableFindInChildren<T>).Nodes[ID];
                }
                else
                {
                    foreach (T child in (obj as IEnableFindInChildren<T>).Nodes)
                    {
                        if (ID.IndexOf((child as SampleClass).ID) == 0)
                        {
                            T t = Find(ID, child);
                            if (t != null)
                                return t;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("对象[" + obj.GetType().FullName + "]没有实现接口:IEnableFindInChildren");
            }
            return default(T);
        }

        /// <summary>获得所有下级定义的ID数组,;
        /// 前提是T is SampleClassTree
        /// </summary>
        /// <param name="searchAllChildren"></param>
        /// <param name="owner">集合所属对象的ID,用于提供给直接下级</param>
        /// <returns></returns>
        public TreeNodeToArrary<string>[] Lister(bool searchAllChildren, string owner)
        {
            TreeNodeToArrary<string>[] list = new TreeNodeToArrary<string>[0];
            foreach (object o in this)
            {
                Array.Resize<Core.TreeNodeToArrary<string>>(ref list, list.Length + 1);
                list[list.Length - 1] = new TreeNodeToArrary<string>();
                list[list.Length - 1].ID = (o as SampleClassTree<T>).ID;
                list[list.Length - 1].Parent = owner;
                if (searchAllChildren && (o as SampleClassTree<T>).Nodes.Count > 0)
                {
                    Lister(ref list, (o as SampleClassTree<T>), searchAllChildren);
                }
            }
            return list;
        }

        private void Lister(ref Core.TreeNodeToArrary<string>[] list, object obj, bool searchAllChildren)
        {
            foreach (object o in (obj as SampleClassTree<T>).Nodes)
            {
                Array.Resize<Core.TreeNodeToArrary<string>>(ref list, list.Length + 1);
                list[list.Length - 1] = new TreeNodeToArrary<string>();
                list[list.Length - 1].ID = (o as SampleClassTree<T>).ID;
                list[list.Length - 1].Parent = (obj as SampleClassTree<T> ).ID;
                if (searchAllChildren && (o as SampleClassTree<T>).Nodes.Count > 0)
                {
                    Lister(ref list, (o as SampleClassTree<T>), searchAllChildren);
                }
            }
        }

    }

    public interface IEnableFindInChildren<T>
    {
        Core.ListDictionary<T> Nodes { get; set; }
    }



    /// <summary>
    /// 使用索引器。内部起始索引为0,对外部为0
    /// </summary>
    /// <typeparam name="T"></typeparam>

    // class to wrap up Windows 32 API constants and functions.
    public class Win32API
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct OSVersionInfo
        {
            public int OSVersionInfoSize;
            public int majorVersion;
            public int minorVersion;
            public int buildNumber;
            public int platformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string versionstring;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }
        public const int CB_SELECTSTRING = 0x14D;
        public const int CB_ERR = (-1);
        public const int GWL_EXSTYLE = (-20);
        public const int GW_OWNER = 4;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_APPWINDOW = 0x40000;
        public const int CS_DROPSHADOW = 0x20000;
        public const int GCL_STYLE = (-26);

        [DllImport("kernel32.dll", EntryPoint = "CreateDirectoryA")]
        public static extern bool CreateDirectory(string lpPathName, SECURITY_ATTRIBUTES lpSecurityAttribut);

        public delegate bool EnumWindowsCallback(int hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        public static extern int EnumWindows(EnumWindowsCallback callback, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true,
        CharSet = CharSet.Ansi, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        public static extern int EnumWindowsDllImport(EnumWindowsCallback callback, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern int FindWindowAny(int lpClassName, int lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern int FindWindowNullClassName(int lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern int FindWindowNullWindowCaption(string lpClassName, int lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetClassNameA")]
        public static extern int GetClassName(int hwnd, StringBuilder lpClassName, int cch);

        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceA")]
        public static extern int GetDiskFreeSpace(string lpRootPathName,
                                                    ref int lpSectorsPerCluster,
                                                    ref int lpBytesPerSector,
                                                    ref int lpNumberOfFreeClusters,
                                                    ref int lpTotalNumberOfClusters);

        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        public static extern int GetDiskFreeSpaceEx(string lpRootPathName,
                                                        ref int lpFreeBytesAvailableToCaller,
                                                        ref int lpTotalNumberOfBytes,
                                                        ref UInt32 lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll", EntryPoint = "GetDriveTypeA")]
        public static extern int GetDriveType(string nDrive);

        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern int GetParent(int hwnd);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("Kernel32.dll", EntryPoint = "GetVersionExA", CharSet = CharSet.Ansi)]
        public static extern bool GetVersionEx(ref OSVersionInfo osvi);

        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern int GetWindow(int hwnd, int wCmd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA")]
        public static extern int GetWindowLong(int hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextA")]
        public static extern void GetWindowText(int hWnd, StringBuilder lpstring, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "IsIconic")]
        public static extern bool IsIconic(int hwnd);

        [DllImport("Powrprof.dll", EntryPoint = "IsPwrHibernateAllowed")]
        public static extern int IsPwrHibernateAllowed();

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        public static extern bool IsWindowVisible(int hwnd);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern int SetForegroundWindow(int hwnd);

        [DllImport("Powrprof.dll", EntryPoint = "SetSuspendState")]
        public static extern int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SwapMouseButton")]
        public static extern int SwapMouseButton(int bSwap);

        [DllImport("gdi32.dll", EntryPoint = "CreateEllipticRgn")]
        public static extern int CreateEllipticRgn(int X1, int Y1, int X2, int Y2);
        [DllImport("user32.dll", EntryPoint = "SetWindowRgn")]
        public static extern int SetWindowRgn(int hWnd, int hRgn, int bRedraw);
        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgn")]
        public static extern int CreateRectRgn(int X1, int Y1, int X2, int Y2);
        [DllImport("gdi32.dll", EntryPoint = "CombineRgn")]
        public static extern int CombineRgn(int hDest, int hSrc1, int hsrc2, int fHow);
        [DllImport("gdi32.dll", EntryPoint = "GetPixel")]
        public static extern int GetPixel(int hdc,int X1, int Y1);
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(int hWnd);
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(int hWnd, int hDC);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, char[] lParam);

        /// <summary>
        /// 窗体显示阴影:在窗体的构造函数中:SetClassLong(this.Handle, Core.Win32API.GCL_STYLE, Core.Win32API.GetClassLong(this.Handle, Core.Win32API.GCL_STYLE) | Core.Win32API.CS_DROPSHADOW);
        /// 如果系统没有启用特效则不起作用,反之系统本身就有阴影.对有标题框的窗口用处不大，对无标题框的窗口有用处.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
    }

    public class ImageHandler
    {
                
        /// <summary>
        /// 转换图像颜色,使其变暗.
        /// </summary>
        public static void DrawImageDark(Graphics g,Image image,Rectangle dstRect)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = image.Width;
            int height = image.Height;

            float[][] colorMatrixElements = { 
   new float[] {0.75F,  0,  0,  0, 0},
   new float[] {0,  0.75F,  0,  0, 0},
   new float[] {0,  0,  0.75F,  0, 0},
   new float[] {0,  0,  0,  0.75F, 0},
   new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            g.DrawImage(
               image,
               dstRect,  // destination rectangle 
               0, 0,        // upper-left corner of source rectangle 
               width,       // width of source rectangle
               height,      // height of source rectangle
               GraphicsUnit.Pixel,
               imageAttributes);
        }

        public static void DrawImageDarkDark(Graphics g, Image image, Rectangle dstRect)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = image.Width;
            int height = image.Height;

            float[][] colorMatrixElements = { 
   new float[] {0.50F,  0,  0,  0, 0},
   new float[] {0,  0.50F,  0,  0, 0},
   new float[] {0,  0,  0.50F,  0, 0},
   new float[] {0,  0,  0,  0.50F, 0},
   new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            g.DrawImage(
               image,
               dstRect,  // destination rectangle 
               0, 0,        // upper-left corner of source rectangle 
               width,       // width of source rectangle
               height,      // height of source rectangle
               GraphicsUnit.Pixel,
               imageAttributes);
        }
        /// <summary>
        /// 得到一个圆角矩形的路径
        /// </summary>
        /// <param name="shape">返回圆角矩形的封闭路径</param>
        /// <param name="start_x">左顶点的X</param>
        /// <param name="start_y">左顶点的Y</param>
        /// <param name="_width">矩形宽</param>
        /// <param name="_height">矩形高</param>
        /// <param name="w">圆角弧线所在的矩形的宽</param>
        /// <param name="h">圆角弧线所在的矩形的高</param>
        /// <returns></returns>
        public static void ArcRectanglePath(GraphicsPath shape,int start_x, int start_y, int _width, int _height, int w, int h)
        {
            //int _width, _height;
            //int w, h;
            //w = h = 30;//圆角的外接矩形的宽高
            ////x = y =  20;
            //_width = 300;
            //_height = 100;

            //int start_x = 30;
            //int start_y = 30;
            shape.AddLine(start_x + w / 2, start_y, start_x + _width - (w / 2 + 1), start_y);
            shape.AddArc(start_x + _width - (w + 1), start_y, w, h, 270, 90);//切角窗口则注释本行

            shape.AddLine(start_x + _width - 1, start_y + (h / 2), start_x + _width - 1, start_y + _height - (h / 2));
            shape.AddArc(start_x + _width - (w + 1), start_y + _height - (h + 1), w, h, 0, 90);//切角窗口则注释本行

            shape.AddLine(start_x + _width - (w / 2 + 1), start_y + _height - 1, start_x + w / 2, start_y + _height - 1);
            shape.AddArc(start_x, start_y + _height - (h + 1), w, h, 90, 90);//切角窗口则注释本行

            shape.AddLine(start_x, start_y + _height - (h / 2), start_x, start_y + (h / 2));
            shape.AddArc(start_x, start_y, w, h, 180, 90);//切角窗口则注释本行
            shape.CloseFigure(); 
        }

        /// <summary>
        /// 以jpeg格式存
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] FromImage(Image image)
        {
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();
            try
            {
                image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);

                return mstream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] FromImage(Image image,System.Drawing.Imaging.ImageFormat imageformt)
        {
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();
            try
            {
                image.Save(mstream, imageformt);

                return mstream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static Image FromByteArray(byte[] byteArray)
        {
            System.Drawing.Image image = null;
            try
            {
                System.IO.MemoryStream mStream = new System.IO.MemoryStream(byteArray);
                image = System.Drawing.Image.FromStream(mStream,true ,true );
                return image;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换到指定的分辨率
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns></returns>
        public static Image ImageConvert(Image SourceImage, float dpiX, float dpiY)
        {
            Size size = new Size((int)((SourceImage.Width * dpiX) / SourceImage.HorizontalResolution), (int)((SourceImage.Height * dpiY) / SourceImage.VerticalResolution));
            Bitmap bm = new Bitmap(SourceImage, size);
            return (Image)bm.Clone();
        }

        public static void ImageSave(Image SourceImage, string filename)
        {
            ImageSave(filename, SourceImage, SourceImage.RawFormat);
        }

        public static void ImageSave(string filename, Image SourceImage, System.Drawing.Imaging.ImageFormat format)
        {
            Bitmap bm = new Bitmap(SourceImage);
            bm.Save(filename, format);
        }

        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="angle"></param>
        /// <param name="backGround"></param>
        /// <returns></returns>
        public static Image ImageRotate(Image SourceImage, float angle,Color backGround)
        {
            return ImageRotate(SourceImage, angle, new Point(0, 0),backGround);
        }

        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="angle"></param>
        /// <param name="rotateCenter"></param>
        /// <param name="backGround"></param>
        /// <returns></returns>
        public static Image ImageRotate(Image SourceImage, float angle, PointF rotateCenter, Color backGround)
        {
            Bitmap bm = new Bitmap(SourceImage.Width, SourceImage.Height);
            Graphics g = Graphics.FromImage(bm);
            g.Clear(backGround);
            System.Drawing.Drawing2D.Matrix x = new System.Drawing.Drawing2D.Matrix();
            x.RotateAt(angle, rotateCenter);
            g.Transform = x;
            g.DrawImage(SourceImage, 0, 0);
            return  (Image)bm.Clone ();
        }

        /// <summary>
        /// 获得分辨率
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="dipX"></param>
        /// <param name="dipY"></param>
        public static void ScreenDpi(IntPtr hwnd, ref float dipX,ref float dipY)
        {
            Graphics g = Graphics.FromHwnd(hwnd);
            dipX = g.DpiX;
            dipY = g.DpiY;
            g.Dispose();
        }

        /// <summary>
        /// 如果文件无效则返回NULL
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Image FromFile(string file)
        {
            try
            {
                Image img = Image.FromFile(file);
                return img;
            }
            catch
            {
                return default(Image);
            }
        }

        /// <summary>
        /// 按指定长宽比缩放到指定大小,
        /// 水平方向裁剪两边保留中间;垂直方向裁剪底部
        /// </summary>
        /// <param name="src">源图像</param>
        /// <param name="rate">目标长宽比</param>
        /// <param name="width">目标宽</param>
        /// <param name="height">目标高</param>
        /// <returns></returns>
        public static Bitmap StretchImage(Image src, float rate, int width, int height)
        {

            Bitmap pb;
            /// 根据长宽比计算合适的宽高,保证缩放时不会变形
            int h = (int)(src.Width / rate);
            int w = (int)(src.Height * rate);
            if (src.Height > h)
            {
                pb = new Bitmap(src.Width, h);
            }
            else if (src.Width > w)
            {
                pb = new Bitmap(w, src.Height);
            }
            else
                pb = new Bitmap(src.Width, src.Height);

            Graphics g1 = Graphics.FromImage(pb);
            /// 根据计算好的宽高做裁剪,保证缩放时不会变形
            g1.DrawImage(src, new Rectangle(new Point(0, 0), pb.Size), new Rectangle((src.Width - pb.Width) / 2, 0, pb.Width - 1, pb.Height - 1), GraphicsUnit.Pixel);
            //pb.Save(@"c:\"+n, ImageFormat.Jpeg);
            Bitmap bp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bp);
            /// 缩放
            g.DrawImage(pb, new Rectangle(0, 0, width, height), new Rectangle(0, 0, pb.Width - 1, pb.Height - 1), GraphicsUnit.Pixel);
            //bp.Save(@"c:\2.jpg", ImageFormat.Jpeg);
            g.Dispose();
            g1.Dispose();
            return bp;
        }
    }

    public class PropertyItem
    {
        private string _name = "";
        private object _value = null;

        public PropertyItem() { }

        public PropertyItem(string name) { _name = name; }

        public PropertyItem(string name, object value) { _name = name; _value = value; }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public class Spell
    {
        XmlExplorer _xml=new XmlExplorer ();        

        public Spell() 
        {
            string fname=Application.StartupPath + @"\GB2312Spell.xml";
            if (File.Exists(fname))
            {
                _xml.OpenFile (fname);
            }
        }

        /// <summary> 
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串 
        /// </summary> 
        /// <param name="CnStr">汉字字符串</param> 
        /// <returns>相对应的汉语拼音首字母串</returns> 
        public string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
            }

            return strTemp;
        }

        /// <summary> 
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary> 
        /// <param name="CnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private string GetCharSpellCode(string CnChar)
        {
            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char 
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }


            #region 拼音码表
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //没有U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53688 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 
            #endregion

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }

            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53688))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else 
            {
                if (_xml.FileName.Length > 0)
                    return _xml.ReadValue("Spell", "Code" + iCnChar.ToString(), "?");
                else 
                    return ("?");
            }
            
        }
    }

    public class General
    {
        //private string CompanyName = "";
        ///给Web使用的许可检查
        //public static bool GetGeneral(string n,string f)
        //{
        //    General g = new General();
        //    g.Check2();
        //}

        //这里面不能有消息弹出,应开始一个消息弹出线程
//        internal bool Check2()
//        {
//#if (!DEBUG)
//            {
//                if (Application.CompanyName != "深圳远睿恒软件有限公司" && Application.CompanyName != "UnvaryingSagacity")
//                {
//                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "本程序属于深圳远睿恒软件有限公司所有, 请不要非法使用.", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//                    //t.Start();
//                    //MessageBox.Show("本程序属于深圳远睿恒软件有限公司所有, 请不要非法使用.", "深圳远睿恒软件有限公司");
//                    return false;
//                }
//                string fileName = Application.StartupPath + @"\" + Application.ProductName + ".lic";
//                if (!File.Exists(fileName))
//                {
//                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件不存在， 软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//                    //t.Start();
//                    return false;
//                }
//                License.LicenseContent lic = new UnvaryingSagacity.License.LicenseContent();
//                if (!lic.Valid)
//                {
//                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件已经损坏，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//                    //t.Start();
//                    return false;
//                }
//                if (lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//软件产品//制造商") != Application.CompanyName)
//                {
//                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件内容不正确，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//                    //t.Start();
//                    return false;
//                }
//                if (lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//软件产品//名称") != Application.ProductName)
//                {
//                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件内容与事实不符，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
//                    //t.Start();
//                    return false;
//                }
//                // 不再检查UnvaryingSagacity.Core.dll签名,因为会经常更新
//                //string publicKey = lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//加密区//公钥");
//                //string privateKey = GetPrivateKey(Application.CompanyName+"出品:" + publicKey);
//                //byte[] pKey = Encoding.UTF8.GetBytes(privateKey);
//                //byte[] dst = ShaEnCoder.ShaFile(Application.StartupPath + @"\UnvaryingSagacity.Core.dll", pKey);
//                //string s1 = ShaEnCoder.HashToString(dst);
//                //string s2 = lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//签名区//Core");
//                return true;
//            }
//#else
//            return (Application.CompanyName == "深圳远睿恒软件有限公司" || Application.CompanyName == "UnvaryingSagacity");
//#endif
//        }

        /// <summary>
        /// 转换数字星期为大写数字,如0=星期日
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string GetUCaseDayOfWeek(DayOfWeek w)
        {
            int i=(int)w;
            switch (i)
            {
                case 0:
                    return "日";
                default:
                    return UCaseNumeric(i.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="preFix">前缀,如"星期"</param>
        /// <returns></returns>
        public static string GetUCaseDayOfWeek(DayOfWeek w,string preFix)
        {
            return preFix + GetUCaseDayOfWeek(w);
        }

        /// <summary>
        /// 根据开始编号和数量,获得截止编号
        /// </summary>
        /// <param name="start"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetEndNumberByAmount(string start,int amount)
        {
            if (amount <= 1)
            {
                return start;
            }
            else{
                int n = 0;
                if (!int.TryParse(start, out n))
                {
                    //非全部数字
                    for (int i = start.Length - 1; i >= 0; i--)
                    {
                        if (!Char.IsDigit(start, i))
                        {
                            n = int.Parse(start.Substring(i+1));
                            break;
                        }
                    }
                }
                n += (amount - 1);
                string s = n.ToString();
                if (s.Length >= start.Length)
                {
                    return s;
                }
                else
                {
                    n = start.Length - s.Length;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(start.Substring(0, n));
                    sb.Append(s);
                    return sb.ToString();
                }
            }
        }
        
        /// <summary> 
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串 
        /// </summary> 
        /// <param name="CnStr">汉字字符串</param> 
        /// <returns>相对应的汉语拼音首字母串</returns> 
        public static string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
            }

            return strTemp;
        }

        /// <summary> 
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary> 
        /// <param name="CnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char 
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }


            #region 拼音码表
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //没有U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53688 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 
            #endregion

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }

            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53688))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else if ((iCnChar == 61091))//睿
            {
                return "R";
            }
            else return ("?");
        }

        public static bool IsNumberic(string s)
        {
            return IsNumberic(s, true,true );
        }

        public static bool IsNumberic(string s, bool allowDot)
        {
            return IsNumberic(s, allowDot, true);
        }
        public static bool IsNumberic(string s, bool allowDot, bool allowSubtract)
        {
            if (s == null)
                return false;
            if (s.Length <= 0)
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (!CharIsNumberic(s.Substring(i, 1)))
                    return false;
            }
            if (allowDot)
            {
                if (s.IndexOf('.') != (s.LastIndexOf('.')))
                    return false;
            }
            else
            {
                if (s.IndexOf('.') >= 0)
                    return false;
            }
            int subtractIndex=s.LastIndexOf('-');
            if (subtractIndex == 0)
            {
                if (!allowSubtract)
                    return false;
                else
                {
                    if (s.Length == 1)
                        return false;
                }

            }
            else if (subtractIndex > 0)
                return false;
            return true;
        }

        public static bool CharIsNumberic(string s)
        {
            //数字:48~57
            //U word:65~90
            //l word:97~122
            char[] cs = s.ToCharArray();
            if (cs.Length > 1)
                return false;
            int c = (int)(cs[0]);
            if (c >= 48 && c <= 57)
                return true;
            else if (c == (int)('-') || c == (int)('.'))
                return true;
            return false;
        }

        public static bool CharIsNumberic(string s, bool allowDot, bool allowSubtract)
        {
            //数字:48~57
            //U word:65~90
            //l word:97~122
            char[] cs = s.ToCharArray();
            if (cs.Length > 1)
                return false;
            int c = (int)(cs[0]);
            if (c >= 48 && c <= 57)
                return true;
            else if (allowDot && c == (int)('.'))
                return true;
            else if (c == (int)('-') && allowSubtract)
                return true;
            return false;
        }

        public static int SelectStringInComboBox(ComboBox oBox, string FindWhat)
        {
            int i;
            i = Win32API.SendMessage(oBox.Handle.ToInt32(), Win32API.CB_SELECTSTRING, -1, FindWhat.ToCharArray());
            return i;

        }

        public static string GetTextInComboBox(ComboBox oBox, int itemData)
        {
            System.Collections.IEnumerator o = oBox.Items.GetEnumerator();
            while (o.MoveNext())
            {
                try
                {
                    if (((UnvaryingSagacity.Core.Printer.CboItem)o.Current).ItemData == itemData)
                        return o.Current.ToString();
                }
                catch
                {
                    return o.Current.ToString();
                }
            }
            return "";
        }

        public static string GetTextInComboBox(ComboBox oBox, Int16 itemIndex)
        {
            Int16 i = -1;
            System.Collections.IEnumerator o = oBox.Items.GetEnumerator();
            while (o.MoveNext())
            {
                i++;
                if (i == itemIndex)
                    return o.Current.ToString();
            }
            return "";
        }

        public static int GetIndexInComboBox(ComboBox oBox, int itemData)
        {
            int i = -1;
            System.Collections.IEnumerator o = oBox.Items.GetEnumerator();
            while (o.MoveNext())
            {
                i++;
                try
                {
                    if (((UnvaryingSagacity.Core.Printer.CboItem)o.Current).ItemData == itemData)
                        return i;
                }
                catch
                {
                    return -1;
                }
            }
            return -1;
        }

        public static string ChineseNumeric(string t)
        {
            const string CHINESENUM = "零壹贰叁肆伍陆柒捌玖";
            StringBuilder sb = new StringBuilder();
            foreach (char c in t)
            {
                switch (c)
                {
                    case '1':
                        sb.Append (CHINESENUM.Substring (1,1));
                        break;
                    case '2':
                        sb.Append (CHINESENUM.Substring (2,1));
                        break;
                    case '3':
                        sb.Append (CHINESENUM.Substring (3,1));
                        break;
                    case '4':
                        sb.Append (CHINESENUM.Substring (4,1));
                        break;
                    case '5':
                        sb.Append (CHINESENUM.Substring (5,1));
                        break;
                    case '6':
                        sb.Append (CHINESENUM.Substring (6,1));
                        break;
                    case '7':
                        sb.Append (CHINESENUM.Substring (7,1));
                        break;
                    case '8':
                        sb.Append (CHINESENUM.Substring (8,1));
                        break;
                    case '9':
                        sb.Append (CHINESENUM.Substring (9,1));
                        break;
                    case '0':
                        sb.Append (CHINESENUM.Substring (0,1));
                        break;
                    default :
                        sb.Append (c);
                        break;
                }
            }
            return sb.ToString ();

        }

        public static string UCaseNumeric(string t)
        {
            const string UCaseNUM = "零一二三四五六七八九";
            StringBuilder sb = new StringBuilder();
            foreach (char c in t)
            {
                switch (c)
                {
                    case '1':
                        sb.Append (UCaseNUM.Substring (1,1));
                        break;
                    case '2':
                        sb.Append (UCaseNUM.Substring (2,1));
                        break;
                    case '3':
                        sb.Append (UCaseNUM.Substring (3,1));
                        break;
                    case '4':
                        sb.Append (UCaseNUM.Substring (4,1));
                        break;
                    case '5':
                        sb.Append (UCaseNUM.Substring (5,1));
                        break;
                    case '6':
                        sb.Append (UCaseNUM.Substring (6,1));
                        break;
                    case '7':
                        sb.Append (UCaseNUM.Substring (7,1));
                        break;
                    case '8':
                        sb.Append (UCaseNUM.Substring (8,1));
                        break;
                    case '9':
                        sb.Append (UCaseNUM.Substring (9,1));
                        break;
                    case '0':
                        sb.Append (UCaseNUM.Substring (0,1));
                        break;
                    default :
                        sb.Append (c);
                        break;
                }
            }
            return sb.ToString ();
        }

        public static string ChineseAmount(string t)
        {
            const string CHINESENUM = "零壹贰叁肆伍陆柒捌玖";
            const string CHINESECUNIT = "分角元拾佰仟万拾佰仟亿";
            bool nozero = false;
            int i, j, k, l;
            string zhen = "", t0 = "", t1 = "", t2;
            i = t.IndexOf('.') + 1;
            if (i == 0 || i == t.Length)
                t = t + ".00";
            else if (i == t.Length - 1)
                t = t + "0";
            else if (i < t.Length - 2)
                t = t.Substring(1 - 1, i + 2);
            k = 1;

            for (i = t.Length - 1; i >= 0; i--)
            {
                t2 = t.Substring(i, 1);
                if (t2 == "0" && (!nozero))
                {
                    if (zhen == "") zhen = "整";
                    if (k == 3 || k == 7)
                        t1 = CHINESECUNIT.Substring(k - 1, 1) + t1;
                    else if (k == 11)
                    {
                        if (t1.Substring(1, 1) == "万")
                            t1 = CHINESECUNIT.Substring(k - 1, 1) + t1.Substring(2 - 1);
                        else
                            t1 = CHINESECUNIT.Substring(k - 1, 1) + t1;
                    }
                    k = k + 1;
                    if (k > 11) k = k - 8;
                }
                else if (t2 == "." && (!nozero))
                { }
                else
                {
                    nozero = true;
                    try
                    {
                        l = int.Parse(t2);
                    }
                    catch
                    {
                        continue;
                    }
                    if (l == 0)
                    {
                        if (t2 != t0)
                        {
                            t0 = t2;
                            j = int.Parse(t2) + 1;
                            if (k == 3 || k == 7 || k == 11) //5, 13, 21
                                t1 = CHINESECUNIT.Substring(k - 1, 1) + t1;
                            else
                                t1 = CHINESENUM.Substring(j - 1, 1) + t1;

                        }
                        else
                        {
                            if (k == 3 || k == 7)//5, 13
                                t1 = CHINESECUNIT.Substring(k - 1, 1) + t1;
                            else if (k == 11)//21
                            {
                                if (t1.Substring(1, 1) == "万")
                                    t1 = CHINESECUNIT.Substring(k - 1, 1) + t1.Substring(2 - 1);
                                else
                                    t1 = CHINESECUNIT.Substring(k - 1, 1) + t1;
                            }
                        }
                    }
                    else if (l >= 1 && l <= 9)
                    {
                        t0 = t2;
                        j = int.Parse(t2) + 1;
                        if (k == 11 && t1.Substring(1 - 1, 1) == "万") t1 = t1.Substring(2 - 1);
                        t1 = CHINESENUM.Substring(j - 1, 1) + CHINESECUNIT.Substring(k - 1, 1) + t1;
                    }
                    k = k + 1;
                    if (k > 11) k = k - 8;
                }
            }

            t2 = t1.Substring(1 - 1, 1);
            while (CHINESECUNIT.IndexOf(t2) >= 0 || t2 == "零")
            {
                t1 = t1.Substring(2 - 1);
                if (t1.Length > 1)
                    t2 = t1.Substring(1 - 1, 1);
                else
                    break;

            }
            if (t1 == "") t1 = "零元";
            return t1 + zhen;
        }

        public static void StringRemove(ref string s, char FindChar)
        {
            int i = -1;
            StringBuilder sb = new StringBuilder(s);
            while (true)
            {
                i = sb.ToString().IndexOf(FindChar);
                if (i < 0)
                    break;
                else
                    sb.Remove(i, 1);
            }
            s = sb.ToString();
        }

        public static void StringRemove(ref string s, string FindString)
        {
            int i = -1;
            StringBuilder sb = new StringBuilder(s);
            while (true)
            {
                i = sb.ToString().IndexOf(FindString);
                if (i < 0)
                    break;
                else
                    sb.Remove(i, 1);
            }
            s = sb.ToString();
        }

        public static Font FontFromString(string s)
        {
            if (s.Length > 0)
            {
                string[] arySplit = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                Font oFont = new Font(arySplit[0], int.Parse(arySplit[1]), (FontStyle)(Int16.Parse(arySplit[2])));
                return oFont;
            }
            else
                return default(Font);
        }

        public static String FontToString(Font oFont)
        {
            StringBuilder s = new StringBuilder();
            string SplitChar = ",";

            if (oFont != null)
            {
                s.Append(oFont.Name);
                s.Append(SplitChar);
                s.Append(oFont.Size.ToString());
                s.Append(SplitChar);
                s.Append((Int16)(oFont.Style));
            }
            return s.ToString();
        }

        /// <summary>
        /// 从内部日期转到标准日期
        /// </summary>
        /// <param name="s">format:yyyymmdd或yyyyMMddhhmmss</param>
        /// <returns></returns>
        public static DateTime FromString(string s)
        {
            try
            {
                int y = int.Parse(s.Substring(0, 4));
                int M = int.Parse(s.Substring(4, 2));
                int d = int.Parse(s.Substring(6, 2));
                int h = s.Length >= 10 ? int.Parse(s.Substring(8, 2)) : 0;
                int m = s.Length >= 12 ? int.Parse(s.Substring(10, 2)) : 0;
                int ss = s.Length >= 14 ? int.Parse(s.Substring(12, 2)) : 0;
                DateTime date = new DateTime(y, M, d, h, m, ss);
                return date;
            }
            catch
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 从标准日期转到内部日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns>format:yyyymmdd</returns>
        public static string FromDateTime(DateTime date)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(date.Year);
            sb.Append(date.Month.ToString("00"));
            sb.Append(date.Day.ToString("00"));
            return sb.ToString();
        }
        /// <summary>
        /// 从10位长度日期转到内部日期
        /// </summary>
        /// <param name="s"></param>
        /// <param name="splitChar">可以为0长度</param>
        /// <returns>format:yyyymmdd or yyyy-MM-dd</returns>
        public static string ToShortDateString(string s,string splitChar)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(s.Substring(0, 4));
                sb.Append(splitChar);
                sb.Append(s.Substring(5, 2));
                sb.Append(splitChar);
                sb.Append(s.Substring(8, 2));
                return sb.ToString ();
            }
            catch
            {
                return default(string);
            }
        }

        /// <summary>
        /// HexTo(Encoding.Default.GetBytes(_Radix16), _Radix16.length)
        /// </summary>
        /// <param name="_Radix16"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int HexTo(string _Radix16, int length)
        {
            byte[] bt = Encoding.Default.GetBytes(_Radix16);
            return HexTo(bt, _Radix16.Length);
        }

        /// <summary>
        /// 16进制字节数组转换会整型
        /// </summary>
        /// <param name="_Radix16"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int HexTo(byte[] _Radix16, int length)
        {
            int n = 0;
            int i = 0;
            try
            {
                for (i = 0; i < length; i++)
                {
                    if (_Radix16[i] >= '0' && _Radix16[i] <= '9')
                        n = n * 16 + _Radix16[i] - '0';
                    if (_Radix16[i] >= 'a' && _Radix16[i] <= 'f')
                        n = n * 16 + _Radix16[i] - 'a' + 10;
                    if (_Radix16[i] >= 'A' && _Radix16[i] <= 'F')
                        n = n * 16 + _Radix16[i] - 'A' + 10;
                }
                return n;
            }
            catch
            {
                return 0;
            }
        }

        public static int HexTo(byte[] _Radix16, int index, int length)
        {
            int n = 0;
            int i = 0;
            for (i = index; i < (index+length); i++)
            {
                if (_Radix16[i] >= '0' && _Radix16[i] <= '9')
                    n = n * 16 + _Radix16[i] - '0';
                if (_Radix16[i] >= 'a' && _Radix16[i] <= 'f')
                    n = n * 16 + _Radix16[i] - 'a' + 10;
                if (_Radix16[i] >= 'A' && _Radix16[i] <= 'F')
                    n = n * 16 + _Radix16[i] - 'A' + 10;
            }
            return n;
        }

        /// <summary>
        /// 使用Microsoft.VisualBasic.Conversion.Hex() 获得字节数组,不够的由0x00填充
        /// </summary>
        /// <param name="b"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(int b, byte length)
        {
            byte[] dst = new byte[length];
            byte[] bt = System.Text.Encoding.Default.GetBytes(Microsoft.VisualBasic.Conversion.Hex(b));
            if (bt.Length < length)
            {
                for (int i = 0; i < (length - bt.Length); i++)
                {
                    dst[i] = 0x30;
                }
                Array.Copy(bt, 0, dst, length - bt.Length, bt.Length);
            }
            else
            {
                Array.Copy(bt, dst, length);
            }
            return dst;
        }

        public static byte[] GetHexBytes(long b, byte length)
        {
            byte[] dst = new byte[length];
            byte[] bt = System.Text.Encoding.Default.GetBytes(Microsoft.VisualBasic.Conversion.Hex(b));
            if (bt.Length < length)
            {
                Array.Copy(bt, dst, bt.Length);
            }
            else
            {
                Array.Copy(bt, dst, length);
            }
            return dst;
        }
        /// <summary>
        /// 返回16进制字符,使用Microsoft.VisualBasic.Conversion.Hex()
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string GetHexString(int b)
        {
            return Microsoft.VisualBasic.Conversion.Hex(b);
        }


        //这里面不能有消息弹出,应开始一个消息弹出线程
        internal static bool Check()
        {
#if (!DEBUG)
            {
                if (Application.CompanyName != "深圳远睿恒软件有限公司" && Application.CompanyName != "UnvaryingSagacity")
                {
                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "本程序属于深圳远睿恒软件有限公司所有, 请不要非法使用.", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //t.Start();
                    //MessageBox.Show("本程序属于深圳远睿恒软件有限公司所有, 请不要非法使用.", "深圳远睿恒软件有限公司");
                    return false;
                }
                string fileName = Application.StartupPath + @"\" + Application.ProductName + ".lic";
                if (!File.Exists(fileName))
                {
                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件不存在， 软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //t.Start();
                    return false;
                }
                License.LicenseContent lic = new UnvaryingSagacity.License.LicenseContent();
                if (!lic.Valid)
                {
                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件已经损坏，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //t.Start();
                    return false;
                }
                if (lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//软件产品//制造商") != Application.CompanyName)
                {
                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件内容不正确，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //t.Start();
                    return false;
                }
                if (lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//软件产品//名称") != Application.ProductName)
                {
                    //ShowMessageboxThread t = new ShowMessageboxThread(null, "许可文件内容与事实不符，软件将无法运行。", "深圳远睿恒软件有限公司", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //t.Start();
                    return false;
                }
                // 不再检查UnvaryingSagacity.Core.dll签名,因为会经常更新
                //string publicKey = lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//加密区//公钥");
                //string privateKey = GetPrivateKey(Application.CompanyName+"出品:" + publicKey);
                //byte[] pKey = Encoding.UTF8.GetBytes(privateKey);
                //byte[] dst = ShaEnCoder.ShaFile(Application.StartupPath + @"\UnvaryingSagacity.Core.dll", pKey);
                //string s1 = ShaEnCoder.HashToString(dst);
                //string s2 = lic.GetContent("深圳远睿恒软件有限公司" + "软件运行许可//签名区//Core");
                return true;
            }
#else
            return (Application.CompanyName == "深圳远睿恒软件有限公司" || Application.CompanyName == "UnvaryingSagacity");
#endif
        }

        public static string GetPrivateKey(string key)
        {
            byte[] src = Encoding.UTF8.GetBytes(key);
            Array.Sort(src);
            return ShaEnCoder.HashToString(src);
        }

        /// <summary>
        /// 获得的是文件版本, 对DLL文件不准确
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static System.Diagnostics.FileVersionInfo GetVersionInfo(string filename)
        {
            return System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
        }

        /// <summary>
        /// 该方法对DLL文件准确,但会锁定文件,导致一定时间不能写
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetVersionOfAssembly(string filename)
        {
            try
            {
                Assembly ass = Assembly.LoadFile(filename);
                AssemblyName asm = ass.GetName();
                string ver=asm.Version.ToString(4);
                return ver;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 比较版本大小,为0则相等,为1则ver1大于ver2,为-1则ver1小于ver2
        /// </summary>
        /// <param name="ver1"></param>
        /// <param name="ver2"></param>
        /// <returns></returns>
        public static int CompareVer(string ver1, string ver2)
        {
            int result = 0;
            string[] ss1 = ver1.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] ss2 = ver2.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            if (ss1.Length >= ss2.Length)
            {
                //以ss2为准
                for (i = 0; i < ss2.Length; i++)
                {
                    int i1 = int.Parse(ss1[i]);
                    int i2 = int.Parse(ss2[i]);
                    result = i1.CompareTo(i2);
                    if (result != 0)
                        return result;
                }
                //到这里时, 如果长度不等则SS1大

                if (ss1.Length > ss2.Length)
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
            {
                //以ss1为准
                for (i = 0; i < ss1.Length; i++)
                {
                    int i1 = int.Parse(ss1[i]);
                    int i2 = int.Parse(ss2[i]);
                    result = i1.CompareTo(i2);
                    if (result != 0)
                        return result;
                }
                return 0;
            }
        }

        /// <summary>
        /// 延时
        /// </summary>
        /// <param name="ms">要延时的毫秒数</param>
        public static void Delay_MS(int ms)
        {
            long tick = DateTime.Now.Ticks;
            while (true)
            {
                Application.DoEvents();
                long tick2 = DateTime.Now.Ticks;
                if ((tick2 - tick) > (ms * 1000))
                {
                    tick = DateTime.Now.Ticks;
                    break;
                }

            }
        }

        /// <summary>
        /// ISO/IEC 13239 CRC计算 (x^16+x^12+x^5+1)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Crc16">返回2字节的校验码</param>
        /// <param name="len">input 长度</param>
        public static void CalCrc16_13239(byte[] input, byte[] Crc16, Int64 len)
        {
            // 
            int temp = 0xffff;
            Int64 i;
            int j;

            for (i = 0; i < len; i++)
            {
                temp = temp ^ ((short)input[i]);
                for (j = 0; j < 8; j++)
                {
                    if ((temp & 0x0001) != 0)
                    {
                        temp = (temp >> 1) ^ 0x8408;
                    }
                    else
                    {
                        temp = (temp >> 1);
                    }
                }
            }

            Crc16[0] = (byte)temp;
            Crc16[1] = (byte)(temp >> 8);
        }

        public static void ShowInfoDialog(IWin32Window owner,string info)
        {
            UIInfoBrower ui = new UIInfoBrower();
            ui.textBox1.Text = info;
            ui.Show(owner); 
        }
    }

    public class ShowMessageboxThread
    {
        string _text;
        string _caption;
        MessageBoxButtons _messageBoxButtons;
        MessageBoxIcon _messageBoxIcon;

        public ShowMessageboxThread( string text, string caption, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            _text = text;
            _caption = caption;
            _messageBoxButtons = messageBoxButtons;
            _messageBoxIcon = messageBoxIcon;
        }

        public void Start()
        {
            Thread thread = new Thread(Show);
            thread.Start(); 
        }

        void Show()
        {
            MessageBox.Show( _text, _caption, _messageBoxButtons, _messageBoxIcon);
        }
    }

    public class XmlSerializer<T>
    {

        public static bool FromXmlSerializer(string filename, out T Items)
        {
            if (!File.Exists(filename))
            {
                Items = default(T);
                return false;
            }
            FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read);
            XmlSerializer newSr = new XmlSerializer(typeof(T));
            //ShaEnCoder.
            try
            {
                Items = (T)newSr.Deserialize(f);
                f.Close();
                return true;
            }
            catch
            {
                Items = default(T);
                f.Close();
                return false;
            }
        }


        public static bool FromXmlSerializer(MemoryStream memStream, out T Items)
        {
            XmlSerializer newSr = new XmlSerializer(typeof(T));
            bool b = false;

            try
            {
                Items = (T)newSr.Deserialize(memStream);
                b = true;
            }
            catch
            {
                Items = default(T); 
            }
            return b;
        }

        public static bool ToXmlSerializer(string filename, T Items)
        {
            TextWriter tr = new StreamWriter(filename, false);
            XmlSerializer sr = new XmlSerializer(typeof(T));
            try
            {
                sr.Serialize(tr, Items);
                tr.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 返回的MemoryStream中丢失了编码方式" encoding="utf-8"", 在使用FromXmlSerializer时会失败
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="Items"></param>
        public static void ToXmlSerializer(MemoryStream memStream, T Items)
        {
            XmlSerializer sr = new XmlSerializer(typeof(T));
            sr.Serialize(memStream, Items);
        }
    }

    public class Log
    {
        StreamWriter w;
        StreamReader r;
        System.Data.SqlClient.SqlConnection _sqlCnn = new System.Data.SqlClient.SqlConnection();
        string _username = "";

        public Log(String logFileName,FileAccess access )
        {
            try
            {
                if (access == FileAccess.Write)
                    w = File.AppendText(logFileName);
                else if (access == FileAccess.Read)
                    r = File.OpenText(logFileName);
            }
            catch { }
        }

        /// <summary>用于Sqlserver
        /// ，检查表Log是否存在，不存在就创建。
        /// 结构：name(用户名),D(yyyyMMddHHmmss),Computer,IP,Action,Description
        /// </summary>
        /// <param name="sqlCnn">sql链接</param>
        public Log(System.Data.SqlClient.SqlConnection sqlCnn,string userName)
        {
            _username = userName;
            Data.DataSchemaProvider.Sql sql = new UnvaryingSagacity.Data.DataSchemaProvider.Sql();
            sql.Connect = sqlCnn;
            if (!sql.ExistTable("Log"))
            {
                int i = 0;
                Data.DataSchemaProvider.DataColumnEx[] cols=new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx[7];
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "name";
                cols[i].DataType = System.Data.DbType.String;
                cols[i].MaxLength = 50;
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "D";
                cols[i].DataType = System.Data.DbType.VarNumeric;
                cols[i].MaxLength = 14;
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "MS";
                cols[i].DataType = System.Data.DbType.VarNumeric;
                cols[i].MaxLength = 3; 
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "Computer";
                cols[i].DataType = System.Data.DbType.String;
                cols[i].MaxLength = 50;
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "IP";
                cols[i].DataType = System.Data.DbType.String;
                cols[i].MaxLength = 50;
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "Action";
                cols[i].DataType = System.Data.DbType.String;
                cols[i].MaxLength = 50;
                i++;
                cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                cols[i].ColumnName = "Description";
                cols[i].DataType = System.Data.DbType.String;
                cols[i].MaxLength = 500;
                sql.CreateTable("Log", cols, new string[5] { "name", "D","MS", "Computer", "Action" });
            }
            _sqlCnn = sqlCnn;
        }

        public void Write(string logMessage)
        {
            try
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
                // Update the underlying file.
                w.Flush();
            }
            catch { }
        }

        /// <summary>用于Sqlserver
        /// 
        /// </summary>
        /// <param name="action">操作</param>
        /// <param name="Description">详细信息</param>
        public void Write( string action, string Description)
        {
            try
            {
                string name = System.Net.Dns.GetHostName();
                string ip = "";
                System.Net.IPAddress[] ips=System.Net.Dns.GetHostAddresses(name);
                if (ips.Length > 0)
                {
                    ip = ips[0].ToString();
                }
                DateTime dt = DateTime.Now;
                string d = dt.ToString("yyyyMMddHHmmss");
                string ms = dt.Millisecond.ToString();
                StringBuilder sb = new StringBuilder("INSERT INTO [Log] (name, D,MS ,Computer, IP, Action, Description) VALUES (");
                sb.Append("'");
                sb.Append(_username);
                sb.Append("','");
                sb.Append(d);
                sb.Append("','");
                sb.Append(ms);
                sb.Append("','");
                sb.Append(name );
                sb.Append("','");
                sb.Append(ip);
                sb.Append("','");
                sb.Append(action);
                sb.Append("','");
                sb.Append(Description);
                sb.Append("')");
                string sql = sb.ToString();
                System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sql, _sqlCnn);
                cm.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "写日志", MessageBoxButtons.OK, MessageBoxIcon.Error);  
            }
        }

        /// <summary>用于Sqlserver，
        /// 根据过滤器查询日志，具体结构请参考Log表定义
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public System.Data.SqlClient.SqlDataReader Read(string filter, string order)
        {
            string sql = "select name, D,Computer, IP, Action, Description from Log";
            if(filter.Length >0)
            {
                sql += " where " + filter;
            }
            if(order .Length >0)
            {
                sql += " order by " + order;
            }
            System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sql, _sqlCnn);
            System.Data.SqlClient.SqlDataReader r = cm.ExecuteReader ();
            return r;
        }

        public string [] Read()
        {
            // While not at the end of the file, read and write lines.
            String line;
            string[] lines = new string[0];
            while ((line = r.ReadLine()) != null)
            {
                Array.Resize<string>(ref lines, lines.Length + 1);
                lines[lines.Length - 1] = line;
                Console.WriteLine(line);
            }
            r.Close();
            return lines;
        }

        /// <summary>
        /// 用于Sqlserver
        /// </summary>
        /// <param name="filter"></param>
        public void Clear(string filter)
        {
            StringBuilder sb = new StringBuilder("Delete from [Log] where ");
            sb.Append(filter);
            string sql = sb.ToString();
            System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sql, _sqlCnn);
            cm.ExecuteNonQuery();
        }
    }

    public static class ShaEnCoder
    {
        public static void ShaFile(String sourceFile, String destFile, byte[] key)
        {
            HMACSHA256 sha256 = new HMACSHA256(key);
            File.WriteAllText(destFile, HashToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(File.ReadAllText(sourceFile, Encoding.UTF8)))));
        }

        public static byte[] ShaFile(String sourceFile, byte[] key)
        {
            HMACSHA256 sha256 = new HMACSHA256(key);
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(File.ReadAllText(sourceFile, Encoding.UTF8)));
        }

        public static byte[] GetHash(byte[] src, byte[] key)
        {
            HMACSHA256 sha256 = new HMACSHA256(key);
            return sha256.ComputeHash(src);
        }

        public static byte[] GetHash(String src, byte[] key)
        {
            byte[] b = Encoding.UTF8.GetBytes(src);
            HMACSHA256 sha256 = new HMACSHA256(key);
            return sha256.ComputeHash(b);
        }

        /// <summary>
        /// 把完成哈希后的字节数组转成数字字符
        /// </summary>
        /// <param name="src">要转的字节数组</param>
        /// <returns></returns>
        public static string HashToString(byte[] src)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                sb.Append(src[i].ToString());
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// 把完成哈希后的字节数组转成数字字符,一个字符由3个数字组成
        /// </summary>
        /// <param name="src">要转的字节数组</param>
        /// <returns></returns>
        public static string HashToString(byte[] src, bool fixLength)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                if (fixLength)
                    sb.Append(src[i].ToString("000"));
                else
                    sb.Append(src[i].ToString());
            }
            return sb.ToString();
        }

        public static byte[] UnHashFromString(string src)
        {
            int i = 0;
            byte[] bs = new byte[0];
            while (i < src.Length)
            {
                Array.Resize<byte>(ref bs, bs.Length + 1);
                if (src.Length < (i + 3))
                    bs[bs.Length - 1] = byte.Parse(src.Substring(i));
                else
                    bs[bs.Length - 1] = byte.Parse(src.Substring(i, 3));
                i = i + 3;
            }
            return bs;
        }

        /// <summary>
        /// 由key中随机产生64个字节的密钥
        /// </summary>
        /// <param name="key">最好是asc码1~255,0会被忽略</param>
        /// <returns></returns>
        public static byte[] GetKey(string key)
        {
            Random r = new Random(key.Length);
            byte[] b = Encoding.UTF8.GetBytes(key);
            byte[] _key = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                while(true)
                {
                    int indexOf=r.Next(b.Length);
                    if (!b[indexOf].Equals(0))
                    {
                        _key[i] = b[indexOf];
                        break;
                    }
                } 
            }
            return _key;
        }
    }

    public class Security
    {
        [DllImport("UnvaryingSagacity.C.dll", EntryPoint = "GetKeys", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        private static extern void GetKeys(int flag, int offset, int length, byte[] key);

        [DllImport("UnvaryingSagacity.C.dll", EntryPoint = "Crypt01", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        private static extern void Crypt01(byte[] input, int inputLength, byte[] key, int keyLength);

        [DllImport("UnvaryingSagacity.C.dll", EntryPoint = "Crypt02", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        private static extern void Crypt02(byte[] buf, int length, int mode);

        public byte[] GetPrivateKeys(int flag, int offset, int length)
        {
            byte[] target = new byte[length];
            GetKeys(flag, offset, length, target);
            return target;
        }

        public void GetSecurity(ref int flag, ref int offset)
        {
            flag = new Random().Next(1, 256);
            offset = new Random().Next(0, 512);
        }

        /// <summary>
        /// 加密原字符串并转成base64编码
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string GetSecurityString(ref int flag, ref int offset, int length, string source)
        {
            GetSecurity(ref flag, ref offset);
            byte[] bt = Encoding.UTF8.GetBytes(source);
            byte[] key = GetPrivateKeys(flag, offset, length);
            rc4_crypt(ref bt, key);
            string s = Convert.ToBase64String(bt);
            return s;
        }

        public string GetSecurityString(int flag, int offset, int length, byte[] source)
        {
            byte[] bt = new byte[source.Length];
            Array.Copy(source, bt, source.Length);
            byte[] key = GetPrivateKeys(flag, offset, length);
            rc4_crypt(ref bt, key);
            string s = Convert.ToBase64String(bt);
            return s;
        }
        /// <summary>
        /// 按base64解码并解密字符串
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string UnSecurityString(int flag, int offset, int length, string source)
        {
            byte[] key = GetPrivateKeys(flag, offset, length);
            byte[] src = Convert.FromBase64String(source);
            rc4_crypt(ref src, key);
            string s = Encoding.UTF8.GetString(src);
            return s;
        }

        public byte[] UnSecurityStringToBytes(int flag, int offset, int length, string source)
        {
            byte[] key = GetPrivateKeys(flag, offset, length);
            byte[] src = Convert.FromBase64String(source);
            rc4_crypt(ref src, key);
            return src;
        }

        /// <summary>
        /// 解码和编码用同一个方法
        /// </summary>
        /// <param name="inbuf">要解码或编码的字节数组</param>
        /// <param name="key">密码</param>
        public void rc4_crypt(ref byte[] inbuf, string key)
        {
            byte[] aryKey;

            aryKey = Encoding.UTF8.GetBytes(key);
            rc4_crypt(ref inbuf, aryKey);
        }

        public void rc4_crypt(ref byte[] inbuf, byte[] key)
        {
            Crypt01(inbuf, inbuf.Length, key, key.Length);
        }

        public void ToPrivateKey(ref byte[] buf)
        {
            Crypt02(buf, buf.Length, 0);
        }
    }

    public static class RC4
    {
        /// <summary>
        /// 初始化密码字典
        /// </summary>
        /// <param name="perm"></param>
        /// <param name="key"></param>
        /// <param name="keylen"></param>
        private static void rc4_init(out int[] perm, byte[] key)
        {
            byte tmp;
            int j=0;
            int i=0;

            perm = new int[256];
            /* Initialize state with identity permutation */
            for (i = 0; i < 256; i++)
            {
                perm[i] = i;
            }
            /* Randomize the permutation using key data */
            for (i = 0; i < 256; i++)
            {
                j = (byte)(j + perm[i] + key[i % key.Length ]);
                //swap_bytes(&perm[i], &perm[j]);
                tmp =(byte ) perm[i];
                perm[i] = perm[j];
                perm[j] = tmp;
            }
        }

        /// <summary>
        /// 解码和编码用同一个方法
        /// </summary>
        /// <param name="inbuf">要解码或编码的字节数组</param>
        /// <param name="key">密码</param>
        public static void rc4_crypt(ref byte[] inbuf,  string key)
        {
            int i=0;
            int j=0;
            int index1 = 0;
            int index2 = 0;
            int tmp;
            int[] perm;
            byte[] aryKey;

            aryKey = Encoding.UTF8.GetBytes(key);
            rc4_init(out perm, aryKey);
            for (i = 0; i < inbuf.Length; i++)
            {
                /* Update modification indicies */
                index1++;
                index2 = (byte )(index2 + perm[index1]);

                /* Modify permutation */
                //swap_bytes(&perm[index1],&perm[index2]);
                tmp = perm[index1];
                perm[index1] = perm[index2];
                perm[index2] = tmp;
                /* Encrypt/decrypt next byte */
                j = (byte)(perm[index1] + perm[index2]);
                inbuf[i] = (byte)(inbuf[i] ^ (byte)(perm[j]));
            }
        }

        public static void rc4_crypt(ref byte[] inbuf, byte[] key)
        {
            int i = 0;
            int j = 0;
            int index1 = 0;
            int index2 = 0;
            int tmp;
            int[] perm;

            rc4_init(out perm, key);
            for (i = 0; i < inbuf.Length; i++)
            {
                /* Update modification indicies */
                index1++;
                index2 = (byte)(index2 + perm[index1]);

                /* Modify permutation */
                //swap_bytes(&perm[index1],&perm[index2]);
                tmp = perm[index1];
                perm[index1] = perm[index2];
                perm[index2] = tmp;
                /* Encrypt/decrypt next byte */
                j = (byte)(perm[index1] + perm[index2]);
                inbuf[i] = (byte)(inbuf[i] ^ (byte)(perm[j]));
            }
        }
    }

    public class FieldDefine
    {
        public string Name { get; set; }
        public string Expression { get; set; }
    }

    public class ExpressionDefine:FieldDefine 
    {
        /// <summary>
        /// 多个参数用逗号风格,参考VB的形参语法
        /// </summary>
        public string Parame { get; set; }
    }

    /// <summary>
    /// VB代码的表达式计算器
    /// 更高级的通过属性赋值计算还没弄明白,要弄清楚类Binder的用法
    /// </summary>
    public sealed class ExpressionCalculator
    {
        private static CodeDomProvider comp = new VBCodeProvider();
        private static CompilerParameters cp = new CompilerParameters();
        private static MethodInfo mi;
        private static CompilerError[] _compilerErrors;
        private CompilerResults _compilerResult;

        /// <summary>
        /// 计算VB代码的表达式,每次都要编译效率很低
        /// </summary>
        /// <param name="expression">表达式,如果返回Null请检查CompilerError</param>
        /// <returns></returns>
        public static object Eval(string expression)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("Imports System");
            codeBuilder.AppendLine("Imports System.Math");
            codeBuilder.AppendLine("Imports Microsoft.VisualBasic");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("Public Module Mode");
            codeBuilder.AppendLine("   Public Function Func() As Object");
            codeBuilder.AppendLine("        Return " + expression);
            codeBuilder.AppendLine("   End Function");
            codeBuilder.AppendLine("End Module");

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            string code = codeBuilder.ToString();
            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code);

            if (cr.Errors.HasErrors)
            {
                _compilerErrors = new CompilerError[cr.Errors.Count];
                cr.Errors.CopyTo(_compilerErrors, 0);
                return null;
            }
            else
            {
                try
                {
                    Assembly a = cr.CompiledAssembly;
                    Type t = a.GetType("Mode");
                    mi = t.GetMethod("Func", BindingFlags.Static | BindingFlags.Public);
                    return mi.Invoke(null, new object[0]);
                }
                catch (Exception ex)
                {
                    _compilerErrors = new CompilerError[1];
                    _compilerErrors[0] = new CompilerError();
                    _compilerErrors[0].ErrorText = ex.InnerException.Message;
                    _compilerErrors[0].IsWarning = false;
                    return default(object);
                }
            }
        }
        
        /// <summary>
        /// 计算VB代码的表达式,每次都要编译效率很低
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="vars">格式参看VB的变量定义,如:Dim b as string</param>
        /// <returns></returns>
        public static object Eval(string expression,string [] vars)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("Imports System");
            codeBuilder.AppendLine("Imports System.Math");
            codeBuilder.AppendLine("Imports Microsoft.VisualBasic");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("Public Module Mode");
            codeBuilder.AppendLine("   Public Function Func() As Object");
            foreach(string var in vars )
            {
                codeBuilder.AppendLine(var);
            }
            codeBuilder.AppendLine("        Return " + expression);
            codeBuilder.AppendLine("   End Function");
            codeBuilder.AppendLine("End Module");

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            string code = codeBuilder.ToString();
            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code);

            if (cr.Errors.HasErrors)
            {
                _compilerErrors = new CompilerError[cr.Errors.Count];
                cr.Errors.CopyTo(_compilerErrors, 0);
                return null;
            }
            else
            {
                try
                {
                    Assembly a = cr.CompiledAssembly;
                    Type t = a.GetType("Mode");
                    //object mode = a.CreateInstance("Mode");
                    mi = t.GetMethod("Func", BindingFlags.Static | BindingFlags.Public);
                    return mi.Invoke(null, new object[0]);
                }
                catch { return default(object); }
            }
        }

        public static CompilerError[] CompilerErrors
        {
            get { return _compilerErrors; }
        }

        /// <summary>
        /// 通过公共字段计算比参数要慢,测试了有120个打印项的票证
        /// </summary>
        public bool FieldMode { get; set; }
        /// <summary>
        /// 通过参数计算,只编译一次效率很高
        /// </summary>
        /// <param name="expression">变量作为函数的参数传递</param>
        /// <returns></returns>
        public bool CreateCalculator(ExpressionDefine[] exps)
        {
            //Core.General.Check();
            if (!Core.General.Check())
                return false;
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("Imports System");
            codeBuilder.AppendLine("Imports System.Math");
            codeBuilder.AppendLine("Imports Microsoft.VisualBasic");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("Public Class Mode");

            for (int i = 0; i < exps.Length; i++)
            {
                codeBuilder.AppendLine("   Public Function " + exps[i].Name + "(" + exps[i].Parame + ") As Object");
                codeBuilder.AppendLine("        Return " + exps[i].Expression);
                codeBuilder.AppendLine("   End Function");
            }
            codeBuilder.AppendLine("End Class");

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            string code = codeBuilder.ToString();
            _compilerResult = comp.CompileAssemblyFromSource(cp, code);

            if (_compilerResult.Errors.HasErrors)
            {
                _compilerErrors = new CompilerError[_compilerResult.Errors.Count];
                _compilerResult.Errors.CopyTo(_compilerErrors, 0);
                _compilerResult = null;
                return false;
            }
            return true;
        }

        public bool CreateCalculator(string[] Fields, FieldDefine[] exps)
        {
            //Core.General.Check();
            ////if (!Core.General.Check())
            ////    return false;
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("Imports System");
            codeBuilder.AppendLine("Imports System.Math");
            codeBuilder.AppendLine("Imports Microsoft.VisualBasic");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("Public Class Mode");
            foreach (string s in Fields)
            {
                codeBuilder.AppendLine(s);
            }
            for (int i = 0; i < exps.Length; i++)
            {
                codeBuilder.AppendLine("   Public Function " + exps[i].Name + "() As Object");
                codeBuilder.AppendLine("        Return " + exps[i].Expression);
                codeBuilder.AppendLine("   End Function");
            }
            codeBuilder.AppendLine("End Class");

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            string code = codeBuilder.ToString();
            _compilerResult = comp.CompileAssemblyFromSource(cp, code);

            if (_compilerResult.Errors.HasErrors)
            {
                _compilerErrors = new CompilerError[_compilerResult.Errors.Count];
                _compilerResult.Errors.CopyTo(_compilerErrors, 0);
                _compilerResult = null;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 根据CreateCalculator得到的类执行方法,效率很高
        /// </summary>
        /// <param name="param">方法名称</param>
        /// <param name="param">方法的实参,注意类型与顺序</param>
        /// <returns></returns>
        public object Eval(string methodName ,object [] param)
        {
            if (_compilerResult != null)
            {
                Assembly a = _compilerResult.CompiledAssembly;

                Type t = a.GetType("Mode");
                object mode = a.CreateInstance(t.Name);
                mi = t.GetMethod(methodName);
                try
                {
                    object r = mi.Invoke(mode, BindingFlags.Public | BindingFlags.Instance, null, param, CultureInfo.CurrentCulture);
                    return r;
                }
                catch
                { 
                     
                    return default(object);
                }
            }
            else
                return default(object);
        }

        /// <summary>
        /// 根据CreateCalculator得到的类执行方法,效率很高
        /// </summary>
        /// <param name="param">方法名称</param>
        /// <param name="param">方法的实参,注意类型与顺序</param>
        /// <returns></returns>
        public object Eval(string methodName, PropertyItem [] fields)
        {
            if (_compilerResult != null)
            {
                Assembly a = _compilerResult.CompiledAssembly;

                Type t = a.GetType("Mode");
                object mode = a.CreateInstance(t.Name);
                foreach (PropertyItem item in fields)
                {
                    if (t.GetField(item.Name) != null)
                    {
                        t.InvokeMember(item.Name, BindingFlags.SetField, null, mode, new Object[] { item.Value });
                    }
                }
                mi = t.GetMethod(methodName);
                try
                {
                    object r = mi.Invoke(mode, BindingFlags.Public | BindingFlags.Instance, null, null, CultureInfo.CurrentCulture);
                    return r;
                }
                catch
                { return default(object); }
            }
            else
                return default(object);
        }
    }

    public class ToolTip
    {
        private TipForm _tip;
        private bool _showing = false;
        private double _opacity = 0.75;
        private string _tipMessage = "";
        private Color _backColor = Color.FromArgb(255, 255, 192);
        private bool _isPrompt = false;

        public ToolTip(){}

        public bool Showing
        {
            get { return _showing; }
        }

        public Rectangle Bounds { get; set; }

        public string Message { get; set; }

        public void Show(IWin32Window owner, Rectangle rt, string s)
        {
            Bounds = rt;
            Message = s;
            if (_tip == null)
            {
                _tip = new TipForm();
            }
            _tip.Bounds = rt;
            _tipMessage = s;
            _opacity = 100;
            _tip.BackColor = _backColor;
            _tip.Load += new EventHandler(_tip_Load);
            _tip.Paint += new PaintEventHandler(_tip_Paint);
            if (_showing)
                _tip.Visible = true;
            else
                _tip.Show(owner);
            _showing = true;
            _tip.Refresh();
            Application.DoEvents();
        }

        public void Show(IWin32Window owner, Rectangle rt, string s,bool IsPrompt)
        {
            if (IsPrompt)
            {
                _opacity = 100;
                _backColor = Color.SkyBlue;
            }
            _isPrompt = IsPrompt;
            Show(owner,rt, s);
        }

        public void ShowPrompt(IWin32Window owner,string s)
        {
            Size sz = TextRenderer.MeasureText(s, (owner as Form).Font);
            sz.Width += 150;
            sz.Height += 50;
            Rectangle rt = new Rectangle(((owner as Form).Width - sz.Width) / 2, ((owner as Form).Height - sz.Height) / 2, sz.Width, sz.Height);
            rt = (owner as Form).RectangleToScreen(rt);
            _opacity = 100;
            _backColor = Color.SkyBlue;
            _isPrompt = true;
            Show(owner, rt, s);
        }

        void _tip_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(_tipMessage, _tip.Font, new SolidBrush(_tip.ForeColor ), new Rectangle(new Point(0, 0), _tip.Bounds.Size), sf);
        }

        public void Hide()
        {
            _showing = false;
            if (_tip != null)
            {
                _tip.Close();
                _tip.Dispose();
                _tip = null;
            }
        }

        void _tip_Load(object sender, EventArgs e)
        {
            _tip.Bounds = Bounds;
            _tip.Opacity = _opacity;
            
        }
        
    }

    public class InputBox
    {
        private string _title = "InputBox";
        private string _prompt = "prompt";
        private object _result = default(object);
        private int _maxLength = 250;
        private string _defaultValue = "";

        private System.Drawing.Design.UITypeEditorEditStyle _editStyle = System.Drawing.Design.UITypeEditorEditStyle.None;

        public object[] DropdownCollection { get; set; }

        public ComboBoxStyle DropDownStyle { get; set; }

        public InputBox(string title, string prompt)
        {
            _title = title;
            _prompt = prompt;
            gf = new GeneralForm();
        }

        public InputBox(string title, string prompt, int maxLength, string defultValue)
        {
            _title = title;
            _prompt = prompt;
            _maxLength = maxLength;
            _defaultValue = defultValue;
            gf = new GeneralForm();
        }

        public InputBox(string title, string prompt, int maxLength, System.Drawing.Design.UITypeEditorEditStyle EditStyle)
        {
            _title = title;
            _prompt = prompt;
            _maxLength = maxLength;
            _editStyle = EditStyle;
            gf = new GeneralForm();
        }

        public object Result { get { return _result; } }

        public DialogResult  ShowDialog(System.Windows.Forms.IWin32Window owner)
        {
            InitializeComponent();
            return gf.ShowDialog(owner);
        }

        #region Windows 窗体设计器生成的代码
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = _prompt;
            if (_editStyle == System.Drawing.Design.UITypeEditorEditStyle.DropDown)
            {
                // 
                // comboBox1
                // 
                this.comboBox1.Location = new System.Drawing.Point(15, 103);
                this.comboBox1.Name = "textBox1";
                this.comboBox1.MaxLength = _maxLength;
                this.comboBox1.Size = new System.Drawing.Size(383, 21);
                this.comboBox1.ImeMode = ImeMode.On; 
                this.comboBox1.TabIndex = 1;
                this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; 
                this.textBox1.Visible = false;
                if (DropdownCollection != null)
                    this.comboBox1.Items.AddRange(DropdownCollection);
            }
            else
            {
                // 
                // textBox1
                // 
                this.textBox1.Location = new System.Drawing.Point(15, 103);
                this.textBox1.Name = "textBox1";
                this.textBox1.MaxLength = _maxLength;
                this.textBox1.Size = new System.Drawing.Size(383, 21);
                this.textBox1.TabIndex = 1;
                this.textBox1.ImeMode = ImeMode.On; 
                this.textBox1.Text = _defaultValue;
                this.comboBox1.Visible = false;
            }
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(323, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += delegate
            {
                if (_editStyle == System.Drawing.Design.UITypeEditorEditStyle.DropDown)
                    _result = comboBox1.Text;
                else
                    _result = textBox1.Text;
                gf.DialogResult = DialogResult.OK; 
                gf.Close();
            };
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(323, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button1.Click += delegate
            {
                gf.Close();
            };
            // 
            // Form1
            // 
            this.gf = new GeneralForm();
            gf.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            gf.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            gf.ClientSize = new System.Drawing.Size(410, 145);
            gf.ControlBox = false;
            gf.Controls.Add(this.button2);
            gf.Controls.Add(this.button1);
            if (_editStyle == System.Drawing.Design.UITypeEditorEditStyle.DropDown)
                gf.Controls.Add(this.comboBox1);
            else
                gf.Controls.Add(this.textBox1);
            gf.Controls.Add(this.label1);
            gf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            gf.Name = "generalForm_Inputbox";
            gf.Text = _title;
            gf.AcceptButton = button1;
            gf.CancelButton = button2;
            gf.ResumeLayout(false);
            gf.PerformLayout();

        }
        private GeneralForm gf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        #endregion
    }

    public class DoubleInputBox
    {

        private string _title = "InputBox";
        private string _prompt1 = "prompt";
        private string _prompt2 = "prompt";
        private string _result1 = "";
        private string _result2 = "";
        private string _defaultValue1 = "";
        private string _defaultValue2 = "";

        public DoubleInputBox(string title, string prompt1, string prompt2, string defultValue1, string defultValue2)
        {
            _title = title;
            _prompt1 = prompt1;
            _prompt2 = prompt2;
            _defaultValue1 = defultValue1;
            _defaultValue2 = defultValue2;
            gf = new GeneralForm();
            gf.Shown += new EventHandler(gf_Shown);
        }

        void gf_Shown(object sender, EventArgs e)
        {
            this.textBox1.Text = _defaultValue1;
            this.textBox2.Text = _defaultValue2; 
        }

        public string Result1 { get { return _result1; } }

        public string Result2 { get { return _result2; } }

        public DialogResult  ShowDialog(System.Windows.Forms.IWin32Window owner)
        {
            InitializeComponent();
            return gf.ShowDialog(owner);
        }

        void button2_Click(object sender, EventArgs e)
        {
            gf.Close();
        }

        void button1_Click(object sender, EventArgs e)
        {
            _result1 = textBox1.Text;
            _result2 = textBox2.Text;
            gf.DialogResult = DialogResult.OK;
            gf.Close();
        }

        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = _prompt1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 39);
            this.textBox1.Name = "textBox1";
            this.textBox1.Font = new Font("宋体", 9, FontStyle.Regular);   
            this.textBox1.Size = new System.Drawing.Size(318, 21);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = _defaultValue1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = _prompt2 ;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(26, 91);
            this.textBox2.Name = "textBox2";
            this.textBox2.Font = new Font("宋体", 9, FontStyle.Regular);
            this.textBox2.Size = new System.Drawing.Size(318, 21);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = _defaultValue2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(178, 129);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(269, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(button2_Click); 
            // 
            // GeneralForm
            // 
            this.gf = new GeneralForm();
            gf.AcceptButton = this.button1;
            gf.CancelButton = this.button2;
            gf.ClientSize = new System.Drawing.Size(356, 164);
            gf.Controls.Add(this.button2);
            gf.Controls.Add(this.button1);
            gf.Controls.Add(this.textBox2);
            gf.Controls.Add(this.label2);
            gf.Controls.Add(this.textBox1);
            gf.Controls.Add(this.label1);
            gf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            gf.MaximizeBox = false;
            gf.MinimizeBox = false;
            gf.Name = _title;
            gf.Text = _title;
            gf.ShowIcon = false;
            gf.ShowInTaskbar = false;
            gf.ResumeLayout(false);
            gf.PerformLayout();

        }

        private GeneralForm gf; 
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        #endregion

    }

    public class SelectDateBox
    {
        DateTime _d1;
        DateTime _d2;
        bool _cancel;
        bool _monthViewStyle;

        public DateTime StartDate { get { return _d1; } }

        public DateTime EndDate { get { return _d2; } }

        public bool Cancel { get { return _cancel; } }

        public SelectDateBox(DateTime d1, DateTime d2)
        {
            InitializeComponent();
            gf.AutoSize = true;
            dateTimePicker1.Value = d1;
            dateTimePicker2.Value = d2;
            monthCalendar1.SetDate(d1);
            monthCalendar2.SetDate(d2);
            _d1 = d1;
            _d2 = d2;
            _cancel = true;
        }

        public SelectDateBox(DateTime d1)
        {
            InitializeComponent();
            dateTimePicker1.Value = d1;
            monthCalendar1.TodayDate = d1;
            this.monthCalendar1.Dock = DockStyle.None;
            //this.monthCalendar1.Left = (this.panel1.Width - this.monthCalendar1.Width) / 2;
            gf.Controls.Remove(label2);
            gf.Controls.Remove(dateTimePicker2);
            gf.Controls.Remove(monthCalendar2);
            _d1 = d1;
            _cancel = true;
        }

        public void StartDateRange(DateTime min, DateTime max)
        {
            dateTimePicker1.MinDate = min;
            dateTimePicker1.MaxDate = max;
            monthCalendar1.MinDate = min;
            monthCalendar1.MaxDate = max;
        }

        public void EndDateRange(DateTime min, DateTime max)
        {
            dateTimePicker2.MinDate = min;
            dateTimePicker2.MaxDate = max;
            monthCalendar2.MinDate = min;
            monthCalendar2.MaxDate = max;
        }

        public void ShowDialog(System.Windows.Forms.IWin32Window owner)
        {
            ShowDialog(owner,false );
        }

        public void ShowDialog(System.Windows.Forms.IWin32Window owner,bool monthViewStyle)
        {
            _monthViewStyle = monthViewStyle;
            if (_monthViewStyle)
            {
                gf.Controls.Remove(label1);
                gf.Controls.Remove(dateTimePicker1);
                gf.Controls.Remove(label2);
                gf.Controls.Remove(dateTimePicker2);
            }
            else
            {
                gf.Controls.Remove(monthCalendar1);
                gf.Controls.Remove(monthCalendar2);
            }
            //gf.AutoSize = true;

            gf.ShowDialog(owner);
            gf.Dispose();
            gf = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_monthViewStyle)//日历模式时在事件中赋值
            {
                _d1 = dateTimePicker1.Value;
                _d2 = dateTimePicker2.Value;
            }
            else
            {
                _d1 = monthCalendar1.SelectionStart;
                _d2 = monthCalendar2.SelectionStart;
            }
            _cancel = false;
            gf.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gf.Close();
        }

        void monthCalendar1_MouseUp(object sender, MouseEventArgs e)
        {
            //模拟双击事件
            if (_mouseDownTick == 0)//第一次单击
            {
                _mouseDownRectangle = new Rectangle(
                        e.X - (SystemInformation.DoubleClickSize.Width / 2),
                        e.Y - (SystemInformation.DoubleClickSize.Height / 2),
                        SystemInformation.DoubleClickSize.Width,
                        SystemInformation.DoubleClickSize.Height);
                _mouseDownTick = DateTime.Now.Ticks;
            }
            else//判断双击
            {
                long nowTicks = DateTime.Now.Ticks;
                //long oldTicks = _mouseDownTick;
                if (_mouseDownRectangle.Contains(e.Location) && ((nowTicks - _mouseDownTick) < (SystemInformation.DoubleClickTime * 10000)))
                {
                   //双击
                    button1_Click(null, new EventArgs());
                }
                Console.WriteLine("{0},{1},{2}", (nowTicks - _mouseDownTick), (SystemInformation.DoubleClickTime * 10000), ((nowTicks - _mouseDownTick) < (SystemInformation.DoubleClickTime * 10000)));
               _mouseDownRectangle = new Rectangle(
                      e.X - (SystemInformation.DoubleClickSize.Width / 2),
                      e.Y - (SystemInformation.DoubleClickSize.Height / 2),
                      SystemInformation.DoubleClickSize.Width,
                      SystemInformation.DoubleClickSize.Height);
               _mouseDownTick = nowTicks;
            }
        }

        void monthCalendar2_DateSelected(object sender, DateRangeEventArgs e)
        {
            _d2 = e.Start;
        }

        void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            _d1 = e.Start;
        }
        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {

            this.gf = new GeneralForm();
            this.panel1 = new Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.monthCalendar2 = new System.Windows.Forms.MonthCalendar();
            gf.SuspendLayout();

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "截止日期";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "开始日期";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy年M月d日";
            this.dateTimePicker2.Location = new System.Drawing.Point(40, 97);
            this.dateTimePicker2.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker2.MinDate = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(121, 21);
            this.dateTimePicker2.TabIndex = 15;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy年M月d日";
            this.dateTimePicker1.Location = new System.Drawing.Point(40, 37);
            this.dateTimePicker1.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker1.MinDate = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(121, 21);
            this.dateTimePicker1.TabIndex = 12;
            // 
            // button2
            // 
            this.button2.Dock = DockStyle.Right; 
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 25);
            this.button2.TabIndex = 14;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Dock = DockStyle.Right; 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 13;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Dock = System.Windows.Forms.DockStyle.Left;
            this.monthCalendar1.Location = new System.Drawing.Point(30, 0);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateSelected += new DateRangeEventHandler(monthCalendar1_DateSelected);
            this.monthCalendar1.MouseUp += new MouseEventHandler(monthCalendar1_MouseUp);//模拟双击事件
            // 
            // monthCalendar2
            // 
            this.monthCalendar2.Dock = System.Windows.Forms.DockStyle.Right;
            this.monthCalendar2.Name = "monthCalendar2";
            this.monthCalendar2.TabIndex = 1;
            this.monthCalendar2.DateSelected += new DateRangeEventHandler(monthCalendar2_DateSelected);
            //
            // panel1
            //
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Size = new Size(100, 40);
            this.panel1.Padding = new Padding(10, 0, 5, 15); 
            this.panel1.Controls.Add(this.button1);            
            this.panel1.Controls.Add(this.button2);
            // 
            // SelectDateUI
            // 
            gf.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            gf.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            gf.ClientSize = new System.Drawing.Size(330, 212);
            gf.Padding = new Padding(3);
            gf.Controls.Add(this.label1);
            gf.Controls.Add(this.dateTimePicker1);
            gf.Controls.Add(this.label2);
            gf.Controls.Add(this.dateTimePicker2);
            //gf.Controls.Add(this.button2);
            //gf.Controls.Add(this.button1);
            gf.Controls.Add(this.monthCalendar2);
            gf.Controls.Add(this.monthCalendar1);
            gf.Controls.Add(this.panel1);
            gf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            gf.MaximizeBox = false;
            gf.MinimizeBox = false;
            gf.Name = "SelectDateUI";
            gf.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            gf.Text = "选择日期";
            gf.ResumeLayout(false);
            gf.PerformLayout();

        }

        private Rectangle _mouseDownRectangle = new Rectangle();
        private long _mouseDownTick = 0;

        private GeneralForm gf;
        private Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.MonthCalendar monthCalendar2;

        #endregion

    }

    public class XmlExplorer
    {
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        private string mFileName;
        private XmlDocument mxDoc = new XmlDocument();
        private string _errInfo="";

        public string FileName
        {
            get { return mFileName; }
        }

        /// <summary>
        /// 获得最近的错误信息
        /// </summary>
        public string ErrorMessage { get { return _errInfo; } }

        /// <summary>
        /// 获得当前的XmlDocument
        /// </summary>
        public XmlDocument XmlDoc { get { return mxDoc; } }

        public bool CreateFile(string FileName, string Root)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    StreamWriter w = File.CreateText(FileName);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<?xml version=");
                    sb.Append((char)34);
                    sb.Append("1.0");
                    sb.Append((char)34);
                    sb.Append(" encoding=");
                    sb.Append((char)34);
                    sb.Append("UTF-8");
                    sb.Append((char)34);
                    sb.Append("?>");
                    sb.Append((char)13);
                    sb.Append((char)10);
                    sb.Append("<");
                    sb.Append(Root);
                    sb.Append(">");
                    sb.Append("</");
                    sb.Append(Root);
                    sb.Append(">");
                    w.Write(sb.ToString());
                    w.Close();
                    return true;
                }
                else
                {
                    _errInfo = "文件已经存在";
                    return false;
                }
            }
            catch (Exception ex)
            {
                _errInfo = ex.Message;
                return false;
            }
        }

        public bool OpenFile(string FileName)
        {
            mFileName = FileName;
            try
            {
                mxDoc.Load(mFileName);
                return true;
            }
            catch (Exception ex)
            {
                _errInfo = ex.Message;// "文件[" + mFileName + "]不存在或不是一个合法的XML文件.";
                return false;
            }

        }
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public string ReadValue(string XPath, string Key, string DefultValue)
        {
            string s = "";
            try
            {
                XmlNode xNode = mxDoc.SelectSingleNode(XPath + "//" + Key);
                if (xNode != null)
                {
                    s = xNode.InnerText;
                }
                else
                    s = DefultValue;
                return s;
            }
            catch { return s; }
        }
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public bool WriteValue(string XPath, string Key, string Value)
        {
            return WriteValue(XPath, Key, Value, false);
        }
        public bool WriteValue(string XPath, string Key, string Value,bool delaySave)
        {
            XmlNode xNodeTemp;
            xNodeTemp = mxDoc.CreateElement(Key);
            xNodeTemp.InnerText = Value;
            XmlNode xNode = mxDoc.SelectSingleNode(XPath);
            if (xNode != null)
            {
                if (!Exist(XPath + "//" + Key))
                {
                    xNode.AppendChild(xNodeTemp);
                }
                else
                {
                    xNode = mxDoc.SelectSingleNode(XPath + "//" + Key);
                    xNode.InnerText = Value;
                }
                if (!delaySave)
                    mxDoc.Save(mFileName);
                return true;
            }
            else
                return false;
        }
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public bool CreateNode(string ParentXPath, string name, string attributeName, string attributeValue)
        {
            string xPath = "";
            if (attributeName.Length > 0)
                xPath = ParentXPath + "//" + name + "[@" + attributeName + "='" + attributeValue + "']";
            else
                xPath = ParentXPath + "//" + name;
            if (!Exist(xPath))
            {
                XmlNode xNode = mxDoc.SelectSingleNode(ParentXPath);
                if (xNode != null)
                {
                    XmlNode xNodeTemp;
                    xNodeTemp = mxDoc.CreateNode(XmlNodeType.Element, name, xNode.NamespaceURI);
                    if (attributeName.Length > 0)
                    {
                        XmlAttribute xAttrNode = mxDoc.CreateAttribute(attributeName);
                        xAttrNode.Value = attributeValue;
                        xNodeTemp.Attributes.Append(xAttrNode);
                    }
                    xNode.AppendChild(xNodeTemp);
                    mxDoc.Save(mFileName);
                    return true;
                }
                else
                    return false;
            }
            else
                return true;

        }
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public string[] NodeList(string XPath)
        {
            string[] s = new string[0];
            int i = 0;

            if (XPath.Length > 0)
            {
                // Find a group of nodes based upon the enterd XPath expression.
                try
                {
                    XmlNodeList mxNodeList = mxDoc.SelectNodes(XPath);
                    if (!(mxNodeList == null))
                    {
                        foreach (XmlNode xNode in mxNodeList)
                        {
                            foreach (XmlNode xChild in xNode.ChildNodes)
                            {
                                i++;
                                Array.Resize<string>(ref s, i);
                                if (xChild.Attributes.Count <= 0)
                                    s[i - 1] = xChild.Name;
                                else
                                    s[i - 1] = xChild.Name + "[@" + xChild.Attributes.Item(0).Name + "='" + xChild.Attributes.Item(0).Value + "']";
                            }
                        }
                    }
                }
                catch
                {
                    return s;
                }
            }
            return s;
        }

        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public string[] KeyList(string XPath)
        {
            string[] s = new string[0];
            int i = 0;

            if (XPath.Length > 0)
            {
                // Find a group of nodes based upon the enterd XPath expression.
                try
                {
                    XmlNodeList mxNodeList = mxDoc.SelectNodes(XPath);
                    if (!(mxNodeList == null))
                    {
                        foreach (XmlNode xNode in mxNodeList)
                        {
                            if (xNode.HasChildNodes)
                            {
                                foreach (XmlElement xElement in xNode.ChildNodes)
                                {
                                    i++;
                                    Array.Resize<string>(ref s, i);
                                    s[i - 1] = xElement.Name;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return s;
                }
            }
            return s;
        }
        /// <summary>XPath的格式说明
        /// //父节点[@属性='属性值']//子节点[@属性='属性值']
        /// 方括号及其内容可以不存在
        /// 例如://Department//Item[@ID='B3']
        /// </summary>
        public bool Exist(string XPath)
        {
            bool b = false;
            XmlNode xNode = mxDoc.SelectSingleNode(XPath);
            if (xNode != null)
            {
                b = true;
            }
            return b;
        }
    }

    /// <summary>
    /// 绘制文字阴影.
    /// 先画文字阴影,再画文字.
    /// </summary>
    public class TextShadow
    {
        private int radius = 3;
        private int distance = 5;
        private double angle = 45;
        private byte alpha = 192;

        /// <summary>
        /// 高斯卷积矩阵
        /// </summary>
        private int[] gaussMatrix;
        /// <summary>
        /// 卷积核
        /// </summary>
        private int nuclear = 0;

        /// <summary>
        /// 阴影半径
        /// </summary>
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (radius != value)
                {
                    radius = value;
                    MakeGaussMatrix();
                }
            }
        }

        /// <summary>
        ///  阴影距离
        /// </summary>
        public int Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        /// <summary>
        ///  阴影输出角度(左边平行处为0度。顺时针方向)
        /// </summary>
        public double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        /// <summary>
        /// 阴影文字的不透明度
        /// </summary>
        public byte Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }

        /// <summary>
        /// 对文字阴影位图按阴影半径计算的高斯矩阵进行卷积模糊
        /// </summary>
        /// <param name="bmp">文字阴影位图</param>
        private unsafe void MaskShadow(Bitmap bmp)
        {
            if (nuclear == 0)
                MakeGaussMatrix();
            Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            // 克隆临时位图，作为卷积源
            Bitmap tmp = (Bitmap)bmp.Clone();
            BitmapData dest = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            BitmapData source = tmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                // 源首地址(0, 0)的Alpha字节，也就是目标首像素的第一个卷积乘数的像素点
                byte* ps = (byte*)source.Scan0;
                ps += 3;
                // 目标地址为卷积半径点(radius, radius)的Alpha字节
                byte* pd = (byte*)dest.Scan0;
                pd += (radius * (dest.Stride + 4) + 3);
                // 位图实际卷积的部分
                int width = dest.Width - radius * 2;
                int height = dest.Height - radius * 2;
                int matrixSize = radius * 2 + 1;
                // 卷积矩阵字节偏移
                int mOffset = dest.Stride - matrixSize * 4;
                // 行尾卷积半径(radius)的偏移
                int rOffset = radius * 8;
                int count = matrixSize * matrixSize;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                        byte* s = ps - mOffset;
                        int v = 0;
                        for (int i = 0; i < count; i++, s += 4)
                        {
                            if ((i % matrixSize) == 0)
                                s += mOffset;           // 卷积矩阵的换行
                            v += gaussMatrix[i] * *s;   // 位图像素点Alpha的卷积值求和
                        }
                        // 目标位图被卷积像素点Alpha等于卷积和除以卷积核
                        *pd = (byte)(v / nuclear);
                        pd += 4;
                        ps += 4;
                    }
                    pd += rOffset;
                    ps += rOffset;
                }
            }
            finally
            {
                tmp.UnlockBits(source);
                bmp.UnlockBits(dest);
                tmp.Dispose();
            }
        }

        /// <summary>
        /// 按给定的阴影半径生成高斯卷积矩阵
        /// </summary>
        protected virtual void MakeGaussMatrix()
        {
            double Q = (double)radius / 2.0;
            if (Q == 0.0)
                Q = 0.1;
            int n = radius * 2 + 1;
            int index = 0;
            nuclear = 0;
            gaussMatrix = new int[n * n];

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    gaussMatrix[index] = (int)Math.Round(Math.Exp(-((double)x * x + y * y) / (2.0 * Q * Q)) /
                                                         (2.0 * Math.PI * Q * Q) * 1000.0);
                    nuclear += gaussMatrix[index];
                    index++;
                }
            }
        }

        /// <summary>
        /// radius = 3, distance = 5, angle = 45, alpha = 192;
        /// </summary>
        public TextShadow()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public TextShadow(int radius, int distance, double angle, byte alpha)
        {
            this.radius = radius;
            this.distance = distance;
            this.angle = angle;
            this.alpha = alpha;
        }
        /// <summary>
        /// 画文字阴影
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="text">文字串</param>
        /// <param name="font">字体</param>
        /// <param name="layoutRect">文字串的布局矩形</param>
        /// <param name="format">文字串输出格式</param>
        public void Draw(Graphics g, string text, Font font, RectangleF layoutRect, StringFormat format)
        {
            RectangleF sr = new RectangleF((float)(radius * 2), (float)(radius * 2), layoutRect.Width, layoutRect.Height);
            // 根据文字布局矩形长宽扩大文字阴影半径4倍建立一个32位ARGB格式的位图
            Bitmap bmp = new Bitmap((int)sr.Width + radius * 4, (int)sr.Height + radius * 4, PixelFormat.Format32bppArgb);
            // 按文字阴影不透明度建立阴影画刷
            Brush brush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
            Graphics bg = Graphics.FromImage(bmp);
            try
            {
                // 在位图上画文字阴影
                bg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                bg.DrawString(text, font, brush, sr, format);
                // 制造阴影模糊
                MaskShadow(bmp);
                // 按文字阴影角度、半径和距离输出文字阴影到给定的画布
                RectangleF dr = layoutRect;
                dr.Offset((float)(Math.Cos(Math.PI * angle / 180.0) * distance),
                          (float)(Math.Sin(Math.PI * angle / 180.0) * distance));
                sr.Inflate((float)radius, (float)radius);
                dr.Inflate((float)radius, (float)radius);
                g.DrawImage(bmp, dr, sr, GraphicsUnit.Pixel);
            }
            finally
            {
                bg.Dispose();
                brush.Dispose();
                bmp.Dispose();
            }
        }

        /// <summary>
        /// 画文字阴影
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="text">文字串</param>
        /// <param name="font">字体</param>
        /// <param name="layoutRect">文字串的布局矩形</param>
        public void Draw(Graphics g, string text, Font font, RectangleF layoutRect)
        {
            Draw(g, text, font, layoutRect, null);
        }

        /// <summary>
        /// 画文字阴影
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="text">文字串</param>
        /// <param name="font">字体</param>
        /// <param name="origin">文字串的输出原点</param>
        /// <param name="format">文字串输出格式</param>
        public void Draw(Graphics g, string text, Font font, PointF origin, StringFormat format)
        {
            RectangleF rect = new RectangleF(origin, g.MeasureString(text, font, origin, format));
            Draw(g, text, font, rect, format);
        }

        /// <summary>
        /// 画文字阴影
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="text">文字串</param>
        /// <param name="font">字体</param>
        /// <param name="origin">文字串的输出原点</param>
        public void Draw(Graphics g, string text, Font font, PointF origin)
        {
            Draw(g, text, font, origin, null);
        }
    }

    /// <summary>
    /// 把树形结构的所有元素转成扁平的数组,用于快速遍历
    /// </summary>
    public class TreeNodeToArrary<T>
    {
        public T ID { get; set; }
        public T Parent { get; set; }
    }
}



