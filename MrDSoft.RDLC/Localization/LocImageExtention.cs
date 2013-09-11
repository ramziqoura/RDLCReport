using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GCESRecyclage.Core.MethodExtension;
using System.Windows.Controls;


namespace GCESRecyclage.Core.Localization
{
    [MarkupExtensionReturnType(typeof(ImageSource))]
    public class LocImageExtension : LocResourceExtension
    {
        public LocImageExtension() { }

        public LocImageExtension(string resourceKey) : base(resourceKey) { }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            ImageSource img = null;
            Image imgDefault = new Image();

            if (LocManager.ResourceManager != null)
            {
                System.Drawing.Bitmap bmp =
                    LocManager.ResourceManager.GetObject(ResourceKey) as System.Drawing.Bitmap;
                
                if (bmp != null)
                    img = GetImageSource(bmp);
                else
                    return imgDefault.GetImageSource("pack://application:,,,/GCESRecyclage.ResourceLibrary;component/Images/defaultSource.png");
            }
            return imgDefault.GetImageSource("pack://application:,,,/GCESRecyclage.ResourceLibrary;component/Images/defaultSource.png");

        }

        private ImageSource GetImageSource(System.Drawing.Bitmap bmp)
        {
            BitmapSource src =
                Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            return src;
        }
    }
}
