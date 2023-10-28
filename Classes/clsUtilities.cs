using System.Globalization; // CultureInfo
using System.Reflection; // Assembly
using System.Text.RegularExpressions;

namespace TimeCalc
{
    class ClsUtilities
    {
        public struct DateDiff { public int years, months, days;        }
        private const string dtSeparator = "[:.;,/*-]"; // »+« muss am Anfang stehen, »-« am Ende! backslash is NOT a metacharacter in a POSIX bracket expression

        public static string GetDescription()
        {
            Type clsType = typeof(FrmTimeCalc);
            Assembly assy = clsType.Assembly;
            AssemblyDescriptionAttribute adAttr = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assy, typeof(AssemblyDescriptionAttribute));
            if (adAttr == null) { return string.Empty; }
            return adAttr.Description;
        }

        public static DateTime NormalizePause(string timeString)
        { /* Testdaten:
            */
            timeString = Regex.Replace(timeString.Trim(), @"\D*$", ""); // Non-Digits/Separatoren am Stringende enfernen. 
            if (string.IsNullOrEmpty(timeString)) { return DateTime.MinValue; }
            // Versuch, das Datum direkt zu konvertieren
            string[] formats = { "H:m",
                                 "H:m:s",
                                 "H.m",
                                 "H.m.s",
                                 "t",   // kurzes Zeitformat: 07:12
                                 "T" }; // langes Zeitformat: 07:12:34
            if (DateTime.TryParseExact(timeString, formats, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime time)) { return time; }
            int hour = 0, minute = 0;
            string pattern = @"^(?:" +
               @"(?<minute>\d{1,2})|" +  // 1 || 2 Ziffern
               @"(?<hour>\d{1,2})(?<minute>\d{2})|" +  // 3 || 4 Ziffern
               @"(?<hour>\d{1,2})" + dtSeparator + @"{1}(?<minute>\d{1,2}))?$";
            Match match = Regex.Match(timeString, pattern);
            if (match.Success)
            {// MessageBox.Show("<day>" + match.Groups["day"].Value + ", <month>" + match.Groups["month"].Value + ", <year>" + match.Groups["year"].Value);
                minute = Convert.ToInt32(match.Groups["minute"].Value);
                if (match.Groups["hour"].Success)
                {
                    hour = Convert.ToInt32(match.Groups["hour"].Value);
                }
                //if (hour >= 24 && minute < 10) // hour 2-stellig, minute 0 bis 9, d.h. einstellig 
                //{// Der %-Operator berechnet den Rest, der bei der Division des ersten durch den zweiten Operanden übrig bleibt.
                //    minute = minute >= 0 ? (hour % 10) * 10 + minute : hour % 10; // muss vor nachfolgender Zeile stehen
                //    hour = ((hour - (hour % 10)) / 10); // 25 => 2
                //}
                //if (minute == -1) { minute++; } // keine Minuten? => 0
            }
            //MessageBox.Show("hour: " + hour + Environment.NewLine + "minute: " + minute);
            try { var now = DateTime.Now; return new DateTime(now.Year, now.Month, now.Day, hour, minute, now.Second); }
            catch { return DateTime.MinValue; }
        }

