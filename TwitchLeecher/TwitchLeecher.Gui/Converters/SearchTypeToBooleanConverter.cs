using System;
using System.Globalization;
using Avalonia.Data.Converters;
using TwitchLeecher.Core.Enums;

namespace TwitchLeecher.Gui.Converters
{
    public class SearchTypeToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        private SearchType _lastValue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SearchType))
            {
                throw new ApplicationException("Value has to be of type '" + typeof(SearchType).FullName + "'!");
            }

            if (!(parameter is SearchType))
            {
                throw new ApplicationException("Parameter has to be of type '" + typeof(SearchType).FullName + "'!");
            }

            var valueEnum = (SearchType)value;
            var parameterEnum = (SearchType)parameter;

            var res =  valueEnum.Equals(parameterEnum);
            if (res)
            {
                _lastValue = valueEnum;
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new ApplicationException("Value has to be of type '" + typeof(bool).FullName + "'!");
            }

            if (!(parameter is SearchType))
            {
                throw new ApplicationException("Parameter has to be of type '" + typeof(SearchType).FullName + "'!");
            }

            if (value is bool b && b)
            {
                return parameter;
            }

            return null;
        }

        #endregion IValueConverter Members
    }
}