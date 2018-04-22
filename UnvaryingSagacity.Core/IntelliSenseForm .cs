using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    public partial class IntelliSenseForm : Form
    {

        private AutoCompleteStringCollection _stringCollection;

        public IntelliSenseForm(AutoCompleteStringCollection stringColl)
        {
            _stringCollection = stringColl;
            InitializeComponent();
            foreach(string s in stringColl )
            {
                label1.Text = label1.Text + "\n" + s;
            }
            this.MouseDown += new MouseEventHandler(UIDropDownList_MouseDown);
            this.KeyDown += new KeyEventHandler(UIDropDownList_KeyDown);
            this.label1.MouseClick += new MouseEventHandler(label1_MouseClick); 
        }

        void label1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.ToString()); 
        }

        void UIDropDownList_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.ToString()); 
        }

        void UIDropDownList_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.HWnd == this.Handle)
            {
                if (m.Msg == (int)Core.Msgs.WM_ACTIVATE)
                {
                    if (m.WParam.ToInt32() == (int)Core.Msgs.WA_ACTIVE)
                        m.Result = new IntPtr(-1);
                    else
                        base.WndProc(ref m);
                }
                else if (m.Msg == (int)Core.Msgs.WM_MOUSEACTIVATE)
                {
                    m.Result = new IntPtr(-1);
                }
                else if (m.Msg == (int)Core.Msgs.WM_LBUTTONDBLCLK)
                {
                    m.Result = new IntPtr(-1);
                }
                else 
                    base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);
        }


        //Public oDataList As DataList
        //Private VScrollMax As Long
        //Private VScrollValue As Long
        //Private mTextWidth As Long
        //Private mWidth As Long
        //Private mHeight As Long
        //Private mvarMaxRow As Long
        //Private hDefaultBrush As Long
        //Private hBlueBrush As Long
        //Private hWhiteBrush As Long
        //Private hWhitePen As Long
        //Private LastRect As RECT
        //Private LastIndex As Long
        //Private mRowHeight As Long
        //Private TopIndex As Long
        //Private Pointer As Long '1到mvarMaxRow之间
        //Private tPoint As POINTS

        //Public Property Get SelectedIndex() As String
        //    SelectedIndex = TopIndex + Pointer - 2
        //End Property

        //Public Property Get MaxRow() As Long
        //    MaxRow = mvarMaxRow
        //End Property

        //Public Property Let MaxRow(ByVal vData As Long)
        //    mvarMaxRow = vData
        //End Property

        //Public Sub Find(ByVal Key As String)
        //    Dim i As Long

        //    '/*往下走
        //    For i = TopIndex + Pointer - 2 To oDataList.Count - 1
        //        If oDataList.Item(i).ItemName Like "*" & Key & "*" Then
        //            If VScrollMax > 0 Then '在当前范围内
        //                If i <= VScrollMax Then
        //                    VScroll1.Value = i
        //                ElseIf (i + MaxRow) > oDataList.Count Then '在最后并且填不满MaxRow
        //                    VScroll1.Value = VScroll1.Max
        //                Else
        //                    VScroll1.Value = i - MaxRow + 1
        //                End If
        //            End If
        //            Pointer = i - TopIndex + 2
        //            DrawSelected Pointer
        //            Exit Sub
        //        End If
        //    Next i
        //    '*/
        //    '/*往上走
        //    For i = 0 To TopIndex + Pointer - 2
        //        If oDataList.Item(i).ItemName Like "*" & Key & "*" Then
        //            If VScrollMax > 0 Then
        //                If i <= VScrollMax Then
        //                    VScroll1.Value = i
        //                Else
        //                    VScroll1.Value = i - MaxRow + 1
        //                End If
        //            End If
        //            Pointer = i - TopIndex + 2
        //            DrawSelected Pointer
        //            Exit Sub
        //        End If
        //    Next i
        //    '*/
        //End Sub

        //Public Sub MoveDown()
        //    If (TopIndex + Pointer - 1) < oDataList.Count Then
        //        If Pointer < mvarMaxRow Then
        //            Pointer = Pointer + 1
        //        Else
        //            VScroll1.Value = VScroll1.Value + 1
        //        End If
        //        DrawSelected Pointer
        //    End If
        //End Sub

        //Public Sub MoveUp()
        //    If TopIndex > 0 Then
        //        If Pointer > 1 Then
        //            Pointer = Pointer - 1
        //        Else
        //            If TopIndex > 1 Then
        //                VScroll1.Value = VScroll1.Value - 1
        //            End If
        //        End If
        //        DrawSelected Pointer
        //    End If
        //End Sub

        //Public Sub Resize(p As POINTS)
        //    Dim i As Long
        //    Dim s As String
        //    Dim w As Long

        //    tPoint = p
        //    If MaxRow < oDataList.Count Then
        //        VScrollMax = oDataList.Count - MaxRow
        //        VScroll1.Max = VScrollMax
        //        VScroll1.Visible = True
        //    Else
        //        MaxRow = oDataList.Count
        //    End If
        //    mHeight = (mRowHeight + 4) * MaxRow + 4
        //    mWidth = 10
        //    For i = 1 To oDataList.Count
        //        s = oDataList.Item(i - 1).ItemName
        //        w = TextWidth(s)
        //        If w > mWidth Then
        //            mWidth = w
        //        End If
        //    Next i
        //    If MaxRow < oDataList.Count Then
        //        mWidth = mWidth + 42 '16 + 16 + 8+2
        //        mTextWidth = mWidth - 18
        //    Else
        //        mWidth = mWidth + 30 '16 +   8+2
        //        mTextWidth = mWidth - 2
        //    End If
        //    SetWindowPos Me.hwnd, HWND_TOP, p.X, p.Y, mWidth, mHeight, SWP_NOACTIVATE
        //End Sub

        //Private Sub Form_Initialize()
        //    mvarMaxRow = 6
        //    mWidth = Me.ScaleWidth
        //    Pointer = 1
        //    TopIndex = 1
        //    VScrollValue = 0
        //End Sub

        //Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
        //    If KeyCode = vbKeyUp Then
        //        MoveUp
        //        KeyCode = 0
        //    ElseIf KeyCode = vbKeyDown Then
        //        MoveDown
        //        KeyCode = 0
        //    End If
        //End Sub

        //Private Sub Form_Load()
        //    CreateGdiObject
        //    mRowHeight = TextHeight("陈")
        //    Me.PictureClip1.Cols = 4
        //    Me.PictureClip1.Rows = 52
        //    Me.PictureClip1.Picture = LoadResPicture(101, 0)

        //    SubclassWindow VScroll1.hwnd, Me
        //    SubclassWindow hwnd, Me
        //End Sub

        //Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
        //    Dim i As Long

        //    i = GetSelected(CLng(Y))
        //    Pointer = i + 1
        //    DrawSelected Pointer
        //End Sub

        //Private Sub Form_Paint()
        //    DrawList
        //    DrawSelected Pointer
        //End Sub

        //Private Sub Form_Resize()
        //    Me.VScroll1.Move Me.ScaleWidth - VScroll1.Width - 1, Me.ScaleTop + 1, VScroll1.Width, Me.ScaleHeight - 2
        //End Sub

        //Private Sub Form_Unload(Cancel As Integer)
        //    DeleteGdiObject
        //    UnSubclassWindow VScroll1.hwnd, Me
        //    UnSubclassWindow hwnd, Me
        //End Sub

        //Private Function GetSelected(ByVal Y As Long) As Long
        //    Dim i As Long
        //    i = Int(Y / (4 + mRowHeight))
        //    GetSelected = i
        //End Function

        //Private Sub DrawSelected(ByVal Index As Long, Optional ByVal FullBackColor As Boolean = True)
        //    Dim tRect As RECT
        //    Dim oldPen As Long
        //    Dim s As String

        //    If LastRect.Top > 0 Then
        //        DrawFocusRect Me.hDC, LastRect
        //        If LastIndex >= 0 Then
        //            s = oDataList.Item(LastIndex).ItemName
        //            FillRect Me.hDC, LastRect, hWhiteBrush
        //            LastRect.Top = LastRect.Top + 2
        //            LastRect.Left = 16 + 5
        //            DrawText Me.hDC, s, -1, LastRect, DT_VCENTER Or DT_LEFT
        //        End If
        //    End If
        //    If Index > MaxRow Then Index = MaxRow

        //    s = oDataList.Item(TopIndex + Index - 2).ItemName
        //    tRect.Left = 16 + 2
        //    tRect.Top = (Index - 1) * (4 + mRowHeight) + 2
        //    tRect.bottom = (tRect.Top + mRowHeight + 4)
        //    tRect.Right = mTextWidth
        //    LastRect = tRect
        //    If FullBackColor Then
        //        LastIndex = TopIndex + Index - 2
        //        FillRect Me.hDC, tRect, hBlueBrush
        //        Me.ForeColor = RGB(255, 255, 255)
        //        tRect.Top = tRect.Top + 2
        //        tRect.Left = 16 + 5
        //        DrawText Me.hDC, s, -1, tRect, DT_VCENTER Or DT_LEFT
        //        Me.ForeColor = &H80000012
        //        tRect.Top = tRect.Top - 2
        //        tRect.Left = 16 + 2
        //    Else
        //        tRect.Top = tRect.Top + 2
        //        tRect.Left = 16 + 5
        //        DrawText Me.hDC, s, -1, tRect, DT_VCENTER Or DT_LEFT
        //        tRect.Top = tRect.Top - 2
        //        tRect.Left = 16 + 2
        //    End If
        //    DrawFocusRect Me.hDC, tRect
        //End Sub

        //Private Sub DrawList()
        //    Dim i As Long
        //    Dim j As Long
        //    Dim s As String
        //    Dim tRect As RECT

        //    Cls
        //    tRect.Top = 0
        //    tRect.Left = 0
        //    tRect.Right = Me.ScaleWidth
        //    tRect.bottom = Me.ScaleHeight
        //    Rectangle Me.hDC, tRect.Left, tRect.Top, tRect.Right, tRect.bottom

        //    tRect.Right = Me.ScaleWidth - 1
        //    tRect.bottom = Me.ScaleHeight - 1
        //    FrameRect Me.hDC, tRect, hDefaultBrush 'hWhiteBrush

        //    CurrentY = 4
        //    If Not oDataList Is Nothing Then
        //        For i = TopIndex To oDataList.Count
        //            j = j + 1
        //            If j <= MaxRow Then
        //                PaintPicture Me.PictureClip1.GraphicCell(4 * 49 - 1), 1, CurrentY - 2
        //                CurrentX = 16 + 5
        //                s = oDataList.Item(i - 1).ItemName
        //                Me.Print s
        //                CurrentY = CurrentY + 4
        //            Else
        //                Exit For
        //            End If
        //        Next i
        //    End If
        //    If LastRect.Left > 0 Then
        //        DrawFocusRect Me.hDC, LastRect
        //    End If
        //End Sub

        //Private Sub CreateGdiObject()
        //    Dim tDefaultBrush As LOGBRUSH
        //    With tDefaultBrush
        //        .lbStyle = BS_SOLID
        //        .lbColor = RGB(236, 233, 216) 'DefaultHdrColor
        //    End With
        //    If hDefaultBrush > 0 Then DeleteObject hDefaultBrush
        //    hDefaultBrush = CreateBrushIndirect(tDefaultBrush)

        //    With tDefaultBrush
        //        .lbStyle = BS_SOLID
        //        .lbColor = RGB(49, 106, 197)
        //    End With
        //    If hBlueBrush > 0 Then DeleteObject hBlueBrush
        //    hBlueBrush = CreateBrushIndirect(tDefaultBrush)

        //    With tDefaultBrush
        //        .lbStyle = BS_SOLID
        //        .lbColor = &HFFFFFF
        //    End With
        //    If hWhiteBrush > 0 Then DeleteObject hWhiteBrush
        //    hWhiteBrush = CreateBrushIndirect(tDefaultBrush)

        //    Dim tWhitePen As LOGPEN
        //    With tWhitePen
        //        .lopnColor = &HFFFFFF
        //        .lopnStyle = PS_SOLID
        //    End With
        //    If hWhitePen > 0 Then DeleteObject hWhitePen
        //    hWhitePen = CreatePenIndirect(tWhitePen)
        //End Sub

        //Private Sub DeleteGdiObject()
        //    DeleteObject hDefaultBrush
        //    DeleteObject hBlueBrush
        //    DeleteObject hWhiteBrush
        //    DeleteObject hWhitePen
        //End Sub

        //Private Function ISubclass_WindowProc(ByVal OldWindowProc As Long, ByVal hwnd As Long, ByVal iMsg As Long, ByVal wParam As Long, ByVal lParam As Long) As Long
        //    Dim po As POINTS

        //    If hwnd = Me.hwnd Then
        //        If iMsg = WM_ACTIVATE Then
        //            If wParam = WA_ACTIVE Then
        //                ISubclass_WindowProc = -1
        //            Else
        //                ISubclass_WindowProc = CallPreviousWindowProc(OldWindowProc, hwnd, iMsg, wParam, lParam)
        //            End If
        //        ElseIf iMsg = WM_MOUSEACTIVATE Then
        //            ISubclass_WindowProc = -1
        //        ElseIf iMsg = WM_LBUTTONDBLCLK Then
        //            ISubclass_WindowProc = -1
        //            oDataList.ReturnSelectIndex (TopIndex + Pointer - 2)
        //        Else
        //            ISubclass_WindowProc = CallPreviousWindowProc(OldWindowProc, hwnd, iMsg, wParam, lParam)
        //        End If
        //    ElseIf hwnd = VScroll1.hwnd Then
        //        If iMsg = WM_MOUSEACTIVATE Then
        //            ISubclass_WindowProc = -1
        //        Else
        //            ISubclass_WindowProc = CallPreviousWindowProc(OldWindowProc, hwnd, iMsg, wParam, lParam)
        //        End If
        //    Else
        //        ISubclass_WindowProc = CallPreviousWindowProc(OldWindowProc, hwnd, iMsg, wParam, lParam)
        //    End If
        //End Function

        //Private Sub VScrollChange()
        //    TopIndex = VScrollValue + 1
        //    If LastIndex < TopIndex Or LastIndex > TopIndex + MaxRow Then
        //        LastIndex = -1
        //    End If
        //    DrawList
        //End Sub

        //Private Sub VScroll1_Change()
        //    VScrollValue = VScroll1.Value
        //    VScrollChange
        //End Sub
    }
}
