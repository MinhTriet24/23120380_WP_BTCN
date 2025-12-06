using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI; 
using PaintApp.Core.Enums;
using System;

namespace PaintApp.Core.Helpers
{
    public class ToolToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ToolType currentTool && parameter is string buttonToolName)
            {
                if (Enum.TryParse<ToolType>(buttonToolName, out var buttonTool))
                {
                    if (currentTool == buttonTool)
                    {
                        return new SolidColorBrush(ColorHelper.FromArgb(255, 200, 230, 255));
                    }
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
