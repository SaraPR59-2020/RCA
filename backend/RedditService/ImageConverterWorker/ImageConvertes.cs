﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverterWorker
{
    public class ImageConvertes
    {
        public static Image ConvertImage(Image img)
        {
            return (Image)(new Bitmap(img, new Size(20, 20)));
        }
    }
}
