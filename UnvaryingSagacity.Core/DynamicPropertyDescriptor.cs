using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace UnvaryingSagacity.CustomPropertyAttributes.DynamicPropertyDescriptor
{
    
    public class UniqueChecker
    {
        /// <summary>
        /// 这里如果使用完全限定类型名的编辑器,则编辑器类不能跨越UnvaryingSagacity.Core.Dll
        /// 一定要使用时,需要在自身的程序集中继承类:CustomAttribute,UniqueChecker,ItemStylePropertyDescription
        /// 请参考UnvaryingSagacity.SuitSchemePrinter.DynamicPropertyDescriptor.cs
        /// </summary>
        /// <param name="targe"></param>
        /// <param name="methodName"></param>
        public UniqueChecker(object targe, string methodName)
        {
            Targe = targe;
            MethodName = methodName;
        }

        public string MethodName { get; set; }

        public object Targe { get; set; }
    }

    public class ValidateChecker
    {
        /// <summary>
        /// 这里如果使用完全限定类型名的编辑器,则编辑器类不能跨越UnvaryingSagacity.Core.Dll
        /// 一定要使用时,需要在自身的程序集中继承类:CustomAttribute,UniqueChecker,ItemStylePropertyDescription
        /// 请参考UnvaryingSagacity.SuitSchemePrinter.DynamicPropertyDescriptor.cs
        /// </summary>
        /// <param name="targe"></param>
        /// <param name="methodName"></param>
        public ValidateChecker(object targe, string methodName)
        {
            Targe = targe;
            MethodName = methodName;
        }

        public string MethodName { get; set; }

        public object Targe { get; set; }
    }

    /// <summary>
    /// 检查将要改变的值的内容,并决定是保留还是修改它
    /// </summary>
    public class ValueChecker
    {
        /// <summary>
        /// 这里如果使用完全限定类型名的编辑器,则编辑器类不能跨越UnvaryingSagacity.Core.Dll
        /// 一定要使用时,需要在自身的程序集中继承类:CustomAttribute,UniqueChecker,ItemStylePropertyDescription
        /// 请参考UnvaryingSagacity.SuitSchemePrinter.DynamicPropertyDescriptor.cs
        /// </summary>
        /// <param name="targe"></param>
        /// <param name="methodName"></param>
        public ValueChecker(object targe, string methodName)
        {
            Targe = targe;
            MethodName = methodName;
        }

        public string MethodName { get; set; }

        public object Targe { get; set; }
    }

    /// <summary>
    /// 不需要显示的不要加进来
    /// 参考SuitSchemeItem.SetPropertyDescriptor
    /// </summary>
    public class CustomAttribute
    {
        /// <summary>
        /// 这里如果使用完全限定类型名的编辑器,则编辑器类不能跨越UnvaryingSagacity.Core.Dll
        /// 一定要使用时,需要在自身的程序集中继承类:CustomAttribute,UniqueChecker,ItemStylePropertyDescription
        /// 请参考UnvaryingSagacity.SuitSchemePrinter.DynamicPropertyDescriptor.cs
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="styleType"></param>
        public CustomAttribute(string name, object context, Type styleType)
        {
            this.Name = name;
            this.Context = context;
            this.StyleType = styleType;
            this.EditorTypeName = "";
        }

        public bool ReadOnly { get; set; }

        public UniqueChecker Checker { get; set; }

        public ValidateChecker ValidChecker { get; set; }

        public ValueChecker ValueChecker { get; set; }
        /// <summary>
        /// 编辑器完全限定的类型名
        /// </summary>
        public string EditorTypeName { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public object Context { get; set; }

        public Type StyleType { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return "Name:" + this.Name + ',' + "Context:" + this.Context.ToString();
        }
    }

    /// <summary>
    /// 参考SuitSchemeItem.SetPropertyDescriptor
    /// </summary>
    public class ItemStylePropertyDescription : PropertyDescriptor
    {
        private CustomAttribute _itemStyle;
        private Attribute[] _attribute;
        private AttributeCollection _attributeCollection;
        /// <summary>
        /// 这里如果使用完全限定类型名的编辑器,则编辑器类不能跨越UnvaryingSagacity.Core.Dll
        /// 一定要使用时,需要在自身的程序集中继承类:CustomAttribute,UniqueChecker,ItemStylePropertyDescription
        /// 请参考UnvaryingSagacity.SuitSchemePrinter.DynamicPropertyDescriptor.cs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="attributes"></param>
        public ItemStylePropertyDescription(CustomAttribute item, Attribute[] attributes)
            : base(item.Name, attributes)
        {
            _itemStyle = item;
            int i = base.AttributeArray.Length;
            _attribute = new Attribute[base.AttributeArray.Length + 6];
            base.AttributeArray.CopyTo(_attribute, 0);
            //Array.Resize<Attribute>(ref _attribute, i + 1);
            _attribute[i] = new PropertyOrderAttribute(_itemStyle.Order);
            i++;
            //Array.Resize<Attribute>(ref _attribute, i + 1);
            _attribute[i] = new PropertyUniqueAttribute(_itemStyle.Checker != null ? true : false, _itemStyle.Checker != null ? _itemStyle.Checker.MethodName : "");
            i++;
            _attribute[i] = new PropertyValidAttribute(_itemStyle.ValidChecker != null ? true : false, _itemStyle.ValidChecker != null ? _itemStyle.ValidChecker.MethodName : "");
            i++;
            _attribute[i] = new PropertyValueCheckerAttribute(_itemStyle.ValueChecker != null ? true : false, _itemStyle.ValueChecker != null ? _itemStyle.ValueChecker.MethodName : "");
            i++;
            //Array.Resize<Attribute>(ref _attribute, i + 1);
            _attribute[i] = new EditorAttribute(_itemStyle.EditorTypeName, typeof(System.Drawing.Design.UITypeEditor));
            i++;
            //Array.Resize<Attribute>(ref _attribute, i + 1);
            _attribute[i] = new ReadOnlyAttribute(_itemStyle.ReadOnly);
            _attributeCollection = new AttributeCollection(_attribute);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return this.GetType(); }
        }

        public override object GetValue(object component)
        {
            return _itemStyle.Context;
        }

        public override bool IsReadOnly
        {
            get { return _itemStyle.ReadOnly ; }
        }

        public override Type PropertyType
        {
            get { return _itemStyle.StyleType; }
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            _itemStyle.Context = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override string Description
        {
            get
            {
                return _itemStyle.Description;
            }
        }

        protected override Attribute[] AttributeArray
        {
            get
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return _attributeCollection;
            }
        }

    }
}
