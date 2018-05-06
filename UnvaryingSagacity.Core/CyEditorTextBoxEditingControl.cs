using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    public class CyEditorTextBoxEditingControl:CyEditor ,IDataGridViewEditingControl 
    {
        protected int rowIndex;
        protected DataGridView dataGridView;
        protected bool valueChanged = false;
        Type validatingType;

        public CyEditorTextBoxEditingControl()
        {
            this.ValidatingType = typeof(string);
            this.Mode = DisplayMode.金额; 
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Console.WriteLine("CyEditorTextBoxEditingControl.ProcessDialogKey");
            return base.ProcessDialogKey(keyData);
        }

        public virtual Type ValidatingType
        {
            get
            {
                return this.validatingType;
            }
            set
            {
                this.validatingType = value;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Let the DataGridView know about the value change
            NotifyDataGridViewOfValueChange();
        }

        //  Notify DataGridView that the value has changed.
        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }

        #region IDataGridViewEditingControl Members

        //  Indicates the cursor that should be shown when the user hovers their
        //  mouse over this cell when the editing control is shown.
		public Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.No ;
            }
        }


        //  Returns or sets the parent DataGridView.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return this.dataGridView;
            }

            set
            {
                this.dataGridView = value;
            }
        }


        //  Sets/Gets the formatted value contents of this cell.
		public object EditingControlFormattedValue
        {
            set
            {
                this.Text = value.ToString();
                NotifyDataGridViewOfValueChange();
            }
			get 
			{
				return this.Text;
			}

        }

		//   Get the value of the editing control for formatting.
		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return this.Text;
		}

        //  Process input key and determine if the key should be used for the editing control
        //  or allowed to be processed by the grid. Handle cursor movement keys for the MaskedTextBox
        //  control; otherwise if the DataGridView doesn't want the input key then let the editing control handle it.
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            Console.WriteLine("EditingControlWantsInputKey:{0}", dataGridViewWantsInputKey);
            //switch (keyData & Keys.KeyCode)
            //{
            //    case Keys.Right:
            //        //
            //        // If the end of the selection is at the end of the string
            //        // let the DataGridView treat the key message
            //        //
            //        if (!(this.SelectionLength == 0
            //              && this.SelectionStart == this.ToString().Length))
            //        {
            //            return true;
            //        }
            //        break;

            //    case Keys.Left:
            //        //
            //        // If the end of the selection is at the begining of the
            //        // string or if the entire text is selected send this character 
            //        // to the dataGridView; else process the key event.
            //        //
            //        if (!(this.SelectionLength == 0
            //              && this.SelectionStart == 0))
            //        {
            //            return true;
            //        }
            //        break;

            //    case Keys.Home:
            //    case Keys.End:
            //        if (this.SelectionLength != this.ToString().Length)
            //        {
            //            return true;
            //        }
            //        break;

            //    case Keys.Prior:
            //    case Keys.Next:
            //        if (this.valueChanged)
            //        {
            //            return true;
            //        }
            //        break;

            //    case Keys.Delete:
            //        if (this.SelectionLength > 0 || this.SelectionStart < this.ToString().Length)
            //        {
            //            return true;
            //        }
            //        break;
            //}

            ////
            //// defer to the DataGridView and see if it wants it.
            ////
            return !dataGridViewWantsInputKey;
        }


        //  Prepare the editing control for edit.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            //if (selectAll)
            //{
            //    SelectAll();
            //}
            //else
            //{
            //    //
            //    // Do not select all the text, but position the caret at the 
            //    // end of the text.
            //    //
            //    this.SelectionStart = this.ToString().Length;
            //}
        }

        //  Indicates whether or not the parent DataGridView control should
        //  reposition the editing control every time value change is indicated.
        //  There is no need to do this for the MaskedTextBox.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }


        //  Indicates the row index of this cell.  This is often -1 for the
        //  template cell, but for other cells, might actually have a value
        //  greater than or equal to zero.
        public int EditingControlRowIndex
        {
            get
            {
                return this.rowIndex;
            }

            set
            {
                this.rowIndex = value;
            }
        }



        //  Make the MaskedTextBox control match the style and colors of
        //  the host DataGridView control and other editing controls 
        //  before showing the editing control.
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
            //this.TextAlign = translateAlignment(dataGridViewCellStyle.Alignment);
        }


        //  Gets or sets our flag indicating whether the value has changed.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }

            set
            {
                this.valueChanged = value;
            }
        }
    
		#endregion // IDataGridViewEditingControl.
		
        ///   Routine to translate between DataGridView
        ///   content alignments and text box horizontal alignments.
        private static HorizontalAlignment translateAlignment(DataGridViewContentAlignment align)
        {
            switch (align)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.BottomLeft:
                    return HorizontalAlignment.Left;

                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    return HorizontalAlignment.Center;

                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.BottomRight:
                    return HorizontalAlignment.Right;
            }

            throw new ArgumentException("Error: Invalid Content Alignment!");
        }


    }

    class CyEditorTextBoxCell : DataGridViewTextBoxCell
    {
        private Type validatingType;

        //=------------------------------------------------------------------=
        // MaskedTextBoxCell
        //=------------------------------------------------------------------=
        /// <summary>
        ///   Initializes a new instance of this class.  Fortunately, there's
        ///   not much to do here except make sure that our base class is 
        ///   also initialized properly.
        /// </summary>
        /// 

        public CyEditorTextBoxCell()
            : base()
        {
            this.validatingType = typeof(string);
        }

        ///   Whenever the user is to begin editing a cell of this type, the editing
        ///   control must be created, which in this column type's
        ///   case is a subclass of the MaskedTextBox control.
        /// 
        ///   This routine sets up all the properties and values
        ///   on this control before the editing begins.
        public override void InitializeEditingControl(int rowIndex,
                                                      object initialFormattedValue,
                                                      DataGridViewCellStyle dataGridViewCellStyle)
        {
            CyEditorTextBoxEditingControl mtbec;
            CyEditorTextBoxColumn mtbcol;
            DataGridViewColumn dgvc;

            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                                          dataGridViewCellStyle);

            mtbec = DataGridView.EditingControl as CyEditorTextBoxEditingControl;

            //
            // set up props that are specific to the MaskedTextBox
            //

            dgvc = this.OwningColumn;   // this.DataGridView.Columns[this.ColumnIndex];
            if (dgvc is CyEditorTextBoxColumn)
            {
                mtbcol = dgvc as CyEditorTextBoxColumn;

                
                if (this.ValidatingType == null)
                {
                    mtbec.ValidatingType = mtbcol.ValidatingType;
                }
                else
                {
                    mtbec.ValidatingType = this.ValidatingType;
                }
                if (this.Value != null)
                {
                    mtbec.Text = this.Value.ToString();
                }
                else
                {
                    mtbec.Text = "";
                }
            }
        }

        //  Returns the type of the control that will be used for editing
        //  cells of this type.  This control must be a valid Windows Forms
        //  control and must implement IDataGridViewEditingControl.
        public override Type EditType
        {
            get
            {
                return typeof(CyEditorTextBoxEditingControl);
            }
        }

        //  A Type object for the validating type.
        public virtual Type ValidatingType
        {
            get
            {
                return this.validatingType;
            }
            set
            {
                this.validatingType = value;
            }
        }

        //   Quick routine to convert from DataGridViewTriState to boolean.
        //   True goes to true while False and NotSet go to false.
        protected static bool BoolFromTri(DataGridViewTriState tri)
        {
            return (tri == DataGridViewTriState.True) ? true : false;
        }
    }

    public class CyEditorTextBoxColumn : DataGridViewColumn
    {
        private Type validatingType;

        //  Initializes a new instance of this class, making sure to pass
        //  to its base constructor an instance of a MaskedTextBoxCell 
        //  class to use as the basic template.
        public CyEditorTextBoxColumn()
            : base(new CyEditorTextBoxCell())
        {
        }

        //  Routine to convert from boolean to DataGridViewTriState.
        private static DataGridViewTriState TriBool(bool value)
        {
            return value ? DataGridViewTriState.True
                         : DataGridViewTriState.False;
        }


        //  The template cell that will be used for this column by default,
        //  unless a specific cell is set for a particular row.
        //
        //  A MaskedTextBoxCell cell which will serve as the template cell
        //  for this column.
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }

            set
            {
                //  Only cell types that derive from MaskedTextBoxCell are supported as the cell template.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(CyEditorTextBoxCell)))
                {
                    string s = "Cell type is not based upon the CyEditorTextBoxCell.";//CustomColumnMain.GetResourceManager().GetString("excNotMaskedTextBox");
                    throw new InvalidCastException(s);
                }

                base.CellTemplate = value;
            }
        }

        //  Indicates the type against any data entered in the MaskedTextBox
        //  should be validated.  The MaskedTextBox control will attempt to
        //  instantiate this type and assign the value from the contents of
        //  the text box.  An error will occur if it fails to assign to this
        //  type.
        //
        //  See the MaskedTextBox control documentation for more details.
        public virtual Type ValidatingType
        {
            get
            {
                return this.validatingType;
            }
            set
            {
                CyEditorTextBoxCell mtbc;
                DataGridViewCell dgvc;
                int rowCount;

                if (this.validatingType != value)
                {
                    this.validatingType = value;

                    //
                    // first, update the value on the template cell.
                    //
                    mtbc = (CyEditorTextBoxCell)this.CellTemplate;
                    mtbc.ValidatingType = value;

                    //
                    // now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {
                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvc = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvc is CyEditorTextBoxCell)
                            {
                                mtbc = (CyEditorTextBoxCell)dgvc;
                                mtbc.ValidatingType = value;
                            }
                        }
                    }
                }
            }
        }

    }

}
