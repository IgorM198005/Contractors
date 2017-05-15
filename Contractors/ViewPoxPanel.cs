using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Contractors
{
    public sealed class ViewPoxPanel : Panel
    {
        public double MinByX { get; set; }

        public double MinByY { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size minRectangle = new Size
            {
                Width = Math.Ceiling(this.MinByX * availableSize.Width),

                Height = Math.Ceiling(this.MinByY * availableSize.Height)
            };

            this.Children[0].Measure(availableSize);

            if ((this.Children[0].DesiredSize.Height < minRectangle.Height) || (this.Children[0].DesiredSize.Height < minRectangle.Width))
            {
                return minRectangle; 
            }
            else
            {
                return this.Children[0].DesiredSize;
            }
        }
    }
}
