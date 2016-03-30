using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Amberfish.Graph
{
    static class Extensions
    {
        /// <summary>
        /// Gets the name of the color if the color is known.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Name of the color if it is one of the predefined colors in the Colors class. Otherwise returns color.ToString()</returns>
        public static string GetName(this Color color)
        {
            var properties = typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var selected = properties.FirstOrDefault(p => p.GetValue(null).Equals(color));
            if (selected == null)
                return color.ToString();
            return selected.Name;
        }

        /// <summary>
        /// Finds the most suitable color to be used for text on the specified background
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        public static Color GetContrastColor(this Color backgroundColor)
        {
            if (Luma(backgroundColor) < 140)
            {
                return Colors.White;
            }
            return Colors.Black;
        }

        /// <summary>
        /// Gets the Luma coefficient as per Rec. 709
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static double Luma(Color color)
        {
            return 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
        }
    }
}
