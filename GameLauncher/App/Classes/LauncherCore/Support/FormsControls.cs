using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    static class FormsControls
    {
        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <remarks>Inoke Method: MethodInvoker</remarks>
        /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        static public void SafeInvoke(this Control Control_Form, MethodInvoker Action_Refresh)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Control_Form.Name))
                {
                    if (Control_Form.IsHandleCreated)
                    {
                        if (Control_Form.InvokeRequired)
                        {
                            Control_Form.Invoke(Action_Refresh);
                        }
                        else if (!Control_Form.IsDisposed)
                        {
                            Action_Refresh();
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker [MethodInvoker]", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <remarks>Inoke Method: Action</remarks>
        /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
        /// <typeparam name="T">Controls Value</typeparam>
        /// <param name="this">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        static public void SafeInvoke<T>(this T @this, Action<T> Action_Refresh) where T : Control
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(@this.Name))
                {
                    if (@this.IsHandleCreated)
                    {
                        if (@this.InvokeRequired)
                        {
                            @this.Invoke(Action_Refresh);
                        }
                        else if (!@this.IsDisposed)
                        {
                            Action_Refresh(@this);
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker [Action]", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <remarks>Inoke Method: MethodInvoker Asynchronous</remarks>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <returns>An System.IAsyncResult that represents the result of the System.Windows.Forms.Control.BeginInvoke(System.Delegate) operation.</returns>
        static public IAsyncResult SafeBeginInvokeMethodAsync(this Control Control_Form, MethodInvoker Action_Refresh)
        {
            return Control_Form.BeginInvoke((MethodInvoker)delegate { Control_Form.SafeInvoke(Action_Refresh); });
        }
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <remarks>Inoke Method: MethodInvoker</remarks>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <returns>The return value from System.IAsyncResult that represents the result of the System.Windows.Forms.Control.BeginInvoke(System.Delegate) operation
        /// has no return value</returns>
        static public void SafeBeginInvokeMethod(this Control Control_Form, MethodInvoker Action_Refresh)
        {
            try
            {
                Control_Form.BeginInvoke((MethodInvoker)delegate { Control_Form.SafeInvoke(Action_Refresh); });
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Begin Invoke [MethodInvoker]", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <remarks>Inoke Method: Action</remarks>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <returns>An System.IAsyncResult that represents the result of the System.Windows.Forms.Control.BeginInvoke(System.Delegate) operation.</returns>
        static public IAsyncResult SafeBeginInvokeActionAsync<T>(this T @this, Action<T> Action_Refresh) where T : Control
        {
            return @this.BeginInvoke((Action)delegate { @this.SafeInvoke(Action_Refresh); });
        }
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <remarks>Inoke Method: Action</remarks>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <returns>The return value from System.IAsyncResult that represents the result of the System.Windows.Forms.Control.BeginInvoke(System.Delegate) operation
        /// has no return value</returns>
        static public void SafeBeginInvokeAction<T>(this T @this, Action<T> Action_Refresh) where T : Control
        {
            try
            {
                @this.BeginInvoke((Action)delegate { @this.SafeInvoke(Action_Refresh); });
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Begin Invoke [Action]", null, Error, null, true);
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Retrieves the return value of the asynchronous operation represented by the System.IAsyncResult passed.
        /// </summary>
        /// <typeparam name="T">Controls Value</typeparam>
        /// <param name="this">Name of the Control</param>
        /// <param name="Invoke_Result">The System.IAsyncResult that represents a specific invoke asynchronous operation, 
        /// returned when calling System.Windows.Forms.Control.BeginInvoke(System.Delegate).</param>
        /// <returns>The System.Object generated by the asynchronous operation.</returns>
        static public void SafeEndInvoke<T>(this T @this, IAsyncResult Invoke_Result) where T : Control
        {
            try
            {
                @this.EndInvoke(Invoke_Result);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker [End Invoke (Singleton)]", null, Error, null, true);
            }
            finally 
            {
                GC.Collect();
            }
        }
    }
}
