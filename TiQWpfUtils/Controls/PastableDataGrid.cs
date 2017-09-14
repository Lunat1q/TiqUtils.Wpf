using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiQWpfUtils.Helpers;

namespace TiQWpfUtils.Controls
{
    public class PastableDataGrid : DataGrid
    {
        public event ExecutedRoutedEventHandler ExecutePasteEvent;
        public event CanExecuteRoutedEventHandler CanExecutePasteEvent;

        // ******************************************************************
        static PastableDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(PastableDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    OnExecutedPasteInternal,
                    OnCanExecutePasteInternal));
        }

        // ******************************************************************

        #region Clipboard Paste

        // ******************************************************************
        private static void OnCanExecutePasteInternal(object target, CanExecuteRoutedEventArgs args)
        {
            ((PastableDataGrid) target).OnCanExecutePaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command query its state.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        protected virtual void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            if (CanExecutePasteEvent != null)
            {
                CanExecutePasteEvent(target, args);
                if (args.Handled)
                {
                    return;
                }
            }

            args.CanExecute = true;
            args.Handled = true;
        }

        // ******************************************************************
        private static void OnExecutedPasteInternal(object target, ExecutedRoutedEventArgs args)
        {
            ((PastableDataGrid) target).OnExecutedPaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command is executed.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        protected virtual void OnExecutedPaste(object target, ExecutedRoutedEventArgs args)
        {
            if (ExecutePasteEvent != null)
            {
                ExecutePasteEvent(target, args);
                if (args.Handled)
                {
                    return;
                }
            }

            // parse the clipboard data            
            var rowData = ClipboardHelper.ParseClipboardData();

            var hasAddedNewRow = false;

            // call OnPastingCellClipboardContent for each cell
            var minRowIndex = Items.IndexOf(CurrentItem);
            var maxRowIndex = Items.Count - 1;
            var minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow)
                ? Columns.IndexOf(CurrentColumn)
                : 0;
            var maxColumnDisplayIndex = Columns.Count - 1;
            var rowDataIndex = 0;
            for (var i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {
                if (i < Items.Count)
                {
                    CurrentItem = Items[i];

                    BeginEditCommand.Execute(null, this);

                    var columnDataIndex = 0;
                    for (var j = minColumnDisplayIndex;
                        j <= maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length;
                        j++, columnDataIndex++)
                    {
                        var column = ColumnFromDisplayIndex(j);
                        column.OnPastingCellClipboardContent(Items[i], rowData[rowDataIndex][columnDataIndex]);

                        //column.OnPastingCellClipboardContent(
                    }

                    CommitEditCommand.Execute(this, this);
                    if (i == maxRowIndex)
                    {
                        maxRowIndex++;
                        hasAddedNewRow = true;
                    }
                }
            }

            // update selection
            if (hasAddedNewRow)
            {
                UnselectAll();
                UnselectAllCells();

                CurrentItem = Items[minRowIndex];

                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    SelectedItem = Items[minRowIndex];
                }
                else if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader ||
                         SelectionUnit == DataGridSelectionUnit.Cell)
                {
                    SelectedCells.Add(new DataGridCellInfo(Items[minRowIndex], Columns[minColumnDisplayIndex]));

                }
            }
        }

        // ******************************************************************
        /// <summary>
        ///     Whether the end-user can add new rows to the ItemsSource.
        /// </summary>
        public bool CanUserPasteToNewRows
        {
            get
            {
                var value = GetValue(CanUserPasteToNewRowsProperty);
                return value != null && (bool) value;
            }
            set => SetValue(CanUserPasteToNewRowsProperty, value);
        }

        // ******************************************************************
        /// <summary>
        ///     DependencyProperty for CanUserAddRows.
        /// </summary>
        public static readonly DependencyProperty CanUserPasteToNewRowsProperty =
            DependencyProperty.Register("CanUserPasteToNewRows",
                typeof(bool), typeof(PastableDataGrid),
                new FrameworkPropertyMetadata(true, null, null));

        // ******************************************************************

        #endregion Clipboard Paste

        private void SetGridToSupportManyEditEitherWhenValidationErrorExists()
        {
            Items.CurrentChanged += Items_CurrentChanged;


            //Type DatagridType = this.GetType().BaseType;
            //PropertyInfo HasCellValidationProperty = DatagridType.GetProperty("HasCellValidationError", BindingFlags.NonPublic | BindingFlags.Instance);
            //HasCellValidationProperty.
        }

        void Items_CurrentChanged(object sender, EventArgs e)
        {
            //this.Items[0].
            //throw new NotImplementedException();
        }

        // ******************************************************************
        private void SetGridWritable()
        {
            var datagridType = GetType().BaseType;
            if (datagridType != null)
            {
                var hasCellValidationProperty = datagridType.GetProperty("HasCellValidationError",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (hasCellValidationProperty != null)
                {
                    hasCellValidationProperty.SetValue(this, false, null);
                }
            }
        }

        // ******************************************************************
        public void SetGridWritableEx()
        {
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance;
            var memberInfo = GetType().BaseType;
            if (memberInfo != null)
            {
                var cellErrorInfo = memberInfo.GetProperty("HasCellValidationError", bindingFlags);
                var rowErrorInfo = memberInfo.GetProperty("HasRowValidationError", bindingFlags);
                if (cellErrorInfo != null) cellErrorInfo.SetValue(this, false, null);
                if (rowErrorInfo != null) rowErrorInfo.SetValue(this, false, null);
            }
        }

        // ******************************************************************
    }
}
