using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Vml;

namespace Utilities.ExcelLibrary
{
    [Obsolete("Use Utilities.ExcelLibrary.CSV.Exporter instead", true)]
    public class CSVFile
    {
        public class CSVLine
        {
            private List<string> m_Columns = new List<string>();

            public int ColumnNumber(string value)
            {
                return Columns.IndexOf((from c in Columns
                                        where c.ToUpper() == value.ToUpper()
                                        select c).FirstOrDefault());
            }


            public List<string> Columns
            {
                get
                {
                    return m_Columns;
                }
            }

            public void AddColumn(object value)
            {
                var s = "";

                if (value != null)
                    s = System.Convert.ToString(value);
                AddColumn(s);
            }
            public void AddColumn(string value)
            {
                var curPos = m_Columns.Count - 1;
                ColumnSet(curPos + 1, value);
            }


            public string ColumnGet(int Pos)
            {
                if (Columns.Count <= Pos)
                    return "";
                return Columns[Pos];
            }
            public void ColumnSet(int Pos, string value)
            {
                while (Columns.Count <= Pos)
                    Columns.Add("");
                Columns[Pos] = value;
            }
            public CSVLine()
            {
            }
            public CSVLine(List<string> Cols)
            {
                m_Columns = Cols;
            }
            public string GetCSVLine(string Delimiter = ",")
            {
                var sb = new StringBuilder();
                var m = new StringWriter(sb);
                TextWriter tw = m;// new StreamWriter(m);

                var config = new CsvHelper.Configuration.Configuration();
                config.Delimiter = Delimiter;

                var p = new CsvHelper.CsvWriter(tw, config, false);

                foreach(var c in Columns)
                {
                    p.WriteField(c);
                }
                p.NextRecord();
                p.Flush();


                return sb.ToString();

                //var dbq = "\"";
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                ////string s;
                //bool First = true;
                //if (m_Columns == null)
                //{
                //    m_Columns = new List<string>();
                //}
                //foreach (var s in Columns)
                //{
                //    if (!First)
                //        sb.Append(Delimiter);
                //    else
                //        First = false;
                //    sb.Append(dbq + s.Replace(dbq, dbq + dbq) + dbq);
                //}

                //return sb.ToString();
            }
        }
        public List<CSVLine> Lines = new List<CSVLine>();
        public string ColumnDelimiter = ",";

        public void RemoveColumn(int pos)
        {
            foreach (var l in Lines)
            {
                if (l.Columns.Count > pos)
                    l.Columns.RemoveAt(pos);
            }
        }


        public string GetAsCSV()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var m = new StringWriter(sb);
            TextWriter tw = m;
            var config = new CsvHelper.Configuration.Configuration();
            config.Delimiter = ColumnDelimiter;


            var p = new CsvHelper.CsvWriter(tw, config, false);

            //CSVLine Line;
            foreach (var line in Lines)
            {
                foreach (var c in line.Columns)
                {
                    p.WriteField(c);
                }
                p.NextRecord();

            }
            p.Flush();

            return sb.ToString();
        }

