using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace UnvaryingSagacity.CustomPropertyAttributes
{
    /// <summary>
    /// Delegate that allows a class to change property attributes that
    /// define a property's behaviour in 
    /// a <see cref="System.Windows.Forms.PropertyGrid"/>.
    /// </summary>
    public delegate void PropertyAttributesProvider(PropertyAttributes propertyAttributes);


    /// <summary>
    /// Enhances the <see cref="PropertyDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// 所有的值都从 "PropertyAttributes" 得到
    /// 参考PropertiesDeluxeTypeConverter
    /// </remarks>
    public class PropertyDescriptorEx : PropertyDescriptor
    {
        private PropertyDescriptor basePropertyDescriptor;
        private PropertyAttributes propertyAttributes;

        public PropertyDescriptorEx(PropertyDescriptor basePropertyDescriptor, PropertyAttributes propertyAttributes)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
            this.propertyAttributes = propertyAttributes;
        }

        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return basePropertyDescriptor.ComponentType; }
        }

        public override string DisplayName
        {
            get
            {
                return propertyAttributes.DisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return propertyAttributes.Description;
            }
        }

        public override string Category
        {
            get
            {
                return propertyAttributes.Category;
            }
        }

        public override object GetValue(object component)
        {
            return this.basePropertyDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get
            {
                return propertyAttributes.IsReadOnly;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return propertyAttributes.IsBrowsable;
            }
        }

        public override string Name
        {
            get { return this.basePropertyDescriptor.Name; }
        }

        public override Type PropertyType
        {
            get { return this.basePropertyDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            this.basePropertyDescriptor.ResetValue(component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this.basePropertyDescriptor.ShouldSerializeValue(component);
        }

        public override void SetValue(object component, object value)
        {
            if (this.propertyAttributes.Unique)
            {
                MethodInfo  m;
                m = component.GetType().GetMethod(this.propertyAttributes.UniqueCheckMethodName);
                if (m != null)
                {
                    bool b = (bool)(m.Invoke(component, new object[1] { value }));
                    if (b)
                    {
                        return;
                    }
                }
            }
            if (this.propertyAttributes.Valid )
            {
                MethodInfo m;
                m = component.GetType().GetMethod(this.propertyAttributes.ValidCheckMethodName);
                if (m != null)
                {
                    bool b = (bool)(m.Invoke(component, new object[1] { value }));
                    if (!b)
                    {
                        return;
                    }
                }
            }
            if (this.propertyAttributes.ValueCheck )
            {
                MethodInfo m;
                m = component.GetType().GetMethod(this.propertyAttributes.ValueCheckMethodName );
                if (m != null)
                {
                    value = m.Invoke(component, new object[1] { value });
                }
            }
            this.basePropertyDescriptor.SetValue(component, value);
        }
    }

    /// <summary>
    /// 与PropertyGrid直接交互的对象,这里的属性将反映到PropertyGrid.
    /// </summary>
    public class PropertyAttributes : IComparable
    {
        private string _name;

        public PropertyAttributes(string name)
        {
            _name = name;
            this.Order = 0;
            this.Unique = false;
            this.Valid = false;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// 这个值一直为真,
        /// </summary>
        public bool IsBrowsable{get;set;}

        public bool IsReadOnly { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Order { get; set; }

        public bool Unique { get; set; }
        /// <summary>
        /// 检查值是否属性值是否唯一的方法名称
        /// </summary>
        public string UniqueCheckMethodName { get; set; }

        public bool Valid { get; set; }
        /// <summary>
        /// 检查值是否有效的方法名称
        /// </summary>
        public string ValidCheckMethodName { get; set; }

        public bool ValueCheck { get; set; }
        /// <summary>
        /// 检查值并决定是否要改变它的方法名称
        /// </summary>
        public string ValueCheckMethodName { get; set; }

        #region IComparable
        /// <summary>
        /// 由特性Order使用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            PropertyAttributes other = (PropertyAttributes)obj;
            if (this.Order == other.Order)
            {
                return string.Compare(this.DisplayName, other.DisplayName);
            }
            else
            {
                return (Order < other.Order) ? -1 : 1;
            }
        }

        #endregion
    }

    /// <summary>
    /// 对PropertyGrid.SelectObject进行自定义特性转换.任何要使用这些特性的类
    /// 都要在类的特性描述中加入:[TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    /// <seealso cref="DisplayNameAttribute"/>
    /// <seealso cref="GlobalizedPropertyAttribute"/>
    /// <seealso cref="GlobalizedTypeAttribute"/>
    /// <seealso cref="PropertyAttributesProviderAttribute"/>
    /// <seealso cref="PropertyOrderAttribute"/>
    /// <seealso cref="PropertyUniqueAttribute"/>
    /// </remarks>
    public class PropertiesDeluxeTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context,
            object value,
            Attribute[] attributes)
        {
            // Get the collection of properties
            PropertyDescriptorCollection baseProps =
                TypeDescriptor.GetProperties(value, attributes);
            PropertyDescriptorCollection deluxeProps =
                new PropertyDescriptorCollection(null);

            // For each property use a property descriptor of 
            // our own that has custom behaviour.
            ArrayList orderedPropertyAttributesList = new ArrayList();
            foreach (PropertyDescriptor oProp in baseProps)
            {
                PropertyAttributes propertyAttributes = GetPropertyAttributes(oProp, value);

                if (propertyAttributes.IsBrowsable)
                {
                    orderedPropertyAttributesList.Add(propertyAttributes);
                    deluxeProps.Add(
                        new PropertyDescriptorEx(oProp, propertyAttributes));
                }
            }
            orderedPropertyAttributesList.Sort();
            //
            // Build a string list of the ordered names
            //
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyAttributes propertyAttributes in orderedPropertyAttributesList)
            {
                propertyNames.Add(propertyAttributes.Name);
            }
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by.
            // (Sorting by passing a custom IComparer somehow doesn't work.
            //
            return deluxeProps.Sort((string[])propertyNames.ToArray(typeof(string)));
        }

        /// <summary>
        /// Get property attributes for given property descriptor and target object.
        /// </summary>
        private PropertyAttributes GetPropertyAttributes(
            PropertyDescriptor propertyDescriptor,
            object target
        )
        {
            PropertyAttributes propertyAttributes =
                new PropertyAttributes(propertyDescriptor.Name);
            string resourceBaseName = null;
            string displayName = null;
            string displayNameResourceName = null;
            string descriptionResourceName = null;
            string categoryResourceName = null;
            ResourceManager rm = null;

            //
            // First fill propertyAttributes with statically defined information.
            //

            foreach (Attribute attribute in propertyDescriptor.Attributes)
            {
                Type type = attribute.GetType();
                // If there's a DisplayNameAttribute defined, use that DisplayName.
                if (type.Equals(typeof(DisplayNameAttribute)))
                {
                    displayName = ((DisplayNameAttribute)attribute).DisplayName;
                }
                else if (type.Equals(typeof(PropertyOrderAttribute)))
                {
                    propertyAttributes.Order = ((PropertyOrderAttribute)attribute).Order;
                }
                else if (type.Equals(typeof(PropertyUniqueAttribute)))
                {
                    propertyAttributes.Unique = ((PropertyUniqueAttribute)attribute).Unique;
                    propertyAttributes.UniqueCheckMethodName = ((PropertyUniqueAttribute)attribute).methodName;
                }
                else if (type.Equals(typeof(PropertyValidAttribute)))
                {
                    propertyAttributes.Valid = ((PropertyValidAttribute)attribute).Valid;
                    propertyAttributes.ValidCheckMethodName = ((PropertyValidAttribute)attribute).methodName;
                }
                else if (type.Equals(typeof(PropertyValueCheckerAttribute)))
                {
                    propertyAttributes.ValueCheck = ((PropertyValueCheckerAttribute)attribute).ValueCheck;
                    propertyAttributes.ValueCheckMethodName = ((PropertyValueCheckerAttribute)attribute).methodName;
                }
                else if (type.Equals(typeof(ReadOnlyAttribute)))
                {
                    propertyAttributes.IsReadOnly = ((ReadOnlyAttribute)attribute).IsReadOnly;
                }
            }

            if (resourceBaseName == null)
            {
                if (resourceBaseName == null)
                {
                    resourceBaseName = propertyDescriptor.ComponentType.Namespace +
                        "." + propertyDescriptor.ComponentType.Name;
                }
            }

            // See if at least the culture neutral resources are there.
            // If not, disable globalization
            Assembly assembly = propertyDescriptor.ComponentType.Assembly;
            if (assembly.GetManifestResourceInfo(resourceBaseName + ".resources") == null)
            {
                rm = null;
            }
            else
            {
                rm = new ResourceManager(resourceBaseName, assembly);
                if (displayNameResourceName == null)
                {
                    displayNameResourceName =
                        propertyDescriptor.DisplayName + ".DisplayName";
                }
                if (descriptionResourceName == null)
                {
                    descriptionResourceName =
                        propertyDescriptor.DisplayName + ".Description";
                }
                if (categoryResourceName == null)
                {
                    categoryResourceName =
                        propertyDescriptor.Category + ".Category";
                }
            }

            // Display name.
            if (rm != null)
            {
                propertyAttributes.DisplayName = rm.GetString(displayNameResourceName);
            }
            else
            {
                propertyAttributes.DisplayName = null;
            }
            if (propertyAttributes.DisplayName == null)
            {
                propertyAttributes.DisplayName = displayName;
            }
            if (propertyAttributes.DisplayName == null)
            {
                propertyAttributes.DisplayName = propertyDescriptor.DisplayName;
            }

            // Description.
            if (rm != null)
            {
                propertyAttributes.Description = rm.GetString(descriptionResourceName);
            }
            else
            {
                propertyAttributes.Description = null;
            }
            if (propertyAttributes.Description == null)
            {
                propertyAttributes.Description = propertyDescriptor.Description;
            }

            // Category.
            if (rm != null)
            {
                propertyAttributes.Category = rm.GetString(categoryResourceName);
            }
            else
            {
                propertyAttributes.Category = null;
            }
            if (propertyAttributes.Category == null)
            {
                propertyAttributes.Category = propertyDescriptor.Category;
            }

            // IsBrowsable.
            propertyAttributes.IsBrowsable = propertyDescriptor.IsBrowsable;

            //
            // Now let target be able to override each of these property attributes
            // dynamically.
            //

            PropertyAttributesProviderAttribute propertyAttributesProviderAttribute =
                (PropertyAttributesProviderAttribute)
                    propertyDescriptor.Attributes[typeof(PropertyAttributesProviderAttribute)];
            if (propertyAttributesProviderAttribute != null)
            {
                MethodInfo propertyAttributesProvider =
                    propertyAttributesProviderAttribute.GetPropertyAttributesProvider(target);
                if (propertyAttributesProvider != null)
                {
                    propertyAttributesProvider.Invoke(target, new object[] { propertyAttributes });
                }
            }

            return propertyAttributes;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        //
        // Simple attribute to allow the order of a property to be specified
        //
        private int order;

        public PropertyOrderAttribute(int order)
        {
            this.order = order;
        }

        public int Order
        {
            get
            {
                return order;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyUniqueAttribute : Attribute
    {
        private bool unique;
        private string _method;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unique"></param>
        /// <param name="methodName">要进行检查的方法名称,定义该特性的对象中应该存在此方法</param>
        public PropertyUniqueAttribute(bool unique, string methodName)
        {
            this.unique = unique;
            _method = methodName;
        }

        public string methodName { get { return _method; } }

        public bool Unique
        {
            get
            {
                return unique;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyValidAttribute : Attribute
    {
        private bool _valid;
        private string _method;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unique"></param>
        /// <param name="methodName">要进行检查的方法名称,定义该特性的对象中应该存在此方法</param>
        public PropertyValidAttribute(bool valid, string methodName)
        {
            this._valid = valid;
            _method = methodName;
        }

        public string methodName { get { return _method; } }

        public bool Valid
        {
            get
            {
                return _valid;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyValueCheckerAttribute : Attribute
    {
        private bool _value;
        private string _method;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unique"></param>
        /// <param name="methodName">要进行检查的方法名称,定义该特性的对象中应该存在此方法</param>
        public PropertyValueCheckerAttribute(bool valueCheck, string methodName)
        {
            this._value = valueCheck;
            _method = methodName;
        }

        public string methodName { get { return _method; } }

        public bool ValueCheck
        {
            get
            {
                return _value;
            }
        }
    }
    /// <summary>
    /// Use this attribute on a property to set a property's
    /// <see cref="PropertyAttributesProvider"/> delegate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttributesProviderAttribute : Attribute
    {
        string propertyAttributesProviderName;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PropertyAttributesProviderAttribute(string propertyAttributesProviderName)
        {
            this.propertyAttributesProviderName = propertyAttributesProviderName;
        }

        /// <summary>
        /// Get the <see cref="PropertyAttributesProvider"/> specified by the
        /// <see cref="PropertyAttributesProviderAttribute"/> on given target object.
        /// </summary>
        public MethodInfo GetPropertyAttributesProvider(object target)
        {
            return target.GetType().GetMethod(propertyAttributesProviderName);
        }

        public string Name
        {
            get { return propertyAttributesProviderName; }
        }
    }
}

