using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;

namespace Automation
{
    
    public class ScreenCapture
    {
        private Bitmap _screenBitmap;
        private Graphics _screenGraphics;
        private IntPtr _hwnd;

        public ScreenCapture()
        {
            
        }

        public void TakeWindowSnapshot(int hwnd,string path)
        {
            this._hwnd = (IntPtr)hwnd;
            DllImport.Rect rect = new DllImport.Rect();
            DllImport.GetWindowRect(this._hwnd, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            this._screenBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            this._screenGraphics = Graphics.FromImage(this._screenBitmap);
            this._screenGraphics.SmoothingMode = SmoothingMode.None;

            //DllImport.ShowWindow(this._hwnd, ShowWindowEnum.Maximize);
            IntPtr hdc = this._screenGraphics.GetHdc();
            DllImport.PrintWindow(this._hwnd, hdc, 0u); //dll
            this._screenGraphics.ReleaseHdc(hdc);
            this._screenGraphics.Flush();
            this._screenBitmap.Save(path, ImageFormat.Jpeg);
            this._screenGraphics.Dispose();
            this._screenGraphics = null;
            this._screenBitmap.Dispose();
            this._screenBitmap = null;
        }

        public void TakeWindowSnapshotWithChildWindow(int parentHwnd, int childHwnd, string path)
        {
            IntPtr pareHwnd = (IntPtr)parentHwnd;
            DllImport.Rect pareRect = new DllImport.Rect();
            DllImport.GetWindowRect(pareHwnd, ref pareRect);
            Bitmap pareBM = TakeWindowSnapshot(pareHwnd, pareRect);

            IntPtr chHwnd = (IntPtr)childHwnd;
            DllImport.Rect chRect = new DllImport.Rect();
            DllImport.GetWindowRect(chHwnd, ref chRect);
            Bitmap chBM = TakeWindowSnapshot(chHwnd, chRect);

            Bitmap bitMap = new Bitmap(pareBM.Width, pareBM.Height);

            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.DrawImage(pareBM, 0, 0);
            graphics.DrawImage(chBM, chRect.left - pareRect.left, chRect.top - pareRect.top);

            string addText = "HP " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            AddWaterMarkString(graphics, addText);

            bitMap.Save(path, ImageFormat.Jpeg);
        }

        /// <summary>
        /// Add water mark text to image
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="addText"></param>
        public void AddWaterMarkString(Graphics graphics, string addText)
        {
            Font f = new Font("Calibri", 12);
            Brush b = new SolidBrush(Color.AliceBlue);

            float x = graphics.VisibleClipBounds.Width - addText.Length * 8;

            graphics.DrawString(addText, f, b, x, 20);
        }

        private Bitmap TakeWindowSnapshot(IntPtr hwnd, DllImport.Rect rect)
        {
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            Bitmap bitMap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.None;

            IntPtr hdc = graphics.GetHdc();
            DllImport.PrintWindow(hwnd, hdc, 0u); //dll
            graphics.ReleaseHdc(hdc);
            graphics.Flush();
            graphics.Dispose();

            return bitMap;
        }

        /*
        private void ExecuteWindowsCommand()
        {
            string command = @"tscon " + Process.GetCurrentProcess().SessionId.ToString() + " /dest:console";    


            ProcessStartInfo procStartInfo = new ProcessStartInfo();

            //Sets the FileName property of myProcessInfo to %SystemRoot%\System32\cmd.exe where %SystemRoot% is a system variable which is expanded using Environment.ExpandEnvironmentVariables
            procStartInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%") + @"\System32\cmd.exe"; 

            procStartInfo.UseShellExecute = false;
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.CreateNoWindow = true;

            //set verb
            procStartInfo.Verb = "runas";

            //Sets the arguments
            procStartInfo.Arguments = @"/env /user:" + "Administrator" + " cmd" + command;

            //The process should start with elevated permissions

            Process process = new Process();
            process.StartInfo = procStartInfo;
            process.Start();
            Thread.Sleep(1000);
            process.Close();
                
        }
        */
        

    }
}
