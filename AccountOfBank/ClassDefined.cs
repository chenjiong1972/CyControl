using System;
using System.Collections.Generic;
using System.Text;

namespace UnvaryingSagacity.AccountOfBank
{
    enum UIState
    {
        None,
        Editor,
        View,
        ViewTotal,
    }
    enum InternalBaseObject
    {
        None,
        结算方式,
        账套,
        账户,
        银行,
    
    }

    class Term
    {
        int Year { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime ToDay { get; set; }

        public Term(int Year)
        {
            this.Year = Year;
            StartDate = new DateTime(Year, 1, 1);
            EndDate = new DateTime(Year, 12, 31);
            if (Year < DateTime.Today.Year)
            {
                ToDay = new DateTime(Year, 12, 31);
            }
            else
            {
                ToDay = DateTime.Today;
            }
        }
    }

    class SimlpeObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public virtual void CopyTo(SimlpeObject dst)
        {
            dst.Name = this.Name;
            dst.ID = this.ID;
            dst.Description = this.Description;
        }
    }

    /// <summary>
    /// 账套
    /// </summary>
    class Account : SimlpeObject
    {
        /// <summary>
        /// 满文件名
        /// </summary>
        public string FullPath { get; set; }
        public int StartYear { get; set; }
        public override void CopyTo(SimlpeObject dst)
        {
            base.CopyTo(dst);
            (dst as Account).FullPath = this.FullPath;
        }
    }

    /// <summary>
    /// 账套集合
    /// </summary>
    class Accounts : Core.ListDictionary<Account> { }

    /// <summary>
    /// 账号
    /// </summary>
    class ItemOfBank : SimlpeObject
    {
        /// <summary>
        /// 所属银行名称
        /// </summary>
        public string OfBankName { get; set; }

        public double StartBal { get; set; }

        public override void CopyTo(SimlpeObject dst)
        {
            base.CopyTo(dst);
            (dst as ItemOfBank).OfBankName = this.OfBankName;
            (dst as ItemOfBank).StartBal = this.StartBal;
        }
    }
    /// <summary>
    /// 账户集合
    /// </summary>
    class ItemOfBankCollection : Core.ListDictionary<ItemOfBank> { }

    class YearClosed
    {
        public int Year { get; set; }
        public string ItemOfBankID { get; set; }
        public double StartBal { get; set; }
        public double EndBal { get; set; }
        public bool Closed { get; set; }
        public DateTime CloseDate { get; set; }
    }

    class Users : Core.ListDictionary<UserAndRight.User> { }

}
