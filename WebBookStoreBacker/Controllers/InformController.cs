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
    public class InformController : Controller
    {
        // GET: Inform
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InformList()
        {
            return View();
        }

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public JsonResult GetInformList()
        {
            string selKeyWords = Request.Params["selKeyWords"];
            int pageindex = Convert.ToInt32(Request.Form["page"].ToString());
            int pagesize = Convert.ToInt32(Request.Form["rows"].ToString());

            Expression<Func<Inform, bool>> sWhere = u => u.InformId > 0;

            if (selKeyWords != "" && selKeyWords != null)
            {
                sWhere = sWhere.And(u => u.InformText.Contains(selKeyWords));
            }

            List<Inform> list = new BaseDAL<Inform>().GetPagedList(pageindex, pagesize, sWhere, u => u.InformText, false);
            dictionary.Add("total", new BaseDAL<Inform>().GetListBy(u => u.InformId > 0).Count());
            dictionary.Add("rows", list);
            return Json(dictionary);
        }

        public JsonResult AddInform()
        {
            string InformText = Request.Form["InformText"].ToString();
            int IsVisible = Convert.ToInt32(Request.Form["IsVisible"].ToString());
            int InfoType = Convert.ToInt32(Request.Form["InfoType"].ToString());
            Inform inform = new Inform();
            inform.InformText = InformText;
            inform.IsVisible = IsVisible;
            inform.InfoType = InfoType;
            try
            {
                if (new BaseDAL<Inform>().Add(inform) > 0)
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

        public JsonResult UpdateInform(int id)
        {
            string InformText = Request.Form["InformText"].ToString();
            int IsVisible = Convert.ToInt32(Request.Form["IsVisible"].ToString());
            int InfoType = Convert.ToInt32(Request.Form["InfoType"].ToString());
            Inform inform = new BaseDAL<Inform>().GetModel(u => u.InformId == id);
            inform.InformText = InformText;
            inform.IsVisible = IsVisible;
            inform.InfoType = InfoType;

            if (new BaseDAL<Inform>().Modify(inform) > 0)
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

        public JsonResult DelInform()
        {
            try
            {
                string delinformids = Request.Form["delinformids"].ToString();
                if (delinformids.Contains("_"))
                {
                    string[] aInformids = delinformids.Split('_');
                    foreach (var informid in aInformids)
                    {
                        int Informid = Convert.ToInt32(informid);
                        Inform inform = new BaseDAL<Inform>().GetModel(u => u.InformId == Informid);
                        new BaseDAL<Inform>().Del(inform);
                    }
                }
                else
                {
                    int Informid = Convert.ToInt32(delinformids);
                    Inform inform = new BaseDAL<Inform>().GetModel(u => u.InformId == Informid);
                    new BaseDAL<Inform>().Del(inform);
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
            rowHeader.CreateCell(0, CellType.STRING).SetCellValue("通知信息");//创建第一行第一列的单元格，设定里面的值为ID
            rowHeader.CreateCell(1, CellType.STRING).SetCellValue("是否显示");
            rowHeader.CreateCell(2, CellType.STRING).SetCellValue("信息类型");
            List<Inform> list = new BaseDAL<Inform>().GetListBy(u => u.InformId > 0);
            int i = 0;
            if (list.Count > 0)
            {
                foreach (var Inform in list)
                {
                    IRow rowDate = sheet1.CreateRow(i + 1);
                    rowDate.CreateCell(0, CellType.STRING).SetCellValue(Inform.InformText.ToString());
                    rowDate.CreateCell(1, CellType.STRING).SetCellValue(Inform.IsVisible.ToString());
                    rowDate.CreateCell(2, CellType.STRING).SetCellValue(Inform.InfoType.ToString());
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