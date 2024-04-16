using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.Components
{
    public static class TextBoxHelper
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public static void SetWatermarkText(TextBox textBox, string watermarkText)
        {
            //SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);

            if (textBox.IsHandleCreated && watermarkText != null)
            {
                SendMessage(textBox.Handle, 0x1501, 1, watermarkText);
            }
        }


    }
}
