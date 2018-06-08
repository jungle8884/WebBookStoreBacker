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
    public class WebCommentController : Controller
    {
        // GET: WebComment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WebCommentList()
        {
            return View();
        }

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public JsonResult GetWebCommentList()
        {
            string sUserId = Request.Params["iUserId"];
            int pageindex = Convert.ToInt32(Request.Form["page"].ToString());
            int pagesize = Convert.ToInt32(Request.Form["rows"].ToString());

            Expression<Func<WebComment, bool>> sWhere = u => u.WebCommentId > 0;
            if (sUserId != "" && sUserId != null)
            {
                int iUserId = Convert.ToInt32(sUserId);
                if (iUserId > 0)
                {
                    sWhere = sWhere.And(u => u.UserId == iUserId);
                } 
            }

            List<WebComment> list = new BaseDAL<WebComment>().GetPagedList(pageindex, pagesize, sWhere, u => u.WebCommentId, false);
            dictionary.Add("total", new BaseDAL<WebComment>().GetListBy(u => u.WebCommentId > 0).Count());
            dictionary.Add("rows", list);
            return Json(dictionary);
        }

        [ValidateInput(false)]
        public JsonResult UpdateWebComment(int id)
        {
            int UserId = Convert.ToInt32(Request.Form["UserId"].ToString());
            string CommentTitle = Request.Form["CommentTitle"].ToString();
            string CommentText = Server.HtmlEncode(Request.Form["CommentText"].ToString());
            DateTime CreatedTime = DateTime.Parse(Request.Form["CreatedTime"].ToString());
            string ClientIP = Request.Form["ClientIP"].ToString();

            WebComment webComment = new BaseDAL<WebComment>().GetModel(u => u.WebCommentId == id);
            webComment.UserId = UserId;
            webComment.CommentTitle = CommentTitle;
            webComment.CommentText = Server.HtmlDecode(CommentText);
            webComment.CreatedTime = CreatedTime;
            webComment.ClientIP = ClientIP;

            if (new BaseDAL<WebComment>().Modify(webComment) > 0)
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

        public JsonResult DelWebComment()
        {
            try
            {
                string sWebCommentids = Request.Form["delwebcommentids"].ToString();
                if (sWebCommentids.Contains("_"))
                {
                    string[] aWebCommentids = sWebCommentids.Split('_');
                    foreach (var WebCommenid in aWebCommentids)
                    {
                        int WebCommentid = Convert.ToInt32(WebCommenid);
                        WebComment webComment = new BaseDAL<WebComment>().GetModel(u => u.WebCommentId == WebCommentid);
                        new BaseDAL<WebComment>().Del(webComment);
                    }
                }
                else
                {
                    int WebCommentid = Convert.ToInt32(sWebCommentids);
                    WebComment webComment = new BaseDAL<WebComment>().GetModel(u => u.WebCommentId == WebCommentid);
                    new BaseDAL<WebComment>().Del(webComment);
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
            rowHeader.CreateCell(0, CellType.STRING).SetCellValue("用户ID");//创建第一行第一列的单元格，设定里面的值为ID
            rowHeader.CreateCell(1, CellType.STRING).SetCellValue("评论标题");
            rowHeader.CreateCell(2, CellType.STRING).SetCellValue("评论内容");
            rowHeader.CreateCell(3, CellType.STRING).SetCellValue("创建时间");
            rowHeader.CreateCell(4, CellType.STRING).SetCellValue("IP地址");
            List<WebComment> list = new BaseDAL<WebComment>().GetListBy(u => u.WebCommentId > 0);
            int i = 0;
            if (list.Count > 0)
            {
                foreach (var webComment in list)
                {
                    IRow rowDate = sheet1.CreateRow(i + 1);
                    rowDate.CreateCell(0, CellType.STRING).SetCellValue(webComment.UserId.ToString());
                    rowDate.CreateCell(1, CellType.STRING).SetCellValue(webComment.CommentTitle.ToString());
                    rowDate.CreateCell(2, CellType.STRING).SetCellValue(webComment.CommentText.ToString());
                    rowDate.CreateCell(3, CellType.STRING).SetCellValue(webComment.CreatedTime.ToString());
                    rowDate.CreateCell(4, CellType.STRING).SetCellValue(webComment.ClientIP.ToString());
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