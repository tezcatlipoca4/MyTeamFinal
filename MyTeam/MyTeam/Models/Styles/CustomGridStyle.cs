using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.SfDataGrid.XForms;

namespace MyTeam.Models.Styles
{
    public class CustomGridStyle : DataGridStyle
    {
        public override GridLinesVisibility GetGridLinesVisibility()
        {
            // return base.GetGridLinesVisibility();
            return GridLinesVisibility.Horizontal;
        }
    }
    
}