        public static DateTime NormalizeTime(string timeString)
        { /* Testdaten:
           * 245
             (?<name>subexpression)  => Gruppierungskonstrukt, ermöglicht den Zugriff nach Name oder Zahl
             \d{2}                   => Entsprechung für zwei Dezimalstellen finden
                                        {3} means exactly 3 occurrences, and {3,} means 3 or more occurrences
             \d{1,2}                 => Entsprechung für entweder eine oder zwei Dezimalstellen finden
                                        For example, a{1,2} matches ab but only the first two a's in aaab
             |                       => The vertical bar separates two or more alternatives
            */
            timeString = Regex.Replace(timeString.Trim(), @"\D*$", ""); // Non-Digits/Separatoren am Stringende enfernen. 
            if (string.IsNullOrEmpty(timeString)) { return DateTime.MinValue; }
            // Versuch, das Datum direkt zu konvertieren
            string[] formats = { "H:m",
                                 "H:m:s",
                                 "H.m",
                                 "H.m.s",
                                 "t",   // kurzes Zeitformat: 07:12
                                 "f",   // Datum & Uhrzeit komplett (kurz):	Samstag, 29. November 2014 07:12
                                 "F",   //	Datum & Uhrzeit komplett (lang): Samstag, 29. November 2014 07:12:34
                                 "g",   //	Standard-Datum (kurz): 29.11.2014 07:12
                                 "G",   //	Standard-Datum (lang):	29.11.2014 07:12:34
                                 "T" }; // langes Zeitformat: 07:12:34
            if (DateTime.TryParseExact(timeString, formats, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime time)) { return time; }

            int hour = -1, minute = -1;
            string pattern = @"^(?:" +
               @"(?<hour>\d{1,2})|" +  // 1 || 2 Ziffern
               @"(?<hour>\d{2})(?<minute>\d{1,2})|" +  // 3 || 4 Ziffern
               @"(?<hour>\d{1,2})" + dtSeparator + @"{1}(?<minute>\d{1,2}))?$";
            Match match = Regex.Match(timeString, pattern);
            if (match.Success)
            {// MessageBox.Show("<day>" + match.Groups["day"].Value + ", <month>" + match.Groups["month"].Value + ", <year>" + match.Groups["year"].Value);
                hour = Convert.ToInt32(match.Groups["hour"].Value);
                if (match.Groups["minute"].Success)
                {
                    minute = Convert.ToInt32(match.Groups["minute"].Value);
                }// MessageBox.Show(hour.ToString() + " | " + minute.ToString());
                if (hour >= 24 && minute < 10) // hour 2-stellig, minute 0 bis 9, d.h. einstellig 
                {// Der %-Operator berechnet den Rest, der bei der Division des ersten durch den zweiten Operanden übrig bleibt.
                    minute = minute >= 0 ? (hour % 10) * 10 + minute : hour % 10; // muss vor nachfolgender Zeile stehen
                    hour = ((hour - (hour % 10)) / 10); // 25 => 2
                }
                if (minute == -1) { minute++; } // keine Minuten? => 0
            }
            //MessageBox.Show("hour: " + hour + Environment.NewLine + "minute: " + minute);
            try { var now = DateTime.Now; return new DateTime(now.Year, now.Month, now.Day, hour, minute, now.Second); }
            catch { return DateTime.MinValue; }
        }

        public static DateTime NormalizeDate(string dateString)
        { /* Testdaten:
           *   45,
           *   365,
           *   411,
           *   2365,
           *   9952,
           *   12110,
           *   12121,
           *   31061,
           *   3101961,
           *   2352015,
           *   5122015
           */
            dateString = Regex.Replace(dateString.Trim(), @"[^\d.]*$", ""); // Non-Digits/Separatoren am Ende enfernen (außer Punkt)
            if (string.IsNullOrEmpty(dateString)) { return DateTime.MinValue; }

            // Versuch, das Datum direkt zu konvertieren
            string[] formats = { "d",   // Kurzes Datum: 10.04.2008 
                                 "d.M.",
                                 "d.M.yy",
                                 "dd.MM.yy",
                                 "d.M.yyyy",
                                 "dd.MM.yyyy",
                                 "d. MMM. yyyy",
                                 "d. MMMM yyyy",
                                 "dddd, 'den' d. MMMM yyyy",
                                 "D" }; // langes Datum: Donnerstag, 10. April 2008
            if (DateTime.TryParseExact(dateString, formats, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime date)) { return date; }

            int day = -1, month = -1, year = -1, tempYear = -1;
            string pattern = @"^(?:" +
               @"(?<day>\d{1,2})|" + // 1 oder 2 Ziffern
               @"(?<day>\d{2})(?<month>\d{1,2})|" + // 3 oder 4 Ziffern
               @"(?<day>\d{2})(?<month>\d{1,2})(?<year>\d{2,4})|" +  // 5, 6, 7 oder 8 Ziffern
               @"(?<day>\d{1,2})" + dtSeparator + @"{1}(?<month>\d{1,2})?|" +
               @"(?<day>\d{1,2})" + dtSeparator + @"{1}(?<month>\d{1,2})" + dtSeparator + @"{1}(?<year>\d{2,4}))?$";
            Match match = Regex.Match(dateString, pattern);
            if (match.Success)
            {// MessageBox.Show("<day>" + match.Groups["day"].Value + ", <month>" + match.Groups["month"].Value + ", <year>" + match.Groups["year"].Value);
                day = Convert.ToInt32(match.Groups["day"].Value);
                if (match.Groups["month"].Success)
                {
                    month = Convert.ToInt32(match.Groups["month"].Value);
                    if (match.Groups["year"].Success) { year = tempYear = Convert.ToInt32(match.Groups["year"].Value); }
                }
                if (year == -1)
                {
                    if (month == -1) // ein- oder zweistellige Eingaben
                    {// year ist ebenfalls -1 (s.o.)
                        if (day > DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                        {// Beispiel: Eingabe "45"
                            month = day % 10; //Beispiel: day = 45 => 5
                            day = (day - (day % 10)) / 10; // => 4
                        }
                        else { month = day > DateTime.Now.Day ? DateTime.Now.Month - 1 : DateTime.Now.Month; }
                    }
                    else if (month > 12) // also in jedem Fall 4 stellige Eingabe
                    {// Beispiel: Eingabe "2365"
                        year = month % 10; //Beispiel: month = 65 => 5
                        month = (month - (month % 10)) / 10; // => 6
                        if (day > DateTime.DaysInMonth(DateTime.Now.Year, month))
                        {// Beispiel: Eingabe "9952" (hier als 99|5|2)
                            year = month * 10 + year;
                            month = day % 10; //Beispiel: day = 45 => 5
                            day = (day - (day % 10)) / 10; // => 4
                        }
                    }
                    else if (day > 9) // also in jedem Fall 3-stellige Eingabe, day 2 stellig und deshalb month 1-stellig
                    {// Beispiel: Eingabe "411" oder aber "365" => Abfrage mit 3. Ziffer als Monat (2. Beispiel: "5")
                        if (month > 0 && month <= 12 & day > DateTime.DaysInMonth(DateTime.Now.Year, month))
                        {// Beispiel: Eingabe "365", "550"
                            year = month % 10; // month 5 => year 5
                            month = day % 10;  // day 36  => month 6
                            day = (day - (day % 10)) / 10; // day 36  => day 3
                            if (month == 1 && year < 3)
                            {// Beispiel: Eingabe "411"
                                month = 10 + year; // month 1 => month 11
                                year = -1; // year 1 => year -1
                            }
                        }
                    }
                    if (year == -1) // d.h. "immer noch -1" - year wurde noch nicht hochgerechnet
                    {
                        year = DateTime.Now.Year;
                    }
                }
                else if (year < 1000) // Jahresangabe maximal 3-stellig vorhanden
                {// Beispiel: Eingabe "2352015" (23.52.015)
                    if (month > 12)
                    {
                        year += (month % 10) * 1000; //Beispiel: month = 52 => 2 015
                        month = (month - (month % 10)) / 10; // => 5
                    }
                    if (month > 0 && day > DateTime.DaysInMonth(year, month)) // Cave: month kann hier noch 0 sein => führt zu Abbruch! Beispiel: "31061"
                    {// Beispiel: Eingabe "5122015" (51.22.015 => obiges if: 51.2.2015)
                        month = day % 10 * 10 + month; //Beispiel: day = 51 => 1 * 10 + 2 = 12
                        day = (day - (day % 10)) / 10; // day 51  => day 5
                    }
                }
                if (month == 0 && day > 9) // day.ToString().Length == 2) // Beispiel: "3101961" => day = 31, month = 0, year = 1961
                {
                    month = day % 10 * 10; //Beispiel: day = 31 => 10
                    day = (day - (day % 10)) / 10; // => 3
                }// MessageBox.Show(day.ToString() + " | " + month.ToString() + " | " + year.ToString());
                if (year > 0 && month < 10 && tempYear == -1 && day > DateTime.DaysInMonth(year, month == 0 ? 10 : month))
                {// Beispiel: Eingabe "410", "5.50" => 5.5.0
                    month = (day % 10) * 10 + month; // muss vor nachfolgender Zeile stehen
                    day = (day - (day % 10)) / 10;
                }
                if (year <= 99)  // Testeingabe: "12121", "12110"
                {
                    year = (2000 + year > DateTime.Now.Year ? year + 1900 : year + 2000);
                }
                else if (year <= 999)  // Testeingabe: "12121", "12110"
                {
                    year = (2000 + year > DateTime.Now.Year ? year + 1000 : year + 2000);
                }
            }
            try { return new DateTime(year, month, day); }
            catch { return DateTime.MinValue; }
        }

        public static DateDiff CalcDateDiff(DateTime d1, DateTime d2)
        {
            DateTime dtTemp;
            bool isMinus = false;
            int years, months, days;

            if (d1 > d2)
            {// MessageBox.Show("isMinus");
                dtTemp = d1;
                d1 = d2;
                d2 = dtTemp;
                isMinus = true;
            }

            years = d2.Year - d1.Year;
            dtTemp = d1.AddYears(years);
            if (dtTemp > d2)
            {//so the year is not complete yet... 
                years--;
                dtTemp = d1.AddYears(years);
            }
            months = d2.Month - d1.Month;
            if (d2.Day < d1.Day) months--;
            months = (months + 12) % 12;
            dtTemp = dtTemp.AddMonths(months);
            days = (d2 - dtTemp).Days;

            DateDiff ddf;
            ddf.years = isMinus ? years * -1 : years;
            ddf.months = isMinus ? months * -1 : months;
            ddf.days = isMinus ? days * -1 : days;
            //MessageBox.Show("DateDiff: Years: " + ddf.years.ToString() + ", Months: " + ddf.months.ToString() + ", Days: " + ddf.days.ToString());
            return (ddf);
        }

        public static bool IsDGVEmpty(DataGridView gridView)
        {
            bool isEmpty = true;
            for (int row = 0; row < gridView.RowCount - 1; row++)
            {
                for (int col = 0; col < gridView.Columns.Count; col++)
                {
                    if (gridView.Rows[row].Cells[col].Value != null && !string.IsNullOrEmpty(gridView.Rows[row].Cells[col].Value.ToString()))
                    { isEmpty = false; break; }
                }
            }
            return isEmpty;
        }

        public static void RemoveEmptyRows(DataGridView gridView, List<int> intDTEdits)
        {
            for (int row = 0; row < gridView.RowCount; row++)
            {
                bool isEmpty = true;
                for (int col = 0; col < gridView.ColumnCount; col++)
                {
                    if (intDTEdits.Contains(col) && gridView.Rows[row].Cells[col].Value != null && !string.IsNullOrWhiteSpace(gridView.Rows[row].Cells[col].Value.ToString()))
                    { isEmpty = false; break; }
                }
                if (isEmpty && !gridView.Rows[row].IsNewRow)
                { gridView.Rows.RemoveAt(row--); } // deincrement (after the call) since we are removing the row
            }
        }

        //public static bool IsNullOrWhiteSpace(string value)
        //{// ab .Net 4.5 verfügbar
        //    if (value == null) return true;
        //    for (int i = 0; i < value.Length; ++i)
        //    {// return value.All(char.IsWhiteSpace); using System.Linq (auch > .Net 4.0;)
        //        if (!char.IsWhiteSpace(value[i])) return false;
        //    }
        //    return true;
        //}

        //public static string TextIfNothingToSave(string text)
        //{
        //    if (!text.Contains("-")) { return text; }
        //    else { return Regex.Replace(text, @"\* - ", " - "); }
        //}

        //public static string TextIfContentToSave(string text)
        //{
        //    if (!text.Contains("-")) { return text; }
        //    else { return Regex.Replace(text, @"\*? - ", "* - "); }
        //}

    }
}
