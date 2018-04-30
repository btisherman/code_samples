using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using TVSM.Models;
using System.Text;

namespace TVSM.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")
            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());

        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")
            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());
        }

        [HttpPost]
        public ActionResult Dashboard(string[] selectedSiteCodes)
        {
			// Create Session variable:
			System.Web.HttpContext.Current.Session["SelectedSiteCodes"] = selectedSiteCodes;
			ViewData["SelectedSiteCodes"] = System.Web.HttpContext.Current.Session["SelectedSiteCodes"] as String;

			List<SiteCode> allCust = new List<SiteCode>();

            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

            });

            // In the real application you can ids 

            int count = selectedSiteCodes.Length;

            ViewBag.NumberOfSelectedSiteCodes = "From DashController " + count;


            SiteCode selectedSiteCode = new SiteCode();

            List<SiteCode> listSelectedSiteCodes = new List<SiteCode>();

            // Get BCA_Name:
            StringBuilder sb = new StringBuilder();


            for (int i = 0; i < count; i++)
            {

                selectedSiteCode.Site_Code = selectedSiteCodes[i].ToString();
                selectedSiteCode.BCA_Name = getSiteNameFromSiteCode(selectedSiteCode.Site_Code);
                listSelectedSiteCodes.Add(selectedSiteCode);
                sb.Append(selectedSiteCode.BCA_Name);
                sb.AppendLine();

            }

            ViewBag.BCA_Name = sb.ToString();

            if (selectedSiteCodes != null)
            {
                ViewBag.Message = "Selected Site Codes: " + string.Join(", ", selectedSiteCodes);
                //ViewBag.Message = "Selected Site Codes: " + string.Join(", ", selectedSiteCode.BCA_Name);
            }
            else
            {
                ViewBag.Message = "No Site Code selected";
            }

            allCust = siteCodeList.ToList();

            //return View(allCust);

            return View(selectedSiteCodes);
        }

        [HttpGet]
        public ActionResult SiteCodePartialView()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                //NameSiteCode = DataRow.Field<string>("NameSiteCode"),
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());
        }




        public ActionResult testIndex()
        {

            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")
            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());

        }


        public ActionResult HomePage()
        {

            return View();

        }

        public ActionResult Release()
        {

            return View();

        }



        [HttpPost]
        public string Index(IEnumerable<string> SelectedSiteCodes)
        {

            if (SelectedSiteCodes == null)
            {
                return "No Site Code selected";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("You selected : " + string.Join(", ", SelectedSiteCodes));

                return sb.ToString();
            }
        }


        public ActionResult Layout_Index()
        {
            ViewBag.Message = "Layout Index";

            return View();
        }



        public ActionResult About()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                //NameSiteCode = DataRow.Field<string>("NameSiteCode"),
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());
        }





        public ActionResult LinkFromRaphaWebsite()
        {
            ViewBag.Message = "Layout Index";
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                //NameSiteCode = DataRow.Field<string>("NameSiteCode"),
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

            });

            return View(siteCodeList.ToList());

            //return View();
        }

        public ActionResult ModalPopup()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                //NameSiteCode = DataRow.Field<string>("NameSiteCode"),
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

            });

            return View(siteCodeList.ToList());
        }

        public ActionResult TestPartialView()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

                //NameSiteCode = DataRow.Field<string>("NameSiteCode")
            });

            return View(siteCodeList.ToList());

        }


        [HttpPost]
        public string TestPartialView(IEnumerable<string> SelectedSiteCodes)
        {
            if (SelectedSiteCodes == null)
            {
                return "No cities selected";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("You selected : " + string.Join(", ", SelectedSiteCodes));

                return sb.ToString();
            }
        }


        public ActionResult SelectSiteCode()
        {
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                //NameSiteCode = DataRow.Field<string>("NameSiteCode"),
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")
            });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            return View(siteCodeList.ToList());
        }


        public ActionResult TestSelectAll()
        {

            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code")

                //NameSiteCode = DataRow.Field<string>("NameSiteCode")
            });

            return View(siteCodeList.ToList());
        }



        public string getSiteNameFromSiteCode(string SiteCode)
        {
            string SiteName = string.Empty;
            var dal = new TVSM_DAL();
            var sc = new SiteCode();

            SqlParameter[] parameters = sc.GetSiteNameFromSiteCodeParameter();
            parameters[0].Value = SiteCode;

            SqlDataReader dr = null;
            dr = dal.GetSqlDataReader(sc.GetSiteNameFromSiteCode(), parameters);
            if (dr.Read())
            {
                SiteCode selectedSiteCode = new SiteCode();

                selectedSiteCode.Site_Code = Request.QueryString["site_code"];
                selectedSiteCode.BCA_Name = dr["BCA_Name"].ToString();
                return selectedSiteCode.BCA_Name;
            }
            else
                return "You did not select site code";
        }

        public ActionResult History()
        {
            return View();
        }


        [HttpGet]
        public ActionResult listAllSites()
        {
            TVSM_DAL dal = new TVSM_DAL();
            SiteCode sc = new SiteCode();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

          
            ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;         

            var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
            {
                BCA_Name = DataRow.Field<string>("BCA_Name"),
                Site_Code = DataRow.Field<string>("Site_Code"),
                Site_Name_Code = DataRow.Field<string>("Site_Name_Code")                
             });

            List<SelectListItem> listSelectListItem = new List<SelectListItem>();
            foreach (SiteCode sitecode in siteCodeList)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = sitecode.Site_Name_Code,
                    Value = sitecode.Site_Code
                };

                listSelectListItem.Add(selectListItem);
            }
          
            SiteViewModel siteViewModel = new SiteViewModel();

            siteViewModel.Sites = listSelectListItem;
			//return View(siteViewModel);
			return PartialView(siteViewModel);
		}



        [HttpPost]
        public string listAllSites(IEnumerable<string> SelectedSites)
        {
            if (SelectedSites == null)
            {
                return "You did not select any Site Name";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("You selected: " + string.Join(",", SelectedSites));
                return sb.ToString();

            }
        }




    }
}