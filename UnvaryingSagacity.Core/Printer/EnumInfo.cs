using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.Core.Printer
{
    public enum PrinterLogTextLocation
    {
        左上横放,
        右上横放,
        左下横放,
        右下横放,
        左中竖放,
        右中竖放,
        上中横放,
        下中横放,
        左上竖放,
        左下竖放,
        右上竖放,
        右下竖放,
    }

    public enum PrinterLogPictureLocation
    {
        主标题下,
        左上角,
        右上角,
        左下角,
        右下角,
        页末中,
    }

    public enum PrinterLogTextBorder
    {
        无边线,
        左边线,
        上边线,
        下边线,
        右边线,
        全部边线,
    }

    public enum PrinterSelectPage
    {
        打印所有页,
        打印奇数页,
        打印偶数页,
        打印奇数页和偶数页,
        打印偶数页和奇数页,
    }

    public enum PrinterCellBehave
    {
        普通,
        金额线,
        金额线标题,
        货币符号,
        图像,
        大写金额,
    }

    public enum PrinterBodyStyle
    {
        正文行数,
        正文行高,
        自动,
    }

    public enum PrinterUnit
    {
        /// <summary>
        /// 默认单位（0.01 英寸）
        /// </summary>
        Display,
        Twips,
    }

    public enum PrinterTextAlign
    {
        CenterBottom,
        CenterMiddle,
        CenterTop,
        LeftBottom,
        LeftMiddle,
        LeftTop,
        RightBottom,
        RightMiddle,
        RightTop,
        均分并填满,
    }

    public enum PrinterBorderStyle
    {
        无边框,
        实线边框,
        短线边框,
        点线边框,
        短点线边框,
        短点点线边框,
        双实线边框,
    }

    public enum PageSetupKey
    {
        ePage_不允许设置页眉的边框,
        ePage_不允许设置页脚的边框,
        ePage_不允许设置表头的边框,
        ePage_不允许设置表尾的边框,
        ePage_不允许设置主标题的边框,
        ePage_不允许设置副标题的边框,
        ePage_网格边框为双线,
        ePage_不允许设置标志文本的内容,
        ePage_正文字体包括固定行列,
        ePage_图标打印宽度,
        ePage_图标打印高度,
        ePage_金额线中分隔线的颜色,
        ePage_自动缩放模式,
        ePage_金额要求千分位,
        ePage_细线宽度,
        ePage_PageHeaderBorderColor,     //页眉边框颜色
        ePage_MainTitleBorderColor,     //主标题边框颜色
        ePage_SubTitleBorderColor,     //副标题边框颜色
        ePage_HeaderBorderColor,     //表头边框颜色
        ePage_BodyBorderColor,     //正文边框颜色
        ePage_TailBorderColor,     //表尾边框颜色
        ePage_PageFooterBorderColor,     //页脚边框颜色---------------------
        /// <summary>
        /// 小于或等于0时不限制,由列宽决定
        /// </summary>
        ePage_金额线的最大字符数,  
        //----------------------22一定要同步更新class PageSetup中的常量[PARAMCOUNT]
    }

    internal enum AttachRowType
    {
        页眉,
        主标题,
        副标题,
        表头,
        表尾,
        页脚,
        表体,
        其他,
    }

    internal enum FixRowPos
    {//-1:bottom;0:All;1:Top
        底固定行 = -1,
        全部固定行 = 0,
        顶固定行 = 1,
    }

    internal enum FixColPos
    {//-1:Left;0:All;1:Right
        左固定列 = -1,
        全部固定列 = 0,
        右固定列 = 1,
    }

    internal enum PrinterCellPos
    {
        cellPos_OnlyOne,
        cellPos_LeftTop,
        cellPos_LeftMiddle,
        cellPos_LeftBottom,
        cellPos_RightTop,
        cellPos_RightMiddle,
        cellPos_RightBottom,
        cellPos_CenterTop,
        cellPos_CenterMiddle,
        cellPos_CenterBottom,
        cellPos_LeftTopBottom,
        cellPos_RightTopBottom,
        cellPos_CenterTopBottom,
        cellPos_TopLeftRight,
        cellPos_MiddleLeftRight,
        cellPos_BottomLeftRight,
    }
}
