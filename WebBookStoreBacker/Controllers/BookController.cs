using Dal;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace WebBookStoreBacker.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BookList()
        {
            return View();
        }

        public ActionResult GetReport()
        {
            return View();
        }

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public JsonResult GetBookList()
        {
            string sBookName = Request.Params["sBookName"];
            int pageindex = Convert.ToInt32(Request.Form["page"].ToString());
            int pagesize = Convert.ToInt32(Request.Form["rows"].ToString());

            Expression<Func<Book, bool>> sWhere = u => u.BookId > 0;

            if (sBookName != "" && sBookName != null)
            {
                sWhere = sWhere.And(u => u.BookName.Contains(sBookName));
            }

            List<Book> list = new BaseDAL<Book>().GetPagedList(pageindex, pagesize, sWhere, u => u.BookId, false);
            dictionary.Add("total", new BaseDAL<Book>().GetListBy(u => u.BookId > 0).Count());
            dictionary.Add("rows", list);
            return Json(dictionary);
        }

        public JsonResult AddBook()
        {
            string BookName = Request.Form["BookName"].ToString();
            string BookPublication = Request.Form["BookPublication"].ToString();
            DateTime BookPublicTime = DateTime.Parse(Request.Form["BookPublicTime"].ToString());
            double BookPrice = double.Parse(Request.Form["BookPrice"].ToString());
            string BookISBN = Request.Form["BookISBN"].ToString();
            string BookImage = Request.Form["BookImage"].ToString();
            string BookAuthor = Request.Form["BookAuthor"].ToString();
            string BookTranslator = Request.Form["BookTranslator"].ToString();
            string BookIntroduce = Request.Form["BookIntroduce"].ToString();
            string BookClassfication = Request.Form["BookClassfication"].ToString();

            Book book = new Book();
            book.BookName = BookName;
            book.BookPublication = BookPublication;
            book.BookPublicTime = BookPublicTime;
            book.BookPrice = BookPrice;
            book.BookISBN = BookISBN;
            book.BookImage = BookImage;
            book.BookAuthor = BookAuthor;
            book.BookTranslator = BookTranslator;
            book.BookIntroduce = BookIntroduce;
            book.BookClassfication = BookClassfication;

            try
            {
                if (new BaseDAL<Book>().Add(book) > 0)
                {
                    dictionary.Add("success", true);
                }
                else
                {
                    dictionary.Add("success", false);
                    dictionary.Add("infor", "添加失败");
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return Json(dictionary);
        }

        public JsonResult UpdateBook(int id)
        {
            string BookName = Request.Form["BookName"].ToString();
            string BookPublication = Request.Form["BookPublication"].ToString();
            DateTime BookPublicTime = DateTime.Parse(Request.Form["BookPublicTime"].ToString());
            double BookPrice = double.Parse(Request.Form["BookPrice"].ToString());
            string BookISBN = Request.Form["BookISBN"].ToString();
            string BookImage = Request.Form["BookImage"].ToString();
            string BookAuthor = Request.Form["BookAuthor"].ToString();
            string BookTranslator = Request.Form["BookTranslator"].ToString();
            string BookIntroduce = Request.Form["BookIntroduce"].ToString();
            string BookClassfication = Request.Form["BookClassfication"].ToString();

            Book book = new BaseDAL<Book>().GetModel(u => u.BookId == id);
            book.BookName = BookName;
            book.BookPublication = BookPublication;
            book.BookPublicTime = BookPublicTime;
            book.BookPrice = BookPrice;
            book.BookISBN = BookISBN;
            book.BookImage = BookImage;
            book.BookAuthor = BookAuthor;
            book.BookTranslator = BookTranslator;
            book.BookIntroduce = BookIntroduce;
            book.BookClassfication = BookClassfication;
            
            if (new BaseDAL<Book>().Modify(book) > 0)
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

        public JsonResult DelBook()
        {
            try
            {
                string sBookids = Request.Form["delbookids"].ToString();
                if (sBookids.Contains("_"))
                {
                    string[] aBookids = sBookids.Split('_');
                    foreach (var id in aBookids)
                    {
                        int bookid = Convert.ToInt32(id);
                        Book book = new BaseDAL<Book>().GetModel(u => u.BookId == bookid);
                        new BaseDAL<Book>().Del(book);
                    }
                }
                else
                {
                    int bookid = Convert.ToInt32(sBookids);
                    Book book = new BaseDAL<Book>().GetModel(u => u.BookId == bookid);
                    new BaseDAL<Book>().Del(book);
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
            rowHeader.CreateCell(0, CellType.STRING).SetCellValue("图书名ID");//创建第一行第一列的单元格，设定里面的值为ID
            rowHeader.CreateCell(1, CellType.STRING).SetCellValue("图书名");
            rowHeader.CreateCell(2, CellType.STRING).SetCellValue("出版社");
            rowHeader.CreateCell(3, CellType.STRING).SetCellValue("价格");
            rowHeader.CreateCell(4, CellType.STRING).SetCellValue("ISBN");
            rowHeader.CreateCell(5, CellType.STRING).SetCellValue("图片");
            rowHeader.CreateCell(6, CellType.STRING).SetCellValue("作者");
            rowHeader.CreateCell(7, CellType.STRING).SetCellValue("译者");
            rowHeader.CreateCell(8, CellType.STRING).SetCellValue("介绍");
            rowHeader.CreateCell(8, CellType.STRING).SetCellValue("分类");
            List<Book> list = new BaseDAL<Book>().GetListBy(u => u.BookId > 0);
            int i = 0;
            if (list.Count > 0)
            {
                foreach (var book in list)
                {
                    IRow rowDate = sheet1.CreateRow(i + 1);
                    rowDate.CreateCell(0, CellType.STRING).SetCellValue(book.BookId.ToString());
                    rowDate.CreateCell(1, CellType.STRING).SetCellValue(book.BookName.ToString());
                    rowDate.CreateCell(2, CellType.STRING).SetCellValue(book.BookPrice.ToString());
                    rowDate.CreateCell(3, CellType.STRING).SetCellValue(book.BookISBN.ToString());
                    rowDate.CreateCell(4, CellType.STRING).SetCellValue(book.BookImage.ToString());
                    rowDate.CreateCell(5, CellType.STRING).SetCellValue(book.BookAuthor.ToString());
                    rowDate.CreateCell(6, CellType.STRING).SetCellValue(book.BookTranslator.ToString());
                    rowDate.CreateCell(7, CellType.STRING).SetCellValue(book.BookIntroduce.ToString());
                    rowDate.CreateCell(8, CellType.STRING).SetCellValue(book.BookClassfication.ToString());
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

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Files[0]; //获取选中文件  
                var filecombin = file.FileName.Split('.');
                if (file == null || String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0 || filecombin.Length < 2)
                {
                    return Json(new
                    {
                        fileid = 0,
                        src = "",
                        name = "",
                        msg = "上传出错 请检查文件名 或 文件内容"
                    });
                }
                //定义本地路径位置
                string localPath = ConfigurationManager.AppSettings["Upload"].ToString();
                string TempUpLoadPath = ConfigurationManager.AppSettings["TempUpLoad"].ToString();
                if (!System.IO.Directory.Exists(localPath))
                    System.IO.Directory.CreateDirectory(localPath);

                string fileName = file.FileName;//临时保存文件名
                int fileIndex = 0;
                string localURL = string.Empty;
                //判断是否存在相同文件名的文件 相同累加1继续判断
                if (System.IO.File.Exists(localPath + fileName))
                {
                    fileName = filecombin[0] + "_" + ++fileIndex + "." + filecombin[1];
                }
                localURL = Path.Combine(TempUpLoadPath, fileName);
                file.SaveAs(localURL);   //保存图片（临时保持）
                localURL = Path.Combine(localPath, fileName);
                file.SaveAs(localURL);   //保存图片（文件夹）
                

                return Json(new
                {
                    src = localURL,
                    name = fileName,   
                    msg = "上传成功"
                });
            }
            catch
            {

            }

            return Json(new
            {
                src = "",
                name = "",   
                msg = "上传出错"
            });
        }
    }
}