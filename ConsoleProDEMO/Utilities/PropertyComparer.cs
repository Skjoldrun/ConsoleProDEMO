using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ConsoleProDEMO.Utilities
{
    public class PropertyComparer<T> : IComparer<T>
    {
        private readonly IComparer _comparer;
        private PropertyDescriptor _propertyDescriptor;
        private int _reverse;

        /// <summary>
        /// Initializes the Comparer class for properties.
        /// </summary>
        /// <param name="property">Protperty to be compared</param>
        /// <param name="direction">Sortdirection</param>
        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            _propertyDescriptor = property;
            Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            _comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        /// <summary>
        /// Compares the first value with the second one to apply the sort order for the prior selected protperty to be sorted by.
        /// </summary>
        /// <param name="firstValue">first value to compare</param>
        /// <param name="secondValue">second value to compare</param>
        public int Compare(T firstValue, T secondValue)
        {
            return _reverse * _comparer.Compare(_propertyDescriptor.GetValue(firstValue), _propertyDescriptor.GetValue(secondValue));
        }

        #endregion IComparer<T> Members

        private void SetPropertyDescriptor(PropertyDescriptor descriptor)
        {
            _propertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction)
        {
            _reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            SetPropertyDescriptor(descriptor);
            SetListSortDirection(direction);
        }
    }
}