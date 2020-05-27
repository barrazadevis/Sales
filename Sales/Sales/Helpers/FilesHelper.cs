﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sales.Helpers
{
    using System.IO;

    public class FilesHelper
    {
        public static byte[] ReadyFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }

}
