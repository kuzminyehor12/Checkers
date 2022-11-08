using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Checkers.Forms.Extensions
{
    public static class ByteConverter
    {
        public static byte[] ConvertToBytes(this Image img)
        {
            if (img is null)
            {
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, img);
                return ms.ToArray();
            }
        }

        public static byte[] ConvertToBytes(this Button btn)
        {
            if (btn is null)
            {
                return null;
            }
                
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, btn);
                return ms.ToArray();
            }
        }

        public static Image ConvertToImage(this byte[] buffer)
        {
            if (buffer is null)
            {
                return default;
            }
                
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                object obj = bf.Deserialize(ms);
                return obj as Image;
            }
        }
    }
}
