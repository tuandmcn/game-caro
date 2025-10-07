using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Caro
{
    class TransparentRichTextBox : RichTextBox
    {
        public TransparentRichTextBox()
        {
            // Bật chế độ double-buffering để giảm nhấp nháy khi vẽ lại
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        // Ghi đè phương thức CreateParams để thêm kiểu trong suốt
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Thêm WS_EX_TRANSPARENT style
                // Điều này báo cho Windows rằng control này nên trong suốt
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        // Ghi đè phương thức OnPaintBackground để không vẽ nền
        // Điều này ngăn control tự vẽ màu nền của nó
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Không làm gì cả để giữ nền trong suốt
        }
    }
}
