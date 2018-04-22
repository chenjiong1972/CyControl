using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;

namespace UnvaryingSagacity.Core
{
    public class TextBoxImeOnHalf:TextBox 
    {
        public TextBoxImeOnHalf()
        {
            base.ImeMode = ImeMode.On; 
        }

        protected override void Dispose(bool disposing)
        {
            base.ImeMode = ImeMode.Off; 
            base.Dispose(disposing);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            byte[] b = Encoding.Unicode.GetBytes(e.KeyChar.ToString());
            if (b.Length == 2)
            {
                if (b[1] == 255)
                {
                    b[0] = (byte)(b[0] + 32);
                    b[1] = 0;
                    char[] c = Encoding.Unicode.GetChars(b);
                    e.KeyChar = c[0];
                }
            }
            base.OnKeyPress(e);
        }
    }
}
