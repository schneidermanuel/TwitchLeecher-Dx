using System.Globalization;
using Avalonia.Data.Converters;
using TwitchLeecher.Core.Enums;

namespace TwitchLeecher.Gui.Converters;

public class VideoTypeToBooleanConverter : IValueConverter
{
    private VideoType _lastValue = VideoType.Broadcast;

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is VideoType))
        {
            throw new ApplicationException("Value has to be of type '" + typeof(VideoType).FullName + "'!");
        }

        if (!(parameter is VideoType))
        {
            throw new ApplicationException("Parameter has to be of type '" + typeof(VideoType).FullName + "'!");
        }

        VideoType valueEnum = (VideoType)value;
        VideoType parameterEnum = (VideoType)parameter;

        if (valueEnum.Equals(parameterEnum))
        {
            _lastValue = parameterEnum;
            return true;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool))
        {
            throw new ApplicationException("Value has to be of type '" + typeof(bool).FullName + "'!");
        }

        if (!(parameter is VideoType))
        {
            throw new ApplicationException("Parameter has to be of type '" + typeof(VideoType).FullName + "'!");
        }

        if ((bool)value)
        {
            _lastValue = (VideoType)parameter;
            return parameter;
        }

        return _lastValue;
    }

    #endregion IValueConverter Members
}