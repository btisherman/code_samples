using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TVSM.Models;
using System.Web.UI;
using System.Drawing;


namespace TVSM.Controllers
{
	public class DashboardController : Controller   {

		TVSM_DAL dal = new TVSM_DAL();
		// GET: Dashboard
		public ActionResult Index()
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			try
			{
				dashboardViewModel = FirstLoad(Session["FirstSelectedSiteCode"].ToString());//access 2	
				return View(dashboardViewModel);
			}
			catch
			{
				return RedirectToAction("HomePage", "Home");
			}
			//return View();
		}
		[HttpPost]
		public ActionResult LoadSchedulePerformanceData(string siteCode)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.GetSchedulePerformanceBySiteCodeParameter();
			parameters[0].Value = siteCode;

			ds = dal.GetDataSet(tvsm.GetSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
			DataTable data = ds.Tables[0];
			tbl = ds.Tables[0];
			//return Json(new { data = data }, JsonRequestBehavior.AllowGet);
			return Json(new { success = true, message = "LoadSchedulePerformanceData successfully" }, JsonRequestBehavior.AllowGet);
			//return View();
		}

		#region FirstLoad
		protected DashboardViewModel FirstLoad(string siteCode)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			if (Session["FirstSelectedSiteCode"].ToString() != null)
			{
				dashboardViewModel.FirstSelectedSiteCode = Session["FirstSelectedSiteCode"].ToString();
				ViewBag.FirstSiteName = getSiteNameFromSiteCode(dashboardViewModel.FirstSelectedSiteCode);

				//Determine SchedulePerformance:
				int SchedulePerformanceNumerator = getSchedulePerformanceNumerator(dashboardViewModel.FirstSelectedSiteCode);
				int SchedulePerformanceDenominator = getSchedulePerformanceDenominator(dashboardViewModel.FirstSelectedSiteCode);
				dashboardViewModel.scheduleScore = RoudUpValidation(SchedulePerformanceNumerator, SchedulePerformanceDenominator);
				dashboardViewModel.scheduleImgPath = getImagePath(dashboardViewModel.scheduleScore);

				//Determine Quality Performance:
				int qaPerformanceNumerator = getQaPerformanceNumerator(dashboardViewModel.FirstSelectedSiteCode);
				int qaPerformanceDenominator = getQaPerformanceDenominator(dashboardViewModel.FirstSelectedSiteCode);
				dashboardViewModel.qualityScore = RoudUpValidation(qaPerformanceNumerator, qaPerformanceDenominator);
				dashboardViewModel.qualityImgPath = getImagePath(dashboardViewModel.qualityScore);

				//Determine Cost Performance:
				dashboardViewModel.costScore = getCostPerformanceData(dashboardViewModel.FirstSelectedSiteCode);
				dashboardViewModel.costImgPath = getImagePath(dashboardViewModel.costScore);

				// Determine Overall Performance:
				int overallDonutNum = RoudUpValidation(dashboardViewModel.scheduleScore + dashboardViewModel.qualityScore + dashboardViewModel.costScore, 30);
				dashboardViewModel.overallImgPath = getImagePath(overallDonutNum);
				dashboardViewModel.listProgramViewModel = getProgramsBySiteCode(dashboardViewModel.FirstSelectedSiteCode);

				HttpContext.Session["listProgram"] = new List<ProgramViewModel> { new ProgramViewModel(), new ProgramViewModel() };
				HttpContext.Session["listProgram"] = dashboardViewModel.listProgramViewModel;


				ProgramViewModel programViewModel = new ProgramViewModel();

				UserViewModel userViewModel = new UserViewModel();

				int bemsid = 1747057;

				dashboardViewModel.UserName = getUsernameByBEMSID(bemsid);

				// Save Session Variable:
				System.Web.HttpContext.Current.Session["UserName"] = dashboardViewModel.UserName;
				ViewData["UserName"] = System.Web.HttpContext.Current.Session["UserName"] as String;

				if (dashboardViewModel.UserName == "")
				{
					dashboardViewModel.UserName = "Guest";
				}
			}
			return dashboardViewModel;
		}
		#endregion FirstLoad
		
		[HttpPost]
		public ActionResult Index(IEnumerable<string> SelectedSites)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			
			if (SelectedSites == null)
			{
				return null;
			}
			else
			{
				ViewBag.SelectedSites = SelectedSites;
				
				dashboardViewModel.SelectedSiteCodes = ViewBag.SelectedSites;
				dashboardViewModel.FirstSelectedSiteCode = dashboardViewModel.SelectedSiteCodes[0];

				System.Web.HttpContext.Current.Session["FirstSelectedSiteCode"] = dashboardViewModel.FirstSelectedSiteCode;

				//Determine SchedulePerformance:
				int SchedulePerformanceNumerator = getSchedulePerformanceNumerator(dashboardViewModel.FirstSelectedSiteCode);
				int SchedulePerformanceDenominator = getSchedulePerformanceDenominator(dashboardViewModel.FirstSelectedSiteCode);
				int scheduleNum = RoudUpValidation(SchedulePerformanceNumerator, SchedulePerformanceDenominator);
				dashboardViewModel.scheduleImgPath = getImagePath(scheduleNum);

				//Determine Quality Performance:
				int qaPerformanceNumerator = getQaPerformanceNumerator(dashboardViewModel.FirstSelectedSiteCode);
				int qaPerformanceDenominator = getQaPerformanceDenominator(dashboardViewModel.FirstSelectedSiteCode);
				int qaDonutNum = RoudUpValidation(qaPerformanceNumerator, qaPerformanceDenominator);
				dashboardViewModel.qualityImgPath = getImagePath(qaDonutNum);

				//Determine Cost Performance:
				int costDonutNum = getCostPerformanceData(dashboardViewModel.FirstSelectedSiteCode);
				dashboardViewModel.costImgPath = getImagePath(costDonutNum);

				// Determine Overall Performance:
				int overallDonutNum = RoudUpValidation(scheduleNum + qaDonutNum + costDonutNum, 30);
				dashboardViewModel.overallImgPath = getImagePath(overallDonutNum);

				if (ViewData["SelectedSiteCodes"] == null)
				{
					dashboardViewModel.listProgramViewModel = getProgramsBySiteCode(ViewBag.SelectedSites[0]);

					HttpContext.Session["listProgram"] = new List<ProgramViewModel> { new ProgramViewModel(), new ProgramViewModel() };
					HttpContext.Session["listProgram"] = dashboardViewModel.listProgramViewModel;
				}			

				ProgramViewModel programViewModel = new ProgramViewModel();

				UserViewModel userViewModel = new UserViewModel();

				int bemsid = 1747057;

				dashboardViewModel.UserName = getUsernameByBEMSID(bemsid);

				// Save Session Variable:
				System.Web.HttpContext.Current.Session["UserName"] = dashboardViewModel.UserName;
				ViewData["UserName"] = System.Web.HttpContext.Current.Session["UserName"] as String;

				if (dashboardViewModel.UserName == "")
				{
					dashboardViewModel.UserName = "Guest";
				}

				string[] selectedSiteCodes = SelectedSites.ToString().Split(',');

				StringBuilder sb = new StringBuilder();
				string selectedSiteNames = string.Empty;
				foreach (var sc in SelectedSites)
				{
					sb.Append(getSiteNameFromSiteCode(sc) + ", ");
				}
				
				ViewBag.selectedSiteNames = sb.ToString().Trim().TrimEnd(',');
				dashboardViewModel.SelectedSiteNames = ViewBag.selectedSiteNames;
				string program = dashboardViewModel.SelectedProgram;
				if (dashboardViewModel != null)
				{
					return View(dashboardViewModel);
				}
				else if (dashboardViewModel == null)
				{
					return View(dashboardViewModel);
				}

				return View();

			}
		}
		
		protected int getCostPerformanceData(string costSiteCode)
		{
			int returnInt = 0;
			double cost_round;

			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] costparameters = tvsm.GetCostPerformanceBySiteCodeParameter();
			costparameters[0].Value = costSiteCode;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetCostPerformanceBySiteCode(), costparameters);

			if (dr.Read())
			{
				int total_estimate = Convert.ToInt32(dr["Sum_Total_Est"].ToString());
				int difference_estimate = Convert.ToInt32(dr["Difference_Est"].ToString());
				difference_estimate = Math.Abs(difference_estimate);
				cost_round = (double)difference_estimate/(double)total_estimate;
				cost_round = Math.Abs(1 - cost_round);
				cost_round = cost_round * 10;

				if (cost_round < 1)
				{
					returnInt = 1;
				}
				else if (cost_round > 10)
				{
					returnInt = 10;
				}
				else
				{
					returnInt = (int)Math.Round(cost_round, 0);
				}
			}

			return returnInt;			
		}
		protected int RoudUpValidation(int numerator, int denominator)
		{
			int returnInt;

			if (numerator == 0 || numerator < 0)
			{
				returnInt = 0;
			}
			else
			{
				double roundValue = (double)numerator / denominator * 10;
							
				if (roundValue < 1)
				{
					returnInt = 1;
				}
				else if (roundValue > 10)
				{
					returnInt = 10;
				}
				else
				{
					returnInt = (int)Math.Round(roundValue, 0);
				}
			}

			return returnInt;

		}
		protected int getQaPerformanceNumerator(string siteCode)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.GetQualityPerformanceNumeratorBySiteCodeParameter();
			parameters[0].Value = siteCode;

			ds = dal.GetDataSet(tvsm.GetQualityPerformanceNumeratorBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;

			int numerator = ds.Tables[0].Rows.Count;

			return numerator;
		}
		protected int getQaPerformanceDenominator(string siteCode)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.GetQualityPerformanceDenominatorBySiteCodeParameter();
			parameters[0].Value = siteCode;

			ds = dal.GetDataSet(tvsm.GetQualityPerformanceDenominatorBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}

		protected string getImagePath(int donutNum)
		{
			string ImageSrc=string.Empty;

			switch (donutNum)
			{
				case 0:
					ImageSrc = "/Images/NoData.gif";
					break;
				case 1:
					ImageSrc = "/Images/gauge1.gif";
					break;
				case 2:
					ImageSrc = "/Images/gauge2.gif";
					break;
				case 3:
					ImageSrc = "/Images/gauge3.gif";
					break;
				case 4:
					ImageSrc = "/Images/gauge4.gif";
					break;
				case 5:
					ImageSrc = "/Images/gauge5.gif";
					break;
				case 6:
					ImageSrc = "/Images/gauge6.gif";
					break;
				case 7:
					ImageSrc = "/Images/gauge7.gif";
					break;
				case 8:
					ImageSrc = "/Images/gauge8.gif";
					break;
				case 9:
					ImageSrc = "/Images/gauge9.gif";
					break;
				case 10:
					ImageSrc = "/Images/gauge10.gif";
					break;				
			}

			return ImageSrc;
			
		}
		protected int getSchedulePerformanceNumerator(string siteCode)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();
			SqlParameter[] parameters = tvsm.GetSiteCodeSchedulePerformanceNumeratorParameter();
			parameters[0].Value = siteCode;
			ds = dal.GetDataSet(tvsm.GetSiteCodeSchedulePerformanceNumerator(), parameters, "tbl").Tables["tbl"].DataSet;

			int numerator = ds.Tables[0].Rows.Count;

			return numerator;

		}

		protected int getSchedulePerformanceDenominator(string siteCode)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.GetSiteCodeSchedulePerformanceDenominatorParameter();
			parameters[0].Value = siteCode;

			ds = dal.GetDataSet(tvsm.GetSiteCodeSchedulePerformanceDenominator(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}
		public List<ProgramViewModel> getProgramsBySiteCode(string selectedSiteCode)
		{
			if (selectedSiteCode == null)
			{
				selectedSiteCode = System.Web.HttpContext.Current.Session["SelectedSiteCodes"] as String;
			}
			List<ProgramViewModel> list = new List<ProgramViewModel>();
			var pg = new ProgramViewModel();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			SqlParameter[] parameters = pg.GetProgramBySiteCodeParameter();
			parameters[0].Value = selectedSiteCode;
			ds = dal.GetDataSet(pg.GetProgramBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
			var programList = ds.Tables[0].AsEnumerable().Select(DataRow => new ProgramViewModel
			{
				Program = DataRow.Field<string>("Program")
			});
			return programList.ToList();
		}
		public JsonResult GetPST(string program)
		{
			System.Web.HttpContext.Current.Session["selectedProgram"] = program;		

			List <SelectListItem> pstList = new List<SelectListItem>();		
			var tvsm = new TvsmViewModel();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = tvsm.GetAreaByProgramParameter();
			parameters[0].Value = program;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetAreaByProgram(), parameters);
			while (dr.Read())
			{
				pstList.Add(new SelectListItem
				{
					Text = dr["PST"].ToString(),
					Value = dr["PST"].ToString()
				});			
				}

			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			dashboardViewModel = GetDonutChartByProgram(program);
			
			pstList.Add(new SelectListItem
			{
				Text = "schedule",
				Value = dashboardViewModel.scheduleImgPath
			});

			pstList.Add(new SelectListItem
			{
				Text = "quality",
				Value = dashboardViewModel.qualityImgPath
			});

			pstList.Add(new SelectListItem
			{
				Text = "cost",
				Value = dashboardViewModel.costImgPath
			});

			pstList.Add(new SelectListItem
			{
				Text = "overall",
				Value = dashboardViewModel.overallImgPath
			});			

			return Json(new SelectList(pstList, "Value", "Text"));
		}

		public DashboardViewModel GetDonutChartByProgram(string program)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			string selectedSiteCode = Session["FirstSelectedSiteCode"].ToString();
			string selectedProgram = Session["selectedProgram"].ToString();		

			// Determine SchedulePerformance:
			int SchedulePerformanceNumerator = getSchedulePerformanceNumeratorByProgram(selectedSiteCode, program);
			int SchedulePerformanceDenominator = getSchedulePerformanceDenominatorByProgram(selectedSiteCode, program);
			int scheduleNum = RoudUpValidation(SchedulePerformanceNumerator, SchedulePerformanceDenominator);
			dashboardViewModel.scheduleImgPath = getImagePath(scheduleNum);

			// Determine Quality Performance:
			int QualityPerformanceNumerator = getQualityPerformanceNumeratorByProgram(selectedSiteCode, program);
			int QualityPerformanceDenominator = getQualityPerformanceDenominatorByProgram(selectedSiteCode, program);
			int qualityNum = RoudUpValidation(QualityPerformanceNumerator, QualityPerformanceDenominator);
			dashboardViewModel.qualityImgPath = getImagePath(qualityNum);

			// Determine Cost Performance:
			int costNum = getCostPerformanceByProgram(selectedSiteCode, program);
			dashboardViewModel.costImgPath = getImagePath(costNum);

			// Determine Overall Performance:
			//int overallDonutNum = RoudUpValidation(scheduleNum + qualityNum + costNum, 30);
			int overallDonutNum = OverCalculation(scheduleNum, qualityNum, costNum);
			dashboardViewModel.overallImgPath = getImagePath(overallDonutNum);				

			return dashboardViewModel;
		}
		protected int getSchedulePerformanceNumeratorByProgram(string siteCode, string program)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceNumeratorByProgramParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceNumeratorByProgram(), parameters, "tbl").Tables["tbl"].DataSet;

			int numerator = ds.Tables[0].Rows.Count;

			return numerator;			
		}

		protected int getSchedulePerformanceDenominatorByProgram(string siteCode, string program)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceDenominatorByProgramParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceDenominatorByProgram(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}

		protected int getQualityPerformanceNumeratorByProgram(string siteCode, string program)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.QualityPerformanceNumeratorByProgramParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;

			ds = dal.GetDataSet(tvsm.QualityPerformanceNumeratorByProgram(), parameters, "tbl").Tables["tbl"].DataSet;

			int numerator = ds.Tables[0].Rows.Count;

			return numerator;
		}

		protected int getQualityPerformanceDenominatorByProgram(string siteCode, string program)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceDenominatorByProgramParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;

			ds = dal.GetDataSet(tvsm.QualityPerformanceDenominatorByProgram(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}

		protected int getCostPerformanceByProgram(string siteCode, string program)
		{
			int returnInt = 0;
			double cost_round;

			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] costparameters = tvsm.GetCostPerformanceByProgramParameter();
			costparameters[0].Value = siteCode;
			costparameters[1].Value = program;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetCostPerformanceByProgram(), costparameters);

			if (dr.Read())
			{
				int total_estimate = Convert.ToInt32(dr["Sum_Total_Est"].ToString());
				int difference_estimate = Convert.ToInt32(dr["Difference_Est"].ToString());
				difference_estimate = Math.Abs(difference_estimate);
				cost_round = (double)difference_estimate / (double)total_estimate;
				cost_round = Math.Abs(1 - cost_round);
				cost_round = cost_round * 10;

				if (cost_round < 1)
				{
					returnInt = 1;
				}
				else if (cost_round > 10)
				{
					returnInt = 10;
				}
				else
				{
					returnInt = (int)Math.Round(cost_round, 0);
				}
			}

			return returnInt;
		}

		public JsonResult GetACCPbyPST(string pst)
		{
			List<SelectListItem> accpList = new List<SelectListItem>();

			var tvsm = new TvsmViewModel();

			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = tvsm.GetACCPbyPSTParameter();
			parameters[0].Value = pst;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetACCPbyPST(), parameters);
			while (dr.Read())
			{
				accpList.Add(new SelectListItem
				{
					Text = dr["accp"].ToString(),
					Value = dr["accp"].ToString()
				});
			}

			return Json(new SelectList(accpList, "Value", "Text"));
		}

		public JsonResult GetProjectByACCP(string accp)
		{
			List<SelectListItem> projectList = new List<SelectListItem>();

			var tvsm = new TvsmViewModel();

			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = tvsm.GetProjectByACCPParameter();
			parameters[0].Value = accp;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetProjectByACCP(), parameters);
			while (dr.Read())
			{
				projectList.Add(new SelectListItem
				{
					Text = dr["project"].ToString(),
					Value = dr["project"].ToString()
				});
			}

			return Json(new SelectList(projectList, "Value", "Text"));
		}

		public JsonResult GetMesStatusByProject(string project)
		{
			List<SelectListItem> mesList = new List<SelectListItem>();

			var tvsm = new TvsmViewModel();

			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = tvsm.GetMES_StatusByProjectParameter();
			parameters[0].Value = project;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetMES_StatusByProject(), parameters);
			while (dr.Read())
			{
				mesList.Add(new SelectListItem
				{
					Text = dr["mes_status"].ToString(),
					Value = dr["mes_status"].ToString()
				});
			}

			return Json(new SelectList(mesList, "Value", "Text"));
		}
		public JsonResult GetAllFormTypeList()
		{
			List<SelectListItem> formTypeList = new List<SelectListItem>();

			formTypeList.Add(new SelectListItem { Value = "DESIGN TDR STDR", Text = "DESIGN" });
			formTypeList.Add(new SelectListItem { Value = "MFG TF STF", Text = "MFG" });
			formTypeList.Add(new SelectListItem { Value = "DETAIL COMPONENT", Text = "DETAIL COMPONENT" });

			return Json(new SelectList(formTypeList, "Value", "Text"));

		}

		public JsonResult GetDonutChartByDateDiff(int days)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			string siteCode = Session["FirstSelectedSiteCode"].ToString();
			string program = Session["selectedProgram"].ToString();
			string workArea = Session["workArea"].ToString();

			// Determine SchedulePerformance:
			int SchedulePerformanceNumerator = getSchedulePerformanceNumeratorByDateDiff(siteCode, program, workArea,days);
			int SchedulePerformanceDenominator = getSchedulePerformanceDenominatorByDateDiff(siteCode, program, workArea, days);
			int scheduleNum = RoudUpValidation(SchedulePerformanceNumerator, SchedulePerformanceDenominator);
			dashboardViewModel.scheduleImgPath = getImagePath(scheduleNum);

			// Determine Quality Performance:
			int QualityPerformanceNumerator = getQualityPerformanceNumeratorByDateDiff(siteCode, program, workArea, days);
			int QualityPerformanceDenominator = getQualityPerformanceDenominatorByDateDiff(siteCode, program, workArea, days);
			int qualityNum = RoudUpValidation(QualityPerformanceNumerator, QualityPerformanceDenominator);
			dashboardViewModel.qualityImgPath = getImagePath(qualityNum);

			// Determine Cost Performance:
			int costNum = getCostPerformanceByDataRange(siteCode, program,workArea, days);
			dashboardViewModel.costImgPath = getImagePath(costNum);

			// Determine Overall Performance:
			int overallDonutNum = OverCalculation(scheduleNum, qualityNum, costNum);
			dashboardViewModel.overallImgPath = getImagePath(overallDonutNum);

			// Bind imgPath to donutList:
			List<SelectListItem> donutList = new List<SelectListItem>();
			donutList.Add(new SelectListItem { Value = dashboardViewModel.scheduleImgPath, Text = "schedule" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.qualityImgPath, Text = "quality" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.costImgPath, Text = "cost" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.overallImgPath, Text = "overall" });

			return Json(new SelectList(donutList, "Value", "Text"));
		}

		public JsonResult GetDonutListByProgram(string program)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			string siteCode = Session["FirstSelectedSiteCode"].ToString();
			System.Web.HttpContext.Current.Session["selectedProgram"] = program;
			
			dashboardViewModel = GetDonutChartByProgram(program);
			// Bind imgPath to donutList:
			List<SelectListItem> donutList = new List<SelectListItem>();
			donutList.Add(new SelectListItem { Value = dashboardViewModel.scheduleImgPath, Text = "schedule" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.qualityImgPath, Text = "quality" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.costImgPath, Text = "cost" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.overallImgPath, Text = "overall" });

			return Json(new SelectList(donutList, "Value", "Text"));
		}

		public JsonResult GetDonutListByArea(string area)
		{
			System.Web.HttpContext.Current.Session["workArea"] = area;
			DashboardViewModel dashboardViewModel = new DashboardViewModel();
			//string area = Session["workArea"].ToString();
			string siteCode = Session["FirstSelectedSiteCode"].ToString();
			string program = Session["selectedProgram"].ToString();
		

			dashboardViewModel = GetDonutChartByWorkArea(area);
			// Bind imgPath to donutList:
			List<SelectListItem> donutList = new List<SelectListItem>();
			donutList.Add(new SelectListItem { Value = dashboardViewModel.scheduleImgPath, Text = "schedule" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.qualityImgPath, Text = "quality" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.costImgPath, Text = "cost" });
			donutList.Add(new SelectListItem { Value = dashboardViewModel.overallImgPath, Text = "overall" });

			return Json(new SelectList(donutList, "Value", "Text"));
		}

		protected int getSchedulePerformanceNumeratorByDateDiff(string siteCode, string program, string area, int days)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceNumeratorByDataRangeParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;
			parameters[3].Value = days;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceNumeratorByDataRange(), parameters, "tbl").Tables["tbl"].DataSet;
			int numerator = ds.Tables[0].Rows.Count;

			return numerator;
		}

		protected int getSchedulePerformanceDenominatorByDateDiff(string siteCode, string program, string area, int days)
		{
			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceDenominatorByDataRangeParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;
			parameters[3].Value = days;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceDenominatorByDataRange(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}

		protected int getQualityPerformanceNumeratorByDateDiff(string siteCode, string program, string area, int days)
		{
			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.QualityPerformanceNumeratorByDataRangeParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;
			parameters[3].Value = days;

			ds = dal.GetDataSet(tvsm.QualityPerformanceNumeratorByDataRange(), parameters, "tbl").Tables["tbl"].DataSet;
			int numerator = ds.Tables[0].Rows.Count;

			return numerator;
		}

		protected int getQualityPerformanceDenominatorByDateDiff(string siteCode, string program, string area, int days)
		{
			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.QualityPerformanceDenominatorByDataRangeParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;
			parameters[3].Value = days;

			ds = dal.GetDataSet(tvsm.QualityPerformanceDenominatorByDataRange(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}

		protected int getCostPerformanceByDataRange(string siteCode, string program, string area, int days)
		{
			int returnInt = 0;
			double cost_round;		
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			try
			{
				SqlParameter[] costparameters = tvsm.CostPerformanceByWorkAreaParameter();
				costparameters[0].Value = siteCode;
				costparameters[1].Value = program;
				costparameters[2].Value = area;
				costparameters[3].Value = days;

				SqlDataReader dr = null;
				dr = dal.GetSqlDataReader(tvsm.CostPerformanceByWorkArea(), costparameters);
				int total_estimate = Convert.ToInt32(dr["Sum_Total_Est"].ToString());
				int difference_estimate = Convert.ToInt32(dr["Difference_Est"].ToString());
				difference_estimate = Math.Abs(difference_estimate);
				cost_round = (double)difference_estimate / (double)total_estimate;
				cost_round = Math.Abs(1 - cost_round);
				cost_round = cost_round * 10;
				if (cost_round < 1)
				{
					returnInt = 1;
				}
				else if (cost_round > 10)
				{
					returnInt = 10;
				}
				else
				{
					returnInt = (int)Math.Round(cost_round, 0);
				}
			}
			catch (Exception ex)
			{
				returnInt = 0;
			}

			return returnInt;
		}

		public JsonResult GetWorkAreaByFormType(string formType)
		{
			List<SelectListItem> workAreaList = new List<SelectListItem>();

			var tvsm = new TvsmViewModel();

			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			if (formType == "DESIGN TDR STDR")
			{
				ds = dal.NoParameterGetDataSet(tvsm.GetWorkAreaByFormTypeDESIGN(), "tbl").Tables["tbl"].DataSet;
			}
			else if (formType == "MFG TF STF")
			{
				ds = dal.NoParameterGetDataSet(tvsm.GetWorkAreaByFormTypeMFG(), "tbl").Tables["tbl"].DataSet;
			}
			else if (formType == "DETAIL COMPONENT")
			{
				ds = dal.NoParameterGetDataSet(tvsm.GetWorkAreaByFormTypeDETAIL(), "tbl").Tables["tbl"].DataSet;
			}		

			foreach (DataRow row in ds.Tables[0].Rows)
			{
				workAreaList.Add(new SelectListItem()
				{
					Text = row["WorkArea"].ToString(),
					Value = row["WorkArea"].ToString()
				});
			}

			return Json(new SelectList(workAreaList, "Value", "Text"));
		}

		public JsonResult GetCCVbyWorkArea(string WorkArea)
		{
			System.Web.HttpContext.Current.Session["workArea"] = WorkArea;
			List<SelectListItem> ccvList = new List<SelectListItem>();
			
			var tvsm = new TvsmViewModel();

			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = tvsm.GetCCVbyWorkAreaParameter();
			parameters[0].Value = WorkArea;

			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(tvsm.GetCCVbyWorkArea(), parameters);
			while (dr.Read())
			{
				ccvList.Add(new SelectListItem
				{
					Text = dr["ccv"].ToString(),
					Value = dr["ccv"].ToString()
				});
			}		

			return Json(new SelectList(ccvList, "Value", "Text"));
		}

		protected DashboardViewModel GetDonutChartByWorkArea(string area)
		{
			DashboardViewModel dashboardViewModel = new DashboardViewModel();

			string selectedSiteCode = Session["FirstSelectedSiteCode"].ToString();
			string program = Session["selectedProgram"].ToString();

			// Determine Schedule Performance:
			int SchedulePerformanceNumerator = getSchedulePerformanceNumeratorByWorkArea(selectedSiteCode, program, area);
			int SchedulePerformanceDenominator = getSchedulePerformanceDenominatorByWorkArea(selectedSiteCode, program, area);
			int scheduleNum = RoudUpValidation(SchedulePerformanceNumerator, SchedulePerformanceDenominator);
			dashboardViewModel.scheduleImgPath = getImagePath(scheduleNum);

			// Determine Quality Performance:
			int QualityPerformanceNumerator = getQualityPerformanceNumeratorByWorkArea(selectedSiteCode, program, area);
			int QualityPerformanceDenominator = getQualityPerformanceDenominatorByWorkArea(selectedSiteCode, program, area);
			int qualityNum = RoudUpValidation(QualityPerformanceNumerator, QualityPerformanceDenominator);
			dashboardViewModel.qualityImgPath = getImagePath(qualityNum);

			// Determine Cost Performance:
			int costNum = getCostPerformanceByArea(selectedSiteCode, program, area);
			dashboardViewModel.costImgPath = getImagePath(costNum);

			// Determine Overall Performance:
			//int overallDonutNum = RoudUpValidation(scheduleNum + qualityNum + costNum, 30);
			int overallDonutNum = OverCalculation(scheduleNum, qualityNum, costNum);
			dashboardViewModel.overallImgPath = getImagePath(overallDonutNum);



			return dashboardViewModel;
		}

		protected int OverCalculation(int scheduleNum, int qualityNum, int costNum)
		{
			int sum = scheduleNum + qualityNum + costNum;
			double OneThird = (double)(sum / 3);
					
			int overallInt = (int)Math.Round(OneThird);

			return overallInt;
		}
		protected int getSchedulePerformanceNumeratorByWorkArea(string siteCode, string program, string area)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceNumeratorByWorkAreaParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceNumeratorByWorkArea(), parameters, "tbl").Tables["tbl"].DataSet;
			int numerator = ds.Tables[0].Rows.Count;
		
			return numerator;
		}
		protected int getSchedulePerformanceDenominatorByWorkArea(string siteCode, string program, string area)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.SchedulePerformanceDenominatorByWorkAreaParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;

			ds = dal.GetDataSet(tvsm.SchedulePerformanceDenominatorByWorkArea(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;			
		}
		protected int getQualityPerformanceNumeratorByWorkArea(string siteCode, string program, string area)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.QualityPerformanceNumeratorByWorkAreaParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;

			ds = dal.GetDataSet(tvsm.QualityPerformanceNumeratorByWorkArea(), parameters, "tbl").Tables["tbl"].DataSet;
			int numerator = ds.Tables[0].Rows.Count;

			return numerator;
		}
		protected int getQualityPerformanceDenominatorByWorkArea(string siteCode, string program, string area)
		{
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			SqlParameter[] parameters = tvsm.QualityPerformanceDenominatorByWorkAreaParameter();
			parameters[0].Value = siteCode;
			parameters[1].Value = program;
			parameters[2].Value = area;

			ds = dal.GetDataSet(tvsm.QualityPerformanceDenominatorByWorkArea(), parameters, "tbl").Tables["tbl"].DataSet;

			int denominator = ds.Tables[0].Rows.Count;

			return denominator;
		}
		protected int getCostPerformanceByArea(string siteCode, string program, string area)
		{
			int returnInt = 0;
			double cost_round;

			//TVSM_DAL dal = new TVSM_DAL();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();
			TvsmViewModel tvsm = new TvsmViewModel();

			try
			{
				SqlParameter[] costparameters = tvsm.CostPerformanceByWorkAreaParameter();
				costparameters[0].Value = siteCode;
				costparameters[1].Value = program;
				costparameters[2].Value = area;

				SqlDataReader dr = null;
				dr = dal.GetSqlDataReader(tvsm.CostPerformanceByWorkArea(), costparameters);
				int total_estimate = Convert.ToInt32(dr["Sum_Total_Est"].ToString());
				int difference_estimate = Convert.ToInt32(dr["Difference_Est"].ToString());
				difference_estimate = Math.Abs(difference_estimate);
				cost_round = (double)difference_estimate / (double)total_estimate;
				cost_round = Math.Abs(1 - cost_round);
				cost_round = cost_round * 10;
				if (cost_round < 1)
				{
					returnInt = 1;
				}
				else if (cost_round > 10)
				{
					returnInt = 10;
				}
				else
				{
					returnInt = (int)Math.Round(cost_round, 0);
				}
			}
			catch (Exception ex)
			{
				returnInt = 0;
			}
		
			return returnInt;
		}
		public string getUsernameByBEMSID(int bemsid)
		{
			string userName = string.Empty;
			//var dal = new TVSM_DAL();
			UserViewModel userViewModel = new UserViewModel();
			DataSet ds = new DataSet();
			DataTable tbl = new DataTable();

			SqlParameter[] parameters = userViewModel.GetUserDataByBEMSIDParameter();
			parameters[0].Value = bemsid;
			SqlDataReader dr = null;
			dr = dal.GetSqlDataReader(userViewModel.GetUserDataByBEMSID(), parameters);
			while (dr.Read())
			{
				userName = dr["Name"].ToString();
				
			}
			userViewModel.Name = userName;
			Session["BEMSID"] = bemsid;
			return userName;
		}

		private static List<SelectListItem> ProgramPopulation(string siteCode)
        {
            List<SelectListItem> items = new List<SelectListItem>();

           var dal = new TVSM_DAL();
            var pg = new ProgramModel();

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            SqlParameter[] parameters = pg.GetProgramBySiteCodeParameter();
            parameters[0].Value = siteCode;

            SqlDataReader dr = null;
            dr = dal.GetSqlDataReader(pg.GetProgramBySiteCode(), parameters);
            while(dr.Read())
             {
                    items.Add(new SelectListItem
                    {
                        Text = dr["Program"].ToString(),
                        Value = dr["Site_Code"].ToString()
                    });
             }
                
              return items;
        }

        [HttpPost]
        //public ActionResult Old_Index(string[] selectedSiteCodes)
        //{
        //    List<SiteCode> allCust = new List<SiteCode>();
           
        //    var sc = new SiteCode();
        //    DataSet ds = new DataSet();
        //    DataTable tbl = new DataTable();
        //    ds = dal.NoParameterGetDataSet(sc.GetAllSiteCode(), "tbl").Tables["tbl"].DataSet;

        //    var siteCodeList = ds.Tables[0].AsEnumerable().Select(DataRow => new SiteCode
        //    {
        //        BCA_Name = DataRow.Field<string>("BCA_Name"),
        //        Site_Code = DataRow.Field<string>("Site_Code")

        //    });

        //   int count = selectedSiteCodes.Length;

        //    ViewBag.NumberOfSelectedSiteCodes = "From DashController " + count;


        //    SiteCode selectedSiteCode = new SiteCode();

        //    List<SiteCode> listSelectedSiteCodes = new List<SiteCode>();

        //    StringBuilder sb = new StringBuilder();


        //    for (int i = 0; i < count; i++)
        //    {

        //        selectedSiteCode.Site_Code = selectedSiteCodes[i].ToString();
        //        selectedSiteCode.BCA_Name = getSiteNameFromSiteCode(selectedSiteCode.Site_Code);
        //        listSelectedSiteCodes.Add(selectedSiteCode);
        //        sb.Append(selectedSiteCode.BCA_Name);
        //        sb.AppendLine();

        //    }

        //    ViewBag.BCA_Name = sb.ToString();

        //    if (selectedSiteCodes != null)
        //    {
        //        ViewBag.Message = "Selected Site Codes: " + string.Join(", ", selectedSiteCodes);
        //        //ViewBag.Message = "Selected Site Codes: " + string.Join(", ", selectedSiteCode.BCA_Name);
        //    }
        //    else
        //    {
        //        ViewBag.Message = "No Site Code selected";
        //    }

        //    allCust = siteCodeList.ToList();

        //    return View(selectedSiteCodes);
        //}

		 /**Post Selected Sites from listAllSites.cshtml**/
        //[HttpPost]
        public string Home(IEnumerable<string> SelectedSites)
        {
            if (SelectedSites == null)
            {
                return "You did not select any Site Name";
            }
            else
            {
                string[] selectedSiteCodes = SelectedSites.ToString().Split(',');
                int count = selectedSiteCodes.Length;

                StringBuilder sb = new StringBuilder();

                string selectedSiteNames = string.Empty;

				foreach (var sc in SelectedSites)
				{

					//sb.Append(string.Join(", ", getSiteNameFromSiteCode(sc)));
					sb.Append(getSiteNameFromSiteCode(sc) + ", ");
				}


				//sb.Append(string.Join(",", SelectedSites));

				ViewBag.selectedSiteNames = sb.ToString().Trim().TrimEnd(',');


				return (ViewBag.selectedSiteNames);

            }
        }

        [HttpPost]
        public ActionResult SelectedSiteCode(string selectedSiteCode)
        {
            var sc = new SiteCode();
            sc.Site_Code = Request.QueryString["site_code"];
            sc.BCA_Name = getSiteNameFromSiteCode(Request.QueryString["site_code"].ToString());

            ViewBag.selectedSiteCode = sc;

            return View();
        }

        public ActionResult SiteCodePartialView()
        {
            return View();
        }

		public string getSiteNameFromSiteCode(string SiteCode)
        {
            string SiteName = string.Empty;
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

		public ActionResult DateRangePartialView()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult DateRangePartialView(string dateString)
		{
			dateString = ViewBag.DateOfDaysAgo.ToString();

			return PartialView();
		}

		public ActionResult getDateNumberDaysAgo(string NumDaysAgo)
		{
			NumDaysAgo = "120";
			int i = Convert.ToInt32(NumDaysAgo);

			DateTime DayAgo = DateTime.Today.AddDays(-i);
			ViewBag.DateOfDaysAgo = DayAgo;

			//return Content (DayAgo.ToString(), "text/plain");

			//return Json(ViewBag.DateOfDaysAgo, JsonRequestBehavior.AllowGet);
			return View("DateRangePartialView");
		}

		
		#region SchedulePerformance
		//public ActionResult SchedulePerformance()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();
		//	sp = GetSchedulePerformanceCounts();

		//	ViewBag.DelinCount = sp.DelinCount.ToString();
		//	ViewBag.LateCount = sp.LateCount.ToString();
		//	ViewBag.OnTimeCount = sp.OnTimeCount.ToString();
		//	ViewBag.TotalCount = sp.TotalCount.ToString();

		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetSchedulePerformanceBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
		//	sp.TotalCount = ds.Tables[0].Rows.Count;

		//	var spList = ds.Tables[0].AsEnumerable().Select(DataRow => new SchedulePerformanceModel
		//	{
		//		Tool_Number = DataRow.Field<string>("Tool_Number"),
		//		Order_ID = DataRow.Field<string>("Order_ID"),
		//		Event = DataRow.Field<string>("Event"),
		//		Performance = DataRow.Field<string>("Performance"),
		//		Tooling_Health = DataRow.Field<string>("Tooling_Health")
		//	});

		//	return View(spList.ToList());
		//}
		//protected List<SchedulePerformanceModel> getAllSPbySiteCode()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();
		//	sp = GetSchedulePerformanceCounts();

		//	ViewBag.DelinCount = sp.DelinCount.ToString();
		//	ViewBag.LateCount = sp.LateCount.ToString();
		//	ViewBag.OnTimeCount = sp.OnTimeCount.ToString();
		//	ViewBag.TotalCount = sp.TotalCount.ToString();

		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetSchedulePerformanceBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
		//	sp.TotalCount = ds.Tables[0].Rows.Count;

		//	var spList = ds.Tables[0].AsEnumerable().Select(DataRow => new SchedulePerformanceModel
		//	{
		//		Tool_Number = DataRow.Field<string>("Tool_Number"),
		//		Order_ID = DataRow.Field<string>("Order_ID"),
		//		Event = DataRow.Field<string>("Event"),
		//		Performance = DataRow.Field<string>("Performance"),
		//		Tooling_Health = DataRow.Field<string>("Tooling_Health")
		//	});

		//	return spList.ToList();
		//}
		//public SchedulePerformanceModel GetSchedulePerformanceCounts()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();

		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetSchedulePerformanceCountBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetSchedulePerformanceCountBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;

		//	if (ds.Tables[0].Rows.Count >= 1)
		//	{
		//		string DelinInt;
		//		string LateInt;
		//		string OnTimeInt;

		//		DelinInt = ds.Tables[0].Rows[0][0].ToString();
		//		LateInt = ds.Tables[0].Rows[1][0].ToString();
		//		OnTimeInt = ds.Tables[0].Rows[2][0].ToString();

		//		sp.DelinCount = Convert.ToInt32(DelinInt.ToString());
		//		sp.LateCount = Convert.ToInt32(LateInt.ToString());
		//		sp.OnTimeCount = Convert.ToInt32(OnTimeInt.ToString());
		//		sp.TotalCount = sp.DelinCount + sp.LateCount + sp.OnTimeCount;
		//	}
		//	else
		//	{
		//		sp.DelinCount = 0;
		//		sp.LateCount = 0;
		//		sp.OnTimeCount = 0;
		//		sp.TotalCount = 0;
		//	}

		//	return sp;
		//}
		//public PartialViewResult AllSchedulePerformance()
		//{
		//	List<SchedulePerformanceModel> model = getAllSPbySiteCode();
		//	return PartialView("_SchedulePerformance", model);
		//}
		//public PartialViewResult DelinSchedulePerformance()
		//{
		//	List<SchedulePerformanceModel> model = getDelinSPbySiteCode();
		//	return PartialView("_SchedulePerformance", model);
		//}
		//public PartialViewResult LateSchedulePerformance()
		//{
		//	List<SchedulePerformanceModel> model = getLateSPbySiteCode();
		//	return PartialView("_SchedulePerformance", model);
		//}
		//public PartialViewResult OnTimeSchedulePerformance()
		//{
		//	List<SchedulePerformanceModel> model = getOnTimeSPbySiteCode();
		//	return PartialView("_SchedulePerformance", model);
		//}
		//protected List<SchedulePerformanceModel> getDelinSPbySiteCode()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();
		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	sp = GetSchedulePerformanceCounts();

		//	ViewBag.DelinCount = sp.DelinCount.ToString();
		//	ViewBag.LateCount = sp.LateCount.ToString();
		//	ViewBag.OnTimeCount = sp.OnTimeCount.ToString();
		//	ViewBag.TotalCount = sp.TotalCount.ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetDelinSchedulePerformanceBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetDelinSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
		//	sp.DelinCount = ds.Tables[0].Rows.Count;

		//	var spList = ds.Tables[0].AsEnumerable().Select(DataRow => new SchedulePerformanceModel
		//	{
		//		Tool_Number = DataRow.Field<string>("Tool_Number"),
		//		Order_ID = DataRow.Field<string>("Order_ID"),
		//		Event = DataRow.Field<string>("Event"),
		//		Performance = DataRow.Field<string>("Performance"),
		//		Tooling_Health = DataRow.Field<string>("Tooling_Health")
		//	});

		//	return spList.ToList();				

		//}
		//protected List<SchedulePerformanceModel> getLateSPbySiteCode()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();
		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	sp = GetSchedulePerformanceCounts();

		//	ViewBag.DelinCount = sp.DelinCount.ToString();
		//	ViewBag.LateCount = sp.LateCount.ToString();
		//	ViewBag.OnTimeCount = sp.OnTimeCount.ToString();
		//	ViewBag.TotalCount = sp.TotalCount.ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetLateSchedulePerformanceBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetLateSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
		//	sp.LateCount = ds.Tables[0].Rows.Count;

		//	var spList = ds.Tables[0].AsEnumerable().Select(DataRow => new SchedulePerformanceModel
		//	{
		//		Tool_Number = DataRow.Field<string>("Tool_Number"),
		//		Order_ID = DataRow.Field<string>("Order_ID"),
		//		Event = DataRow.Field<string>("Event"),
		//		Performance = DataRow.Field<string>("Performance"),
		//		Tooling_Health = DataRow.Field<string>("Tooling_Health")
		//	});

		//	return spList.ToList();

		//}
		//protected List<SchedulePerformanceModel> getOnTimeSPbySiteCode()
		//{
		//	SchedulePerformanceModel sp = new SchedulePerformanceModel();
		//	sp.SiteCode = Session["FirstSelectedSiteCode"].ToString();

		//	sp = GetSchedulePerformanceCounts();

		//	ViewBag.DelinCount = sp.DelinCount.ToString();
		//	ViewBag.LateCount = sp.LateCount.ToString();
		//	ViewBag.OnTimeCount = sp.OnTimeCount.ToString();
		//	ViewBag.TotalCount = sp.TotalCount.ToString();

		//	DataSet ds = new DataSet();
		//	DataTable tbl = new DataTable();

		//	SqlParameter[] parameters = sp.GetOnTimeSchedulePerformanceBySiteCodeParameter();
		//	parameters[0].Value = sp.SiteCode;

		//	ds = dal.GetDataSet(sp.GetOnTimeSchedulePerformanceBySiteCode(), parameters, "tbl").Tables["tbl"].DataSet;
		//	sp.OnTimeCount = ds.Tables[0].Rows.Count;

		//	var spList = ds.Tables[0].AsEnumerable().Select(DataRow => new SchedulePerformanceModel
		//	{
		//		Tool_Number = DataRow.Field<string>("Tool_Number"),
		//		Order_ID = DataRow.Field<string>("Order_ID"),
		//		Event = DataRow.Field<string>("Event"),
		//		Performance = DataRow.Field<string>("Performance"),
		//		Tooling_Health = DataRow.Field<string>("Tooling_Health")
		//	});

		//	return spList.ToList();
		//}
#endregion SchedulePerformance



		public ActionResult OrderDetail()
		{
			return View();
		}

	}


	}
