using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShadowDemo
{
    /// <summary>
    ///     Dropshadow.
    ///     Add a shadow to a winform
    /// </summary>
    public class Dropshadow : Form
    {
        private Bitmap _shadowBitmap;
        private Color _shadowColor;
        private int _shadowH;
        private byte _shadowOpacity = 255;
        private int _shadowV;

        public Dropshadow(Form f)
        {

            Owner = f;
            ShadowColor = Color.Black;

            // default style
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            // bind event
            Owner.LocationChanged += UpdateLocation;
            Owner.FormClosing += (sender, eventArgs) => Close();
            Owner.VisibleChanged += (sender, eventArgs) =>
            {
                if (Owner != null)
                    Visible = Owner.Visible;
            };

            Owner.Activated += (sender, args) => Owner.BringToFront();
        }

        public Color ShadowColor {
            get { return _shadowColor; }
            set {
                _shadowColor = value;
                _shadowOpacity = _shadowColor.A;
            }
        }

        public Bitmap ShadowBitmap {
            get { return _shadowBitmap; }
            set {
                _shadowBitmap = value;
                SetBitmap(_shadowBitmap, ShadowOpacity);
            }
        }

        public byte ShadowOpacity {
            get { return _shadowOpacity; }
            set {
                _shadowOpacity = value;
                SetBitmap(ShadowBitmap, _shadowOpacity);
            }
        }

        public int ShadowH {
            get { return _shadowH; }
            set {
                _shadowH = value;
                RefreshShadow(false);
            }
        }

        /// <summary>
        ///     Offset X relate to Owner
        /// </summary>
        public int OffsetX {
            get { return ShadowH - (ShadowBlur + ShadowSpread); }
        }

        /// <summary>
        ///     Offset Y relate to Owner
        /// </summary>
        public int OffsetY {
            get { return ShadowV - (ShadowBlur + ShadowSpread); }
        }

        public new int Width {
            get { return Owner.Width + (ShadowSpread + ShadowBlur) * 2; }
        }

        public new int Height {
            get { return Owner.Height + (ShadowSpread + ShadowBlur) * 2; }
        }

        public int ShadowV {
            get { return _shadowV; }
            set {
                _shadowV = value;
                RefreshShadow(false);
            }
        }

        public int ShadowBlur { get; set; }
        public int ShadowSpread { get; set; }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }

        public static Bitmap DrawShadowBitmap(int width, int height, int borderRadius, int blur, int spread, Color color)
        {
            int ex = blur + spread;
            int w = width + ex * 2;
            int h = height + ex * 2;
            int solidW = width + spread * 2;
            int solidH = height + spread * 2;

            var bitmap = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bitmap);
            // fill background
            g.FillRectangle(new SolidBrush(color)
                , blur, blur, width + spread * 2 + 1, height + spread * 2 + 1);
            // +1 to fill the gap

            if (blur > 0)
            {
                // four dir gradiant
                {
                    // left
                    var brush = new LinearGradientBrush(new Point(0, 0), new Point(blur, 0), Color.Transparent, color);
                    // will thorw ArgumentException
                    // brush.WrapMode = WrapMode.Clamp; 


                    g.FillRectangle(brush, 0, blur, blur, solidH);
                    // up
                    brush.RotateTransform(90);
                    g.FillRectangle(brush, blur, 0, solidW, blur);

                    // right
                    // make sure parttern is currect
                    brush.ResetTransform();
                    brush.TranslateTransform(w % blur, h % blur);

                    brush.RotateTransform(180);
                    g.FillRectangle(brush, w - blur, blur, blur, solidH);
                    // down
                    brush.RotateTransform(90);
                    g.FillRectangle(brush, blur, h - blur, solidW, blur);
                }


                // four corner
                {
                    var gp = new GraphicsPath();
                    //gp.AddPie(0,0,blur*2,blur*2, 180, 90);
                    gp.AddEllipse(0, 0, blur * 2, blur * 2);


                    var pgb = new PathGradientBrush(gp);
                    pgb.CenterColor = color;
                    pgb.SurroundColors = new[] { Color.Transparent };
                    pgb.CenterPoint = new Point(blur, blur);

                    // lt
                    g.FillPie(pgb, 0, 0, blur * 2, blur * 2, 180, 90);
                    // rt
                    var matrix = new Matrix();
                    matrix.Translate(w - blur * 2, 0);

                    pgb.Transform = matrix;
                    //pgb.Transform.Translate(w-blur*2, 0);
                    g.FillPie(pgb, w - blur * 2, 0, blur * 2, blur * 2, 270, 90);
                    // rb
                    matrix.Translate(0, h - blur * 2);
                    pgb.Transform = matrix;
                    g.FillPie(pgb, w - blur * 2, h - blur * 2, blur * 2, blur * 2, 0, 90);
                    // lb
                    matrix.Reset();
                    matrix.Translate(0, h - blur * 2);
                    pgb.Transform = matrix;
                    g.FillPie(pgb, 0, h - blur * 2, blur * 2, blur * 2, 90, 90);
                }
            }

            //
            return bitmap;
        }

        public void UpdateLocation(Object sender = null, EventArgs eventArgs = null)
        {
            Point pos = Owner.Location;

            pos.Offset(OffsetX, OffsetY);
            Location = pos;
        }

        /// <summary>
        ///     Refresh shadow.
        /// </summary>
        /// <param name="redraw"> (optional) redraw the background bitmap. </param>
        public void RefreshShadow(bool redraw = true)
        {
            if (redraw)
            {
                //ShadowBitmap = DrawShadow();
                ShadowBitmap = DrawShadowBitmap(Owner.Width, Owner.Height, 0, ShadowBlur, ShadowSpread, ShadowColor);
            }

            //SetBitmap(ShadowBitmap, ShadowOpacity);
            UpdateLocation();

            // 设置显示区域
            //Region r = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width, Height, BorderRadius, BorderRadius));
            var r = new Region(new Rectangle(0, 0, Width, Height));
            Region or;
            if (Owner.Region == null)
                or = new Region(Owner.ClientRectangle);
            else
                or = Owner.Region.Clone();

            or.Translate(-OffsetX, -OffsetY);
            r.Exclude(or);
            Region = r;

            Owner.Refresh();
        }

        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity = 255)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                var size = new Win32.Size(bitmap.Width, bitmap.Height);
                var pointSource = new Win32.Point(0, 0);
                var topPos = new Win32.Point(Left, Top);
                var blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend,
                    Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }
    }


    // class that exposes needed win32 gdi functions.
    internal static class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        };

        public const Int32 ULW_COLORKEY = 0x00000001;
        public const Int32 ULW_ALPHA = 0x00000002;
        public const Int32 ULW_OPAQUE = 0x00000004;

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        ///     Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified
        ///     offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
        /// <param name="nIndex">
        ///     The zero-based offset to the value to be set. Valid values are in the range zero through the
        ///     number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the
        ///     following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC
        /// </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
            IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ARGB
        {
            public readonly byte Blue;
            public readonly byte Green;
            public readonly byte Red;
            public readonly byte Alpha;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }
    }
}