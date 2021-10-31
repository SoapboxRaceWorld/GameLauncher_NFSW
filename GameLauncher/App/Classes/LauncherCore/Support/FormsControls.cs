using GameLauncher.App.Classes.InsiderKit;
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
        /// <returns>The delegate being invoked has no return value.</returns>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <param name="Window_Name">Name of the Parent Form</param>
        static public void SafeInvokeAction(this Control Control_Form, Action Action_Refresh, Form Window_Name)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Window_Name.Name))
                {
                    if (!(Application.OpenForms[Window_Name.Name] != null ? Application.OpenForms[Window_Name.Name].Disposing : true) && !Application.OpenForms[Window_Name.Name].IsDisposed)
                    {
                        if (!Control_Form.IsDisposed || (Control_Form.IsHandleCreated && Control_Form.FindForm().IsHandleCreated))
                        {
                            if (!Control_Form.Disposing)
                            {
                                if (Control_Form.InvokeRequired)
                                {
                                    Control_Form.Invoke(Action_Refresh);
                                }
                                else
                                {
                                    Action_Refresh();
                                }
                            }
                            else
                            {
                                Log.Function("SafeInvokeAction".ToUpper() + "Control: " + Control_Form.Name + " is being Disposed");
                            }
                        }
                        else if (!Application.OpenForms[Window_Name.Name].IsDisposed)
                        {
                            Window_Name.Controls.Add(Control_Form);
                            SafeInvokeAction(Control_Form, Action_Refresh);
                            Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form.Name + " was added to the Form: " + Window_Name.Name);
                        }
                        else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                        {
                            Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form + " <- Handle hasn't been Created or has been Disposed | Action: " + Action_Refresh + " Form: " + Window_Name);
                        }
                    }
                }
                else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form + " Action: " + Action_Refresh + " Form: " + Window_Name + " <- Is Null");
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker Action", null, Error, null, true);
            }
        }
        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <returns>The delegate being invoked has no return value.</returns>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        static public void SafeInvokeAction(this Control Control_Form, Action Action_Refresh)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Control_Form.Name))
                {
                    if (!Control_Form.IsDisposed || (Control_Form.IsHandleCreated && Control_Form.FindForm().IsHandleCreated))
                    {
                        if (Control_Form.InvokeRequired)
                        {
                            Control_Form.Invoke(Action_Refresh);
                        }
                        else
                        {
                            Action_Refresh();
                        }
                    }
                    else if (!Control_Form.FindForm().IsDisposed)
                    {
                        Control_Form.FindForm().Controls.Add(Control_Form);
                        SafeInvokeAction(Control_Form, Action_Refresh);
                        Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form.Name + " was added to the Form: " + Control_Form.FindForm().Name);
                    }
                    else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                    {
                        Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form + " <- Handle hasn't been Created or has been Disposed | Action: " + Action_Refresh);
                    }
                }
                else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + Control_Form + " <- Is Null | Action: " + Action_Refresh);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker Action [Control Only]", null, Error, null, true);
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
        static public void SafeInvokeAction<T>(this T @this, Action<T> Action_Refresh) where T : Control
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(@this.Name))
                {
                    if (!@this.IsDisposed || (@this.IsHandleCreated && @this.FindForm().IsHandleCreated))
                    {
                        if (@this.InvokeRequired)
                        {
                            @this.Invoke(Action_Refresh);
                        }
                        else
                        {
                            Action_Refresh(@this);
                        }
                    }
                    else if (!@this.FindForm().IsDisposed)
                    {
                        @this.FindForm().Controls.Add(@this);
                        SafeInvokeAction(@this, Action_Refresh);
                        Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + @this.Name + " was added to the Form: " + @this.FindForm().Name);
                    }
                    else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                    {
                        Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + @this + " <- Handle hasn't been Created or has been Disposed | Action: " + Action_Refresh);
                    }
                }
                else if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Log.Function("SafeInvokeAction: ".ToUpper() + "Control: " + @this + " <- Is Null | Action: " + Action_Refresh);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker Action [Control Only]", null, Error, null, true);
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
            return @this.BeginInvoke((Action)delegate { @this.SafeInvokeAction(Action_Refresh); });
        }
        /// <summary>
        /// Retrieves the return value of the asynchronous operation represented by the System.IAsyncResult passed.
        /// </summary>
        /// <typeparam name="T">Controls Value</typeparam>
        /// <param name="this">Name of the Control</param>
        /// <param name="Invoke_Result">The System.IAsyncResult that represents a specific invoke asynchronous operation, 
        /// returned when calling System.Windows.Forms.Control.BeginInvoke(System.Delegate).</param>
        /// <returns>The System.Object generated by the asynchronous operation.</returns>
        static public void SafeEndInvokeAsyncCatch<T>(this T @this, IAsyncResult Invoke_Result) where T : Control
        {
            try
            {
                @this.EndInvoke(Invoke_Result);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Safe Invoker [End Invoke (Singleton)]", null, Error, null, true);
            }
        }
    }
}
