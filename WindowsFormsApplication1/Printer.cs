using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace WindowsFormsApplication1
{
    public class Printer
    {
        public delegate void dlg_Print(Graphics g);
        public event dlg_Print Printer_Print;
        PrintDocument printDocument = new PrintDocument();
        public void Print(string Name = "默认打印", int Width = 356, int Height = 1070, int RawKind = 150)
        {
            //printDocument.PrinterSettings可以获取或设置计算机默认打印相关属性或参数，如：printDocument.PrinterSettings.PrinterName获得默认打印机打印机名称
            //printDocument.DefaultPageSettings //可以获取或设置打印页面参数信息、如是纸张大小，是否横向打印等

            //设置文档名
            printDocument.DocumentName = Name;//设置完后可在打印对话框及队列中显示（默认显示document）

            //设置纸张大小（可以不设置取，取默认设置）
            PaperSize ps = new PaperSize(Name, Width, Height);
            ps.RawKind = RawKind; //如果是自定义纸张，就要大于118，（A4值为9，详细纸张类型与值的对照请看http://msdn.microsoft.com/zh-tw/library/system.drawing.printing.papersize.rawkind(v=vs.85).aspx）
            printDocument.DefaultPageSettings.PaperSize = ps;
 //           ps= printDocument.DefaultPageSettings.PaperSize;

            //打印开始前
            //printDocument.BeginPrint += new PrintEventHandler(printDocument_BeginPrint);
            //打印输出（过程）
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
            //打印结束
            printDocument.EndPrint += new PrintEventHandler(printDocument_EndPrint);

            //跳出打印对话框，提供打印参数可视化设置，如选择哪个打印机打印此文档等
//            PrintDialog pd = new PrintDialog();
//            pd.Document = printDocument;
//            if (DialogResult.OK != pd.ShowDialog()) return; //如果确认，将会覆盖所有的打印参数设置

            ////页面设置对话框（可以不使用，其实PrintDialog对话框已提供页面设置）
            //PageSetupDialog psd = new PageSetupDialog();
            //psd.Document = printDocument;
            //if (DialogResult.OK != psd.ShowDialog()) return;

            //打印预览
            //PrintPreviewDialog ppd = new PrintPreviewDialog();
            //ppd.Document = printDocument;
            //if (DialogResult.OK == ppd.ShowDialog())
            printDocument.Print(); //打印

        }
        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (Printer_Print != null)
                Printer_Print(e.Graphics);
        }

        void printDocument_EndPrint(object sender, PrintEventArgs e)
        {
            //打印结束后相关操作
            
        }
    }
}