        public void SaveCsv(Stream stream)
        {
            var s = new BinaryWriter(stream);
            s.Write(GetAsCSV());
        }
        public void SaveCsv(FileInfo file)
        {
            var ms = new MemoryStream();
            SaveCsv(ms);
            var s = file.OpenWrite();
            s.Write(ms.ToArray(), 0, (int)ms.Length);
            s.Close();
        }


        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromFileData(string Data, string ColDelimiter = ",")
        {
            MemoryStream mem = new MemoryStream();
            mem.Write(System.Text.Encoding.UTF8.GetBytes(Data), 0, Data.Length);
            mem.Seek(0, SeekOrigin.Begin);
            StreamReader SR = new StreamReader(mem);
            return LoadFromFileData(SR, ColDelimiter);
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromFileData(MemoryStream Data, string ColDelimiter = ",")
        {
            StreamReader SR = new StreamReader(Data);
            return LoadFromFileData(SR, ColDelimiter);
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromFileData(Stream ReadFile, string ColDelimiter = ",")
        {
            StreamReader SR = new StreamReader(ReadFile);
            return LoadFromFileData(SR, ColDelimiter);
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromFileData(StreamReader ReadFile, string ColDelimiter = ",")
        {
            CSVFile CSVF = new CSVFile();
            CSVF.ColumnDelimiter = ColDelimiter;

            var parser = new CsvHelper.CsvParser(ReadFile, new Configuration() { Delimiter = ColDelimiter });
            while ((true))
            {
                var line = parser.Read();

                if ((line == null))
                    break;
                else
                    CSVF.Lines.Add(new CSVLine(line.ToList()));
            }
            return CSVF;
        }

        //public static CSVFile LoadFromFileDataOld(StreamReader ReadFile, string ColDelimiter = ",")
        //{
        //    // Dim FileHolder As FileInfo = New FileInfo(strPath)
        //    // Dim ReadFile As StreamReader = FileHolder.OpenText()
        //    string strLine = "start";
        //    string strLineNext = "next";
        //    CSVFile CSVF = new CSVFile();
        //    CSVF.ColumnDelimiter = ColDelimiter;
        //    bool First = true;
        //    bool HasQuote = false;
        //    strLine = ReadFile.ReadLine();
        //    if (strLine.Substring(0, 1) == "\"")
        //    HasQuote = true;
        //    while (!ReadFile.EndOfStream)
        //    {
        //        strLineNext = ReadFile.ReadLine();
        //        if (strLineNext == "")
        //            strLineNext = " ";
        //        if (HasQuote)
        //        {
        //            while (!ReadFile.EndOfStream && (strLineNext.Substring(0, 1) != "\"" || strLineNext.Substring(0, 3) == "\",\""))
        //        {
        //                strLine = strLine + strLineNext;
        //                strLineNext = ReadFile.ReadLine();
        //                if (strLineNext == "")
        //                    strLineNext = " ";
        //            }
        //            if (strLineNext.Substring(0, 1) != "\"")
        //        {
        //                strLine = strLine + strLineNext;
        //                strLineNext = "";
        //            }
        //        else if (strLineNext.Substring(0, 3) == "\",\"")
        //        {
        //                strLine = strLine + strLineNext;
        //                strLineNext = "";
        //            }
        //        }
        //        if (strLine != "")
        //        {
        //            Debug.WriteLine(strLine);
        //            strLine = strLine.Replace("\r\n", "<BR/>");
        //            strLine = strLine.Replace("\r", "<BR/>");
        //            strLine = strLine.Replace("\n", "<BR/>");
        //            CSVF.Lines.Add(new CSVLine(ParseCSVLine(strLine + CSVF.ColumnDelimiter, CSVF.ColumnDelimiter)));
        //        }
        //        strLine = strLineNext;
        //    }
        //    if ((!string.IsNullOrEmpty(strLine) && strLine.Trim() != ""))
        //    {
        //        Debug.WriteLine(strLine);
        //        strLine = strLine.Replace("\r\n", "<BR/>");
        //        strLine = strLine.Replace("\r", "<BR/>");
        //        strLine = strLine.Replace("\n", "<BR/>");
        //        CSVF.Lines.Add(new CSVLine(ParseCSVLine(strLine + CSVF.ColumnDelimiter, CSVF.ColumnDelimiter)));
        //    }
        //    return CSVF;
        //}
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromFile(string strPath, string ColDelimiter = ",")
        {
            FileInfo FileHolder = new FileInfo(strPath);
            StreamReader ReadFile = FileHolder.OpenText();
            var CSVF = LoadFromFileData(ReadFile, ColDelimiter);
            ReadFile.Close();
            ReadFile = null/* TODO Change to default(_) if this is not a reference type */;
            return CSVF;
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromIEnumerable<t>(IEnumerable<t> list, bool HasHeader = true, bool useDisplayName = false)
        {
            var olist = (from i in list
                                  select i).ToList();
            return LoadFromDataTable(olist.ToDataTable(useDisplayName), HasHeader);
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Importer instead", true)]
        public static CSVFile LoadFromDataTable(DataTable DT, bool HasHeader = true)
        {
            CSVFile CSVF = new CSVFile();
            if (HasHeader)
            {
                CSVLine L = new CSVLine();

                foreach (DataColumn C in DT.Columns)
                    L.Columns.Add(C.ColumnName);
                CSVF.Lines.Add(L);
            }
            foreach (DataRow R in DT.Rows)
            {
                CSVLine L = new CSVLine();

                foreach (DataColumn C in DT.Columns)
                    L.Columns.Add(R[C.ColumnName].ToString());
                CSVF.Lines.Add(L);
            }
            return CSVF;
        }
        [Obsolete("Use Utilities.ExcelLibrary.CSV.Exporter instead", true)]
        public DataTable ToDataTable(bool hasHeader = true, int headerLine = 0)
        {
            var maxcolumns = (from l in Lines
                              select l.Columns.Count).Max();
            var table = new DataTable();

            for (var cnt = 0; cnt <= maxcolumns - 1; cnt++)
            {
                var colName = "";
                if (hasHeader)
                    colName = Lines[headerLine].ColumnGet(cnt);
                if (string.IsNullOrEmpty(colName))
                    colName = "Column_" + cnt;
                table.Columns.Add(colName);
            }
            var srow = 0;
            if (hasHeader)
                srow = headerLine + 1;
            for (var cnt = srow; cnt <= Lines.Count - 1; cnt++)
            {
                var row = table.NewRow();
                for (var cnum = 0; cnum <= maxcolumns - 1; cnum++)
                    row[cnum] = Lines[cnt].ColumnGet(cnum);
                table.Rows.Add(row);
            }

            return table;
        }


        private static List<string> ParseCSVLine(string CSVstr, string ColDelimiter = ",")
        {
            int startPos = 0;
            int endPos = 0;
            int currPos = 0;
            string tempstr = "";
            int commaPos =0;
            int quotePos = 0;
            int strLen = 0;
            int charLen = 0;

            List<string> a = new List<string>();

            startPos = 1;
            currPos = 1;

            strLen = CSVstr.Length;


            while (strLen != 0)
            {
                // CSVstr = Replace(CSVstr, "," & Chr(34) & ",", ", ,")
                // CSVstr = Replace(CSVstr, ", " & Chr(34) & ",", ", ,")
                // CSVstr = Replace(CSVstr, "," & Chr(34) & " ,", ", ,")
                CSVstr = CSVstr.Replace(",\"\"" , ", ");
                CSVstr = CSVstr.Replace("\"\",", " ,");
                CSVstr = CSVstr.Replace("\"\"", "&quot;");
                commaPos = CSVstr.IndexOf(ColDelimiter, currPos);
                quotePos = CSVstr.IndexOf("\"", currPos);
                // last data
                if (commaPos == 0)
                {
                    if (quotePos == 0)
                    {
                        if (!(currPos > endPos))
                        {
                            endPos = strLen + 1;
                            charLen = endPos - currPos;
                            tempstr =CSVstr.Substring(currPos, charLen);
                            // If Not tempstr = "" Then
                            a.Add(ReadChars(tempstr, 1, charLen, charLen).ToString().Replace("&quot;", "\""));
                        }
                    }
                    else
                    {
                        currPos = quotePos;
                        endPos = CSVstr.IndexOf("\"", currPos+1);
                        charLen = endPos - currPos;
                        tempstr = CSVstr.Substring(currPos+1, charLen - 1);

                        // If Not tempstr = "" Then
                        a.Add(ReadChars(tempstr, 1, charLen, charLen).ToString().Replace("&quot;", "\""));
                    }
                    break;
                }
                // no " in line
                if (quotePos == 0)
                {
                    endPos = commaPos;
                    charLen = endPos - currPos;
                    tempstr = CSVstr.Substring(currPos, charLen);
                    // If Not tempstr = "" Then
                    a.Add(ReadChars(tempstr, 1, charLen, charLen).ToString().Replace("&quot;", "\""));
                }
                else if ((quotePos != 0))
                {
                    // " in line
                    if (commaPos < quotePos)
                    {
                        endPos = commaPos;
                        charLen = endPos - currPos;
                        tempstr = CSVstr.Substring(currPos, charLen);
                        // If Not tempstr = "" Then
                        a.Add(ReadChars(tempstr, 1, charLen, charLen).ToString().Replace("&quot;", "\""));
                    }
                    else
                    {
                        currPos = quotePos;
                        endPos = CSVstr.IndexOf("\"", currPos + 1);
                        if (endPos <= 0)
                            endPos = CSVstr.Length;
                        charLen = endPos - currPos;
                        tempstr = CSVstr.Substring(currPos + 1, charLen-1);

                        // If Not tempstr = "" Then
                        a.Add(ReadChars(tempstr, 1, charLen, charLen).ToString().Replace("&quot;", "\""));
                        // End If
                        endPos = endPos + 1;
                    }
                }
                currPos = endPos + 1;
            }

            return a;
        }

        private static string ReadChars(string str, int StartPos, int EndPos, int strLen)
        {
            int c;
            string s = "";
            for (c = StartPos - 1; c <= EndPos; c++)
            {
                try
                {
                    if (str != null && (c + 1) <= str.Length)
                        s += str.Substring(c, 1);
                }
                catch (Exception exp)
                {
                }
            }
            return s;
        }
    }

}
