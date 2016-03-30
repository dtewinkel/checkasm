using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CheckAsm
{
    sealed class IPictureDispHost:AxHost
    {
        /// <summary>
        /// Default Constructor, required by the framework.
        /// </summary>
        private IPictureDispHost() : base(string.Empty) { }
        /// <summary>
        /// Convert the image to an ipicturedisp.
        /// </summary>
        /// <param name="image">The image instance</param>
        /// <returns>The picture dispatch object.</returns>
        public new static object GetIPictureDispFromPicture(Image image)
        {
            return AxHost.GetIPictureDispFromPicture(image);
        }
        /// <summary>
        /// Convert the dispatch interface into an image object.
        /// </summary>
        /// <param name="picture">The picture interface</param>
        /// <returns>An image instance.</returns>
        public new static Image GetPictureFromIPicture(object picture)
        {
            return AxHost.GetPictureFromIPicture(picture);
        }

    }
}
