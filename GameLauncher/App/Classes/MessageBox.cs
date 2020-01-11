using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;

namespace MeTonaTOR
{
    public class MessageBox
    {
        public static TaskDialogResult Show(string text)
        {
            TaskDialog td = new TaskDialog
            {
                StandardButtons = TaskDialogStandardButtons.Ok,
                Caption = Environment.MachineName,
                InstructionText = "",
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        public static TaskDialogResult Show(string text, string caption, _MessageBoxButtons buttons, _MessageBoxIcon icon)
        {
            TaskDialog td = new TaskDialog
            {
                StandardButtons = (TaskDialogStandardButtons)buttons,
                Icon = (TaskDialogStandardIcon)icon,
                Caption = caption,
                InstructionText = ((TaskDialogStandardIcon)icon).ToString(),
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        public static TaskDialogResult Show(IWin32Window owner, string text, string caption, _MessageBoxButtons buttons, _MessageBoxIcon icon) {
            TaskDialog td = new TaskDialog {
                StandardButtons = (TaskDialogStandardButtons)buttons,
                Icon = (TaskDialogStandardIcon)icon,
                Caption = caption,
                InstructionText = ((TaskDialogStandardIcon)icon).ToString(),
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        public static TaskDialogResult Show(IWin32Window owner, string text, string caption, string instruction, _MessageBoxButtons buttons, _MessageBoxIcon icon) {
            TaskDialog td = new TaskDialog {
                StandardButtons = (TaskDialogStandardButtons)buttons,
                Icon = (TaskDialogStandardIcon)icon,
                Caption = caption,
                InstructionText = instruction,
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        public static TaskDialogResult Show(string text, string caption) {
            TaskDialog td = new TaskDialog {
                StandardButtons = TaskDialogStandardButtons.Ok,
                Icon = TaskDialogStandardIcon.None,
                Caption = caption,
                InstructionText = (TaskDialogStandardIcon.None).ToString(),
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        public static TaskDialogResult Show(IWin32Window owner, string text, string caption, _MessageBoxButtons buttons) {
            TaskDialog td = new TaskDialog {
                StandardButtons = (TaskDialogStandardButtons)buttons,
                Icon = TaskDialogStandardIcon.None,
                Caption = caption,
                InstructionText = (TaskDialogStandardIcon.None).ToString(),
                DetailsExpanded = true,
                Text = text,
            };

            return td.Show();
        }

        /*
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options);
        public static DialogResult Show(string text, string caption);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons);
        public static DialogResult Show(IWin32Window owner, string text, string caption);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword);
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options);
        public static DialogResult Show(IWin32Window owner, string text);
        */

        public enum _MessageBoxButtons
        {
            OK = TaskDialogStandardButtons.Ok,
            OKCancel = TaskDialogStandardButtons.Ok | TaskDialogStandardButtons.Cancel,
            AbortRetryIgnore = TaskDialogStandardButtons.Retry | TaskDialogStandardButtons.Ok | TaskDialogStandardButtons.Cancel,
            YesNoCancel = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel,
            YesNo = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No,
            RetryCancel = TaskDialogStandardButtons.Retry | TaskDialogStandardButtons.Cancel
        }

        public enum _MessageBoxIcon
        {
            None = TaskDialogStandardIcon.None,
            Hand = TaskDialogStandardIcon.Error,
            Stop = TaskDialogStandardIcon.Error,
            Error = TaskDialogStandardIcon.Error,
            Question = TaskDialogStandardIcon.Shield,
            Exclamation = TaskDialogStandardIcon.Warning,
            Warning = TaskDialogStandardIcon.Warning,
            Asterisk = TaskDialogStandardIcon.Information,
            Information = TaskDialogStandardIcon.Information
        }
    }
}
