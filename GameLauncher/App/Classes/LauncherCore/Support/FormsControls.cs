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
        /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
        /// <param name="Control_Form">Name of the Control</param>
        /// <param name="Action_Refresh">Parameters to be set for this Control</param>
        /// <param name="Window_Name">Name of the Parent Form</param>
        static public void SafeInvoke(this Control Control_Form, Action Action_Refresh, Form Window_Name)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Window_Name.Name)) 
                {
                    if (!(Application.OpenForms[Window_Name.Name] != null ? Application.OpenForms[Window_Name.Name].Disposing : true) && !Application.OpenForms[Window_Name.Name].IsDisposed)
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
                LogToFileAddons.OpenLog("Safe Invoker", null, Error, null, true);
            }
        }
    }
}
