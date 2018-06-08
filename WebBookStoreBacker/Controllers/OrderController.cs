using Dal;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace WebBookStoreBacker.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderList()
        {
            return View();
        }

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public JsonResult GetOrderList()
        {
            string selKeyWords = Request.Params["selKeyWords"];
            int pageindex = Convert.ToInt32(Request.Form["page"].ToString());
            int pagesize = Convert.ToInt32(Request.Form["rows"].ToString());

            Expression<Func<BookOrder, bool>> sWhere = u => u.BookOrderId > 0;

            if (selKeyWords != "" && selKeyWords != null)
            {
                sWhere = sWhere.And(u => u.UserName.Contains(selKeyWords));
            }

            List<BookOrder> list = new BaseDAL<BookOrder>().GetPagedList(pageindex, pagesize, sWhere, u => u.BookOrderId, false);
            dictionary.Add("total", new BaseDAL<BookOrder>().GetListBy(u => u.BookOrderId > 0).Count());
            dictionary.Add("rows", list);
            return Json(dictionary);
        }

        public JsonResult UpdateOrder(int id)
        {
            string BookName = Request.Form["BookName"].ToString();
            string UserName = Request.Form["UserName"].ToString();
            int BookAmount = int.Parse(Request.Form["BookAmount"].ToString());
            double BookPrice = double.Parse(Request.Form["BookPrice"].ToString());
            string OrderNumber = Request.Form["OrderNumber"].ToString();

            BookOrder bookOrder = new BaseDAL<BookOrder>().GetModel(u => u.BookOrderId == id);
            bookOrder.BookName = BookName;
            bookOrder.UserName = UserName;
            bookOrder.BookAmount = BookAmount;
            bookOrder.BookPrice = BookPrice;
            bookOrder.BookTotalPrice = BookAmount * BookPrice;
            bookOrder.OrderNumber = OrderNumber;

            if (new BaseDAL<BookOrder>().Modify(bookOrder) > 0)
            {
                dictionary.Add("success", true);
            }
            else
            {
                dictionary.Add("success", false);
                dictionary.Add("infor", "更新失败");
            }
            return Json(dictionary);
        }

        public JsonResult DelOrder()
        {
            try
            {
                string sBookOrderids = Request.Form["delbookorderids"].ToString();
                if (sBookOrderids.Contains("_"))
                {
                    string[] aBookOrderids = sBookOrderids.Split('_');
                    foreach (var BookOrderid in aBookOrderids)
                    {
                        int bookOrderid = Convert.ToInt32(BookOrderid);
                        BookOrder bookOrder = new BaseDAL<BookOrder>().GetModel(u => u.BookOrderId == bookOrderid);
                        new BaseDAL<BookOrder>().Del(bookOrder);
                    }
                }
                else
                {
                    int bookOrderid = Convert.ToInt32(sBookOrderids);
                    BookOrder bookOrder = new BaseDAL<BookOrder>().GetModel(u => u.BookOrderId == bookOrderid);
                    new BaseDAL<BookOrder>().Del(bookOrder);
                }
                dictionary.Add("success", true);
            }
            catch
            {
                dictionary.Add("success", false);
            }
            return Json(dictionary);
        }

        public string GetExcel()
        {
            string filename = "";
            HSSFWorkbook workbook = new HSSFWorkbook();//创建一个工作薄
            ISheet sheet1 = workbook.CreateSheet();//创建一个sheet页
            IRow rowHeader = sheet1.CreateRow(0);//创建第一行
            rowHeader.CreateCell(0, CellType.STRING).SetCellValue("订单ID");//创建第一行第一列的单元格，设定里面的值为ID
            rowHeader.CreateCell(1, CellType.STRING).SetCellValue("图书名");
            rowHeader.CreateCell(2, CellType.STRING).SetCellValue("用户名");
            rowHeader.CreateCell(3, CellType.STRING).SetCellValue("购买数目");
            rowHeader.CreateCell(4, CellType.STRING).SetCellValue("单价");
            rowHeader.CreateCell(5, CellType.STRING).SetCellValue("图书总价");
            rowHeader.CreateCell(6, CellType.STRING).SetCellValue("订单号");
            List<BookOrder> list = new BaseDAL<BookOrder>().GetListBy(u => u.BookOrderId > 0);
            int i = 0;
            if (list.Count > 0)
            {
                foreach (var bookOrder in list)
                {
                    IRow rowDate = sheet1.CreateRow(i + 1);
                    rowDate.CreateCell(0, CellType.STRING).SetCellValue(bookOrder.BookOrderId.ToString());
                    rowDate.CreateCell(1, CellType.STRING).SetCellValue(bookOrder.BookName.ToString());
                    rowDate.CreateCell(2, CellType.STRING).SetCellValue(bookOrder.UserName.ToString());
                    rowDate.CreateCell(3, CellType.STRING).SetCellValue(bookOrder.BookAmount.ToString());
                    rowDate.CreateCell(4, CellType.STRING).SetCellValue(bookOrder.BookPrice.ToString());
                    rowDate.CreateCell(5, CellType.STRING).SetCellValue(bookOrder.BookTotalPrice.ToString());
                    rowDate.CreateCell(6, CellType.STRING).SetCellValue(bookOrder.OrderNumber.ToString());
                    i++;
                }

                filename = "/file/" + Guid.NewGuid().ToString() + ".xls";

                using (Stream stream = System.IO.File.OpenWrite(Server.MapPath(filename)))
                {
                    workbook.Write(stream);
                }
            }
            return filename;
        }

    }
}