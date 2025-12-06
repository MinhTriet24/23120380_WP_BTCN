using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace PaintApp.Core.Helpers
{
    public class StringToImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var path = value as string;

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                Uri imageUri;
                if (path.StartsWith("ms-appx:") || path.StartsWith("http"))
                {
                    imageUri = new Uri(path);
                }
                else
                {
                    imageUri = new Uri("ms-appx:///" + path);
                }

                return new BitmapImage(imageUri);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
