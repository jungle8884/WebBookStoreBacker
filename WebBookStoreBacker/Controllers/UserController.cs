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
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //Get the Main Page
        public ActionResult Main()
        {
            return View();
        }

        //Get the UserManagement page
        public ActionResult UserList()
        {
            ViewBag.Type = Request.QueryString["type"].ToString();
            return View();
        }

        //Get the UserList Data
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public JsonResult GetUserList()
        {
            int type = Convert.ToInt32(Request.Form["iType"].ToString());//type==0为普通用户，type==1为系统管理员
            string sUserName = Request.Params["sUserName"].ToString();
            int pageindex = Convert.ToInt32(Request.Form["page"].ToString());
            int pagesize = Convert.ToInt32(Request.Form["rows"].ToString());

            Expression<Func<User, bool>> sWhere = u => u.UserId > 0 && u.Type == type;      
            
            if (sUserName != "" && sUserName != null)
            {
                sWhere = sWhere.And(u => u.UserName.Contains(sUserName));
            }

            List<User> list = new BaseDAL<User>().GetPagedList(pageindex, pagesize, sWhere, u => u.UserId, false);
            dictionary.Add("total", new BaseDAL<User>().GetListBy(u => u.UserId > 0).Count());
            dictionary.Add("rows", list);
            return Json(dictionary);
        }

        //Add User to DB
        public JsonResult AddUser()
        {
            string UserName = Request.Form["UserName"].ToString();
            string Pwd = Request.Form["Pwd"].ToString();
            string Gender = Request.Form["Gender"].ToString();
            string Email = Request.Form["Email"].ToString();
            string Tel = Request.Form["Tel"].ToString();
            string QQ = Request.Form["QQ"].ToString();
            int Type = 0;//只能添加普通用户
            User user = new User();
            user.UserName = UserName;
            user.Pwd = Pwd;
            user.Gender = Gender;
            user.Email = Email;
            user.Tel = Tel;
            user.QQ = QQ;
            user.Type = Type;
            user.CreatedTime = DateTime.Now;
            try
            {
                if (new BaseDAL<User>().Add(user) > 0)
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

        //Update User to DB
        public JsonResult UpdateUser(int id)
        {
            string UserName = Request.Form["UserName"].ToString();
            string Pwd = Request.Form["Pwd"].ToString();
            string Gender = Request.Form["Gender"].ToString();
            string Email = Request.Form["Email"].ToString();
            string Tel = Request.Form["Tel"].ToString();
            string QQ = Request.Form["QQ"].ToString();
            User user = new BaseDAL<User>().GetModel(u => u.UserId == id);
            user.UserName = UserName;
            user.Pwd = Pwd;
            user.Gender = Gender;
            user.Email = Email;
            user.Tel = Tel;
            user.QQ = QQ;
            if (new BaseDAL<User>().Modify(user) > 0)
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

        //Delete User from DB
        public JsonResult DelUser()
        {
            try
            {
                string sUserids = Request.Form["deluserids"].ToString();
                if (sUserids.Contains("_"))
                {
                    string[] aUserids = sUserids.Split('_');
                    foreach (var id in aUserids)
                    {
                        int userid = Convert.ToInt32(id);
                        User user = new BaseDAL<User>().GetModel(u => u.UserId == userid);
                        new BaseDAL<User>().Del(user);
                    }
                }
                else
                {
                    int userid = Convert.ToInt32(sUserids);
                    User user = new BaseDAL<User>().GetModel(u => u.UserId == userid);
                    new BaseDAL<User>().Del(user);
                }
                dictionary.Add("success", true);
            }
            catch 
            {
                dictionary.Add("success", false);
            }
            return Json(dictionary);
        }

        //Check User is or not logined
        public string Login()
        {
            string username = Request.Form["username"];
            string pwd = Request.Form["pwd"];
            User user = new BaseDAL<User>().GetModel(u => u.UserName == username && u.Pwd == pwd);
            if (user != null)
            {
                if (user.Type != 1)
                {
                    return "没有操作权限";
                }
                else
                {
                    HttpCookie c_username = new HttpCookie("username", username);
                    Response.Cookies.Add(c_username);
                    return "登陆成功";
                }
            }
            else
            {
                return "登陆失败";
            }
        }

        //Export Data to Excel
        public string GetExcel()
        {
            string filename = "";
            HSSFWorkbook workbook = new HSSFWorkbook();//创建一个工作薄
            ISheet sheet1 = workbook.CreateSheet();//创建一个sheet页
            IRow rowHeader = sheet1.CreateRow(0);//创建第一行
            rowHeader.CreateCell(0, CellType.STRING).SetCellValue("用户名ID");//创建第一行第一列的单元格，设定里面的值为ID
            rowHeader.CreateCell(1, CellType.STRING).SetCellValue("用户名");
            rowHeader.CreateCell(2, CellType.STRING).SetCellValue("密码");
            rowHeader.CreateCell(3, CellType.STRING).SetCellValue("性别");
            rowHeader.CreateCell(4, CellType.STRING).SetCellValue("Email");
            rowHeader.CreateCell(5, CellType.STRING).SetCellValue("手机号");
            rowHeader.CreateCell(6, CellType.STRING).SetCellValue("QQ");
            rowHeader.CreateCell(7, CellType.STRING).SetCellValue("用户类型");
            rowHeader.CreateCell(8, CellType.STRING).SetCellValue("创建时间");
            List<User> list = new BaseDAL<User>().GetListBy(u => u.UserId > 0);
            int i = 0;
            if (list.Count > 0)
            {
                foreach (var user in list)
                {
                    IRow rowDate = sheet1.CreateRow(i + 1);
                    rowDate.CreateCell(0, CellType.STRING).SetCellValue(user.UserId.ToString());
                    rowDate.CreateCell(1, CellType.STRING).SetCellValue(user.UserName);
                    rowDate.CreateCell(2, CellType.STRING).SetCellValue(user.Pwd);
                    rowDate.CreateCell(3, CellType.STRING).SetCellValue(user.Gender);
                    rowDate.CreateCell(4, CellType.STRING).SetCellValue(user.Email);
                    rowDate.CreateCell(5, CellType.STRING).SetCellValue(user.Tel);
                    rowDate.CreateCell(6, CellType.STRING).SetCellValue(user.QQ);
                    rowDate.CreateCell(7, CellType.STRING).SetCellValue(user.Type.ToString());
                    rowDate.CreateCell(8, CellType.STRING).SetCellValue(user.CreatedTime.ToString());
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

        //Analyse the User's subscription or sign up
        public ActionResult GetReport()
        {
            return View();
        }

        //Get Pie Chart
        public JsonResult GetPie()
        {
            DateTime startTime = Convert.ToDateTime(Request.Form["startTime"].ToString());
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"].ToString());
            List<User> list = new BaseDAL<User>().GetListBy(u => u.CreatedTime >= startTime && u.CreatedTime <= endTime && u.Type == 0);
            var listGender = (from u in list
                         group u by u.Gender into g
                         select new
                         {
                             key = g.Key,
                             GenderCount = g.Count()
                         }).ToList();
            return Json(listGender);

        }

        //Get Graph Chart
        public JsonResult GetGraph()
        {
            DateTime startTime = Convert.ToDateTime(Request.Form["startTime"].ToString());
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"].ToString());
            List<User> list = new BaseDAL<User>().GetListBy(u => u.CreatedTime >= startTime && u.CreatedTime <= endTime && u.Type == 0);
            var listGraph = (from u in list
                             group u by u.CreatedTime.Value.Year.ToString() + u.CreatedTime.Value.Month.ToString() into g
                             orderby Convert.ToInt32(g.Key.Substring(0, 4)), Convert.ToInt32(g.Key.Substring(4, g.Key.Length - 4))
                             select new
                             {
                                 key = g.Key,
                                 count = g.Count()
                             }).ToList();
            Dictionary<string, object> dt = new Dictionary<string, object>();
            //   JavaScriptSerializer jss = new JavaScriptSerializer();
            List<string> aTime = new List<string>();
            List<int> aCountGraph = new List<int>();
            foreach (var a in listGraph)
            {
                aTime.Add(a.key);
                aCountGraph.Add(a.count);
            }
            dt.Add("a1", aTime);
            dt.Add("a2", aCountGraph);
            //return jss.Serialize(dt);
            return Json(dt);
        }
    }
}