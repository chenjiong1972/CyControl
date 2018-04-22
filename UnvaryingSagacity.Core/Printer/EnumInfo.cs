using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.Core.Printer
{
    public enum PrinterLogTextLocation
    {
        ���Ϻ��,
        ���Ϻ��,
        ���º��,
        ���º��,
        ��������,
        ��������,
        ���к��,
        ���к��,
        ��������,
        ��������,
        ��������,
        ��������,
    }

    public enum PrinterLogPictureLocation
    {
        ��������,
        ���Ͻ�,
        ���Ͻ�,
        ���½�,
        ���½�,
        ҳĩ��,
    }

    public enum PrinterLogTextBorder
    {
        �ޱ���,
        �����,
        �ϱ���,
        �±���,
        �ұ���,
        ȫ������,
    }

    public enum PrinterSelectPage
    {
        ��ӡ����ҳ,
        ��ӡ����ҳ,
        ��ӡż��ҳ,
        ��ӡ����ҳ��ż��ҳ,
        ��ӡż��ҳ������ҳ,
    }

    public enum PrinterCellBehave
    {
        ��ͨ,
        �����,
        ����߱���,
        ���ҷ���,
        ͼ��,
        ��д���,
    }

    public enum PrinterBodyStyle
    {
        ��������,
        �����и�,
        �Զ�,
    }

    public enum PrinterUnit
    {
        /// <summary>
        /// Ĭ�ϵ�λ��0.01 Ӣ�磩
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
        ���ֲ�����,
    }

    public enum PrinterBorderStyle
    {
        �ޱ߿�,
        ʵ�߱߿�,
        ���߱߿�,
        ���߱߿�,
        �̵��߱߿�,
        �̵���߱߿�,
        ˫ʵ�߱߿�,
    }

    public enum PageSetupKey
    {
        ePage_����������ҳü�ı߿�,
        ePage_����������ҳ�ŵı߿�,
        ePage_���������ñ�ͷ�ı߿�,
        ePage_���������ñ�β�ı߿�,
        ePage_����������������ı߿�,
        ePage_���������ø�����ı߿�,
        ePage_����߿�Ϊ˫��,
        ePage_���������ñ�־�ı�������,
        ePage_������������̶�����,
        ePage_ͼ���ӡ���,
        ePage_ͼ���ӡ�߶�,
        ePage_������зָ��ߵ���ɫ,
        ePage_�Զ�����ģʽ,
        ePage_���Ҫ��ǧ��λ,
        ePage_ϸ�߿��,
        ePage_PageHeaderBorderColor,     //ҳü�߿���ɫ
        ePage_MainTitleBorderColor,     //������߿���ɫ
        ePage_SubTitleBorderColor,     //������߿���ɫ
        ePage_HeaderBorderColor,     //��ͷ�߿���ɫ
        ePage_BodyBorderColor,     //���ı߿���ɫ
        ePage_TailBorderColor,     //��β�߿���ɫ
        ePage_PageFooterBorderColor,     //ҳ�ű߿���ɫ---------------------
        /// <summary>
        /// С�ڻ����0ʱ������,���п����
        /// </summary>
        ePage_����ߵ�����ַ���,  
        //----------------------22һ��Ҫͬ������class PageSetup�еĳ���[PARAMCOUNT]
    }

    internal enum AttachRowType
    {
        ҳü,
        ������,
        ������,
        ��ͷ,
        ��β,
        ҳ��,
        ����,
        ����,
    }

    internal enum FixRowPos
    {//-1:bottom;0:All;1:Top
        �׹̶��� = -1,
        ȫ���̶��� = 0,
        ���̶��� = 1,
    }

    internal enum FixColPos
    {//-1:Left;0:All;1:Right
        ��̶��� = -1,
        ȫ���̶��� = 0,
        �ҹ̶��� = 1,
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
