using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using System.Data;

namespace Utilities.ExcelLibrary
{
    [Obsolete("Use Utilities.ExcelLibrary.Excel.Exporter instead", true)]
    public class ExcelExport
    {
        private XLWorkbook workbook;
        public string WorkBookname = "";

        public ExcelExport(string WorkBookname)
        {
            this.WorkBookname = WorkBookname.Replace(" ", "_");
            SetupExcel(this.WorkBookname);
        }
        public ExcelExport(string WorkBookName, DataTable DT)
        {
            this.WorkBookname = WorkBookName.Replace(" ", "_");
            SetupExcel(this.WorkBookname);
            AddSheet(this.WorkBookname, DT);
        }
        //public ExcelExport(string WorkBookName, GridView Grid)
        //{
        //    this.WorkBookname = Replace(WorkBookName, " ", "_");
        //    SetupExcel(this.WorkBookname);
        //    AddSheet(this.WorkBookname, Grid);
        //}
        public void AddSheet(string name)
        {
            workbook.Worksheets.Add(name);
        }
        public void AddSheet(string name, DataTable Tb)
        {
            workbook.Worksheets.Add(Tb, name);
        }
        //public void AddSheet(string Name, GridView Grid)
        //{
        //    Grid.DataBind();
        //    Data.DataTable DT = new Data.DataTable();
        //    DataControlField GC;
        //    foreach (var GC in Grid.Columns)
        //        DT.Columns.Add(new Data.DataColumn(GC.HeaderText.Replace(" ", "_")));
        //    GridViewRow GR;
        //    foreach (var GR in Grid.Rows)
        //    {
        //        Data.DataRow DR = DT.NewRow;
        //        int i;
        //        for (i = 0; i <= Grid.Columns.Count - 1; i++)
        //        {
        //            // Dim TW As New System.IO.StringWriter()
        //            // Dim HTW As New HtmlTextWriter(TW)

        //            DataControlFieldCell GC2;
        //            GC2 = GR.Cells(i);
        //            string tstr = "";
        //            tstr = GC2.Text;
        //            tstr = tstr + TreeControl(GC2);
        //            tstr = RemoveBetween(tstr, "<", ">", false);
        //            DR(i) = tstr.Trim();
        //        }
        //        DT.Rows.Add(DR);
        //    }
        //    AddSheet(Name, DT);
        //}
        public void SaveWorkbook(FileInfo file)
        {
            workbook.SaveAs(file.FullName);
        }
        public void SaveWorkbook(DirectoryInfo directory)
        {
            var file = new FileInfo(directory.FullName +"/" + WorkBookname + ".xlsx");
            workbook.SaveAs(file.FullName);
        }
        public void SaveWorkbook(Stream stream)
        {
            workbook.SaveAs(stream);
        }

        //public void SaveWorkbook(HttpResponse Response)
        //{
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment;filename=""" + WorkBookname + ".xlsx" + """");
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        workbook.SaveAs(memoryStream);
        //        memoryStream.WriteTo(Response.OutputStream);
        //        memoryStream.Close();
        //    }

        //    Response.End();
        //}

        private void SetupExcel(string Name)
        {
            workbook = new XLWorkbook();
            workbook.Properties.Title = Name;
        }


        //private static string TreeControl(Control Con)
        //{
        //    string Tstr = "";
        //    object c;
        //    foreach (var c in Con.Controls)
        //    {
        //        try
        //        {
        //            if (c.visible)
        //                Tstr = Tstr + c.text;
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        // Try
        //        // Tstr = Tstr & c.value
        //        // Catch ex As Exception

        //        // End Try
        //        try
        //        {
        //            Tstr = Tstr + TreeControl(c);
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //    return Tstr;
        //}
        //private static string RemoveBetween(string TheContent, string BeginTagName, string EndTagName, bool TruncateContent)
        //{
        //    TheContent = Replace(Replace(TheContent, "<p>", vbCrLf), "</p>", vbCrLf);
        //    var st; st = 1;
        //    var cnt; cnt = 1;
        //    while (InStr(st, TheContent, BeginTagName) > 0)
        //    {
        //        var BeginComment; BeginComment = InStr(st, TheContent, BeginTagName);
        //        var EndComment; EndComment = InStr(BeginComment + Len(BeginTagName) + 1, TheContent, EndTagName) + Len(EndTagName);
        //        var CheckOtherBegin; CheckOtherBegin = InStr(BeginComment + Len(BeginTagName) + 1, TheContent, "<");
        //        if (CheckOtherBegin == 0 | (CheckOtherBegin == EndComment - Len(EndTagName)) | CheckOtherBegin >= EndComment)
        //            TheContent = Mid(TheContent, 1, BeginComment - 1) + Mid(TheContent, EndComment);
        //        else
        //            st = CheckOtherBegin;
        //        cnt = cnt + 1;
        //        if (cnt > 20000)
        //            break;
        //    }
        //    if (TheContent != null && TruncateContent && TheContent.Length > 45)
        //        TheContent = TheContent.Substring(0, 45) + "...";
        //    // TheContent = Replace(TheContent, vbCrLf, "<br/>")
        //    return TheContent;
        //}

        //public static void RespondWith(string WorkBookName, DataTable DT)
        //{
        //    ExcelExport EE = new ExcelExport(WorkBookName, DT);
        //    HttpResponse Response = HttpContext.Current.Response;
        //    EE.SaveWorkbook(Response);
        //}
        //public static void RespondWith(string WorkBookName, GridView Grid)
        //{
        //    ExcelExport EE = new ExcelExport(WorkBookName, Grid);
        //    HttpResponse Response = HttpContext.Current.Response;
        //    EE.SaveWorkbook(Response);
        //}
    }

}
