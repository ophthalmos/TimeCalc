using System.Collections;
using System.Globalization;

namespace TimeCalc
{
    public class RowComparer : IComparer
    {
        private static int sortOrderModifier = 1;

        public RowComparer(SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Descending) { sortOrderModifier = -1; }
            else if (sortOrder == SortOrder.Ascending) { sortOrderModifier = 1; }
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow rowX = (DataGridViewRow)x;
            DataGridViewRow rowY = (DataGridViewRow)y;
            DateTime dateX = DateTime.ParseExact(rowX.Cells[0].Value.ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime dateY = DateTime.ParseExact(rowY.Cells[0].Value.ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            int CompareResult = DateTime.Compare(dateX, dateY); // Try to sort based on the Datum column.
            if (CompareResult == 0) { CompareResult = string.Compare(rowX.Cells[1].Value.ToString(), rowY.Cells[1].Value.ToString()); } // If the Datum are equal, sort based on the Uhrzeit 'von'.
            return CompareResult * sortOrderModifier;
        }

    }
}
