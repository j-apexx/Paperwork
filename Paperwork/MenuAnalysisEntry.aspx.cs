using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class MenuAnalysisEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["AccessLevel"] == null || Session["AccessLevel"].ToString().ToUpper() != "ADMIN")
            {
                buttonDeleteMenu.Visible = false;
                //iCopy.Visible = false;
            }


            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                //Set the programyear
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DropDownListDistricts.DataBind();
                if (DropDownListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DropDownListDistricts_SelectedIndexChanged(null, null);
                }

                DropDownListSchools.DataBind();
                DropDownListSchools_SelectedIndexChanged(null, null);

                DDLCategories.DataBind();
                DDLCategories_SelectedIndexChanged(DDLCategories, EventArgs.Empty);

            }
        }

        protected void UpdateMenuDayItemsOrder(object sender, EventArgs e)
        {
            if (Request.Form["NewOrd"] == null || Request.Form["OrigOrd"] == null)
                return;
            string[] NewOrdsArray = Request.Form["NewOrd"].Split(',');
            string[] OrigOrdsArray = Request.Form["OrigOrd"].Split(',');

            int dayInt = 0;
            int lastDayInt = 0;
            int ordCounter = 100;
            int ordOrig = 0;
            int ordNew = 0;
            bool orderChanged = false;

            for (int i=0;  i<NewOrdsArray.Length; i++)
            {
                string[] NewDayOrdArray = NewOrdsArray[i].Split('-');
                string[] OrigDayOrdArray = OrigOrdsArray[i].Split('-');
                

                if (int.TryParse(NewDayOrdArray[0], out dayInt)==false)
                    continue;
                if (int.TryParse(OrigDayOrdArray[1], out ordOrig) == false)
                    continue;
                if (int.TryParse(NewDayOrdArray[1], out ordNew) == false)
                    continue;

                if (lastDayInt == 0 || dayInt != lastDayInt)
                {
                    lastDayInt = dayInt;
                    ordCounter = 100;
                    orderChanged = false;//reset the orderChanged flag

                    if (dayInt > 0 && ListViewMenuDays.Items.Count >= dayInt)
                    { 
                        //dayInt is the actual day number - but the ListView is 0 based
                        HiddenField HFOrderChanged = (HiddenField)ListViewMenuDays.Items[dayInt-1].FindControl("HFOrderChanged");
                        if (HFOrderChanged != null)
                        {
                            if (HFOrderChanged.Value != "")
                                orderChanged = true;
                        }
                    }



                    if (orderChanged)
                    {
                        //move all items up by 1000 in preparation for the new order
                        MoveEntreesUp1000(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), dayInt);
                    }
                }

                if (orderChanged == false)
                    continue;



                if (ordNew<100 )
                {
                    //Standards slots are populated First Come First Serve - if the slot is occupied - move the item to the next available additional slot
                    if (StandardSlotOccupied(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), dayInt, ordNew))
                        MoveSingleAddtlEntreeFromTo(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), dayInt, 1000 + ordOrig, ordCounter++);
                    else
                        MoveItemToStandardsSlot(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), dayInt, 1000 + ordOrig, ordNew);
                }
                else
                {
                    MoveSingleAddtlEntreeFromTo(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), dayInt, 1000 + ordOrig, ordCounter++);
                }


            }

            ListViewMenuDays.DataBind();
                

        }

        protected bool StandardSlotOccupied(string pYear, string dist, int schoolNum, int menuCat, int day, int ordNew)
        {
            bool occupied = false;
            string sql = "SELECT COUNT(*) FROM MenuAnalysisEntry WHERE ProgramYear=@pYear AND DistrictID=@dist AND SchoolNum=@schoolNum AND MenuCategory=@menuCat AND DayNumber=@day AND ORD=@ordNew";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnPaperwork"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@pYear", pYear);
                        cmd.Parameters.AddWithValue("@dist", dist);
                        cmd.Parameters.AddWithValue("@schoolNum", schoolNum);
                        cmd.Parameters.AddWithValue("@menuCat", menuCat);
                        cmd.Parameters.AddWithValue("@day", day);
                        cmd.Parameters.AddWithValue("@ordNew", ordNew);
                        conn.Open();
                        int recordcount = (int)cmd.ExecuteScalar();
                        if (recordcount > 0)
                            occupied = true;
                    }
                    catch (SqlException ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }
            return occupied;
        }

        protected bool MoveItemToStandardsSlot(string pYear, string dist, int schoolNum, int menuCat, int day, int ordOrig, int ordNew)
        {
            bool successfullyMoved = false;
            string sql = "UPDATE MenuAnalysisEntry SET ORD=@ordNew WHERE ProgramYear=@pYear AND DistrictID=@dist AND SchoolNum=@schoolNum AND MenuCategory=@menuCat AND DayNumber=@day AND ORD=@ordOrig;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnPaperwork"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {


                        cmd.Parameters.AddWithValue("@pYear", pYear);
                        cmd.Parameters.AddWithValue("@dist", dist);
                        cmd.Parameters.AddWithValue("@schoolNum", schoolNum);
                        cmd.Parameters.AddWithValue("@menuCat", menuCat);
                        cmd.Parameters.AddWithValue("@day", day);
                        cmd.Parameters.AddWithValue("@ordOrig", ordOrig);
                        cmd.Parameters.AddWithValue("@ordNew", ordNew);
                        conn.Open();
                        int recordcount=cmd.ExecuteNonQuery();
                        if (recordcount > 0)
                            successfullyMoved = true;
                    }
                    catch (SqlException ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }
            return successfullyMoved;
        }

        protected void MoveEntreesUp1000(string pYear, string dist, int schoolNum, int menuCat, int day)
        {
            //Updated - if for some reason there are stranded records above 1000 - they will be removed before preparing to move the rest up
            string sql = "DELETE MenuAnalysisEntry WHERE ProgramYear=@pYear AND DistrictID=@dist AND SchoolNum=@schoolNum AND MenuCategory=@menuCat AND DayNumber=@day AND ORD>1000;" +
                "UPDATE MenuAnalysisEntry SET ORD=ORD+1000 WHERE ProgramYear=@pYear AND DistrictID=@dist AND SchoolNum=@schoolNum AND MenuCategory=@menuCat AND DayNumber=@day AND ORD<300;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnPaperwork"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pYear", pYear);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@schoolNum", schoolNum);
                    cmd.Parameters.AddWithValue("@menuCat", menuCat);
                    cmd.Parameters.AddWithValue("@day", day);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void MoveSingleAddtlEntreeFromTo(string pYear, string dist, int schoolNum, int menuCat, int day, int ordCurrent, int ordNew)
        {
            string sql = "UPDATE MenuAnalysisEntry SET ORD=@ordNew WHERE ProgramYear=@pYear AND DistrictID=@dist AND SchoolNum=@schoolNum AND MenuCategory=@menuCat AND DayNumber=@day AND ORD=@ordCurrent";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnPaperwork"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pYear", pYear);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@schoolNum", schoolNum);
                    cmd.Parameters.AddWithValue("@menuCat", menuCat);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@ordCurrent", ordCurrent);
                    cmd.Parameters.AddWithValue("@ordNew", ordNew);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Don't change the program year Session - these values may be antered and modified for prev year during summer.
            //changed the following from sql view to reader to optimize page after recommendation

            using (SqlDataReader reader = (SqlDataReader)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty))
            {
                while (reader.Read())
                {
                    if (reader["ProgramYear"].ToString() == DDLProgramYears.SelectedValue)
                    {
                        DateTime PYStartDate = (DateTime)reader["StartDate"];
                        HFProgramYearStartDate.Value = PYStartDate.ToShortDateString();

                        //If the program year start date is null or the current date is less than 3 months from the start date - don't populate listView
                        if (PYStartDate == null || DateTime.Now < PYStartDate.AddMonths(3))
                        {
                            ListViewMenuDays.Visible = false;
                            LabelYearWarning.Visible = true;
                            return;
                        }
                        else
                        {
                            ListViewMenuDays.Visible = true;
                            LabelYearWarning.Visible = false;
                        }
                        break;
                    }

                }
            }
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                Session["LastItemCat"] = DDLCategories.SelectedValue.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void DropDownListSchools_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlDataReader reader = (SqlDataReader)SqlDataSourceSchools.Select(DataSourceSelectArguments.Empty))
            {
                while (reader.Read())
                {
                    if (reader["SchoolNum"].ToString() == DropDownListSchools.SelectedValue)
                    {
                        HFSchoolType.Value = reader["SchoolType"].ToString();
                        break;
                    }
                }
            }

            //Need to force rebinding - otherwise it won't happen and GridViews will not be shown/hidden
            //However - on page loading - this was causing listview days to be blank.
            if (IsPostBack)
                ListViewMenuDays.DataBind();

            //Switching schools wasn't updating the averages
            if(DDLCategories.SelectedValue=="3" || DDLCategories.SelectedValue == "7" )
                getOverallSummary();
        }

        protected void DDLCategories_SelectedIndexChanged(object sender, EventArgs e)
        {


            switch (DDLCategories.SelectedValue)
            {
                case "1"://Brk Entree
                    HFItemCategoryID.Value = "1";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "2"://Brk Fruit
                    HFItemCategoryID.Value = "4";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "3"://Brk Summary/Averages
                    HFItemCategoryID.Value = "5";
                    divFruitVegButtons.Visible = true;
                    divMenuAdd.Visible = true;
                    IButtonAddVeg1.Visible = false; IButtonAddVeg2.Visible = false;
                    IButtonRemoveVeg1.Visible = false; IButtonRemoveVeg2.Visible = false;
                    divSummary.Visible = true;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    getOverallSummary();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "4"://Lunch Entree
                    HFItemCategoryID.Value = "2";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "5"://Lunch Fruit
                    HFItemCategoryID.Value = "4";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "6"://Lunch Veg
                    HFItemCategoryID.Value = "3";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = true;
                    break;
                case "7"://Lunch Summary/Averages
                    HFItemCategoryID.Value = "5";
                    divFruitVegButtons.Visible = true;
                    IButtonAddVeg1.Visible = true; IButtonAddVeg2.Visible = true;
                    IButtonRemoveVeg1.Visible = true; IButtonRemoveVeg2.Visible = true;
                    divMenuAdd.Visible = true;
                    divSummary.Visible = true;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    getOverallSummary();
                    //IButtonPrintAvgs.Visible = false;
                    break;
                default:
                    HFItemCategoryID.Value = "0";
                    divFruitVegButtons.Visible = false;
                    divMenuAdd.Visible = false;
                    divSummary.Visible = false;//divSummary is at the bottom of the page with overall summary info
                    //Category Changed - update the range hidden fields
                    UpdatePriceRanges();
                    //IButtonPrintAvgs.Visible = false;
                    break;
            }

            //Need to force rebinding - otherwise it won't happen and GridViews will not be shown/hidden
            //However - on page loading - this was causing listview days to be blank.
            if (IsPostBack)
                ListViewMenuDays.DataBind();
        }

        protected void UpdatePriceRanges()
        {
            //reset values
            HFCatMidLower.Value = "";
            HFCatMidUpper.Value = "";

            SqlDataSourceRanges.DataBind();
            using (SqlDataReader reader = (SqlDataReader)SqlDataSourceRanges.Select(DataSourceSelectArguments.Empty))
            {
                if (reader.Read())
                {
                    decimal lowerRange = decimal.MaxValue;
                    decimal upperRange = decimal.MaxValue;
                    if (reader["MidrangeLower"] != null)
                    {
                        decimal.TryParse(reader["MidrangeLower"].ToString(), out lowerRange);
                    }
                    if (reader["MidrangeUpper"] != null)
                    {
                        decimal.TryParse(reader["MidrangeUpper"].ToString(), out upperRange);
                    }

                    HFCatMidLower.Value = lowerRange.ToString();
                    HFCatMidUpper.Value = upperRange.ToString();
                    HFEntreeMidLower.Value = lowerRange.ToString();
                    HFEntreeMidUpper.Value = upperRange.ToString();
                }
            }

            if(DDLCategories.SelectedValue=="3" || DDLCategories.SelectedValue == "7")
            {
                decimal lowerRangeMeal = decimal.MaxValue;
                decimal upperRangeMeal = decimal.MaxValue;

                SqlDataSourceRangesForOverall.DataBind();
                using (SqlDataReader reader2 = (SqlDataReader)SqlDataSourceRangesForOverall.Select(DataSourceSelectArguments.Empty))
                {
                    if (reader2.Read())
                    {
                        if (reader2["MidrangeLower"] != null)
                        {
                            decimal.TryParse(reader2["MidrangeLower"].ToString(), out lowerRangeMeal);
                        }
                        if (reader2["MidrangeUpper"] != null)
                        {
                            decimal.TryParse(reader2["MidrangeUpper"].ToString(), out upperRangeMeal);
                        }
                    }
                }
                
                HFTotalMealMidLower.Value = lowerRangeMeal.ToString();
                HFTotalMealMidUpper.Value = upperRangeMeal.ToString();
            }
        }

        protected void GridViewStandards_RowCommand(object sender, GridViewCommandEventArgs e)
        { 
            GridView gvStandard = (GridView)sender;
            Control parentGv = gvStandard.Parent;

            SqlDataSource SqlDataSourceStandards = new SqlDataSource();
            if (parentGv != null)
            {
                SqlDataSourceStandards = (SqlDataSource)FindControlRecursive(parentGv, "SqlDataSourceStandards");
            }

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Delete" && Page.IsValid && gvStandard!=null && rowIndex>=0)
            {
                if(SqlDataSourceStandards!=null)
                {
                    
                    SqlDataSourceStandards.DeleteParameters["ProgramYear"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["ProgramYear"].ToString();
                    SqlDataSourceStandards.DeleteParameters["DistrictID"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["DistrictID"].ToString();
                    SqlDataSourceStandards.DeleteParameters["SchoolNum"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["SchoolNum"].ToString();
                    SqlDataSourceStandards.DeleteParameters["MenuCategory"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["MenuCategory"].ToString();
                    SqlDataSourceStandards.DeleteParameters["DayNumber"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["DayNumber"].ToString();
                    SqlDataSourceStandards.DeleteParameters["ORD"].DefaultValue = gvStandard.DataKeys[rowIndex].Values["ORD"].ToString();

                    int recordcount=SqlDataSourceStandards.Delete();
                    if(recordcount>0)
                    {
                        ListViewMenuDays.DataBind();
                        //gvStandard.DataBind();
                    }
                }
            }

        }

        protected void gvMenuDay_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Control parentGv = ((GridView)sender).Parent;
            int recordcount = 0;
            
            SqlDataSource sdsDayMenuEntry = new SqlDataSource();
            if (parentGv != null)
            {
                sdsDayMenuEntry = (SqlDataSource)FindControlRecursive(parentGv, "SqlDataSourceDayMenuEntry");
            }

            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "InsertItem" && Page.IsValid)
            {
                try
                {
                    if(sdsDayMenuEntry!=null)
                    {
                        //TextBox TextBoxFQty = (TextBox)FindControlRecursive(parentGv, "TextBoxFQty");
                        DropDownList DDLMenuItems = (DropDownList)FindControlRecursive(parentGv, "DDLMenuItems");
                        CustomValidator validDDLMenuItems = (CustomValidator)FindControlRecursive(parentGv, "validDDLMenuItems");
                        if (/*TextBoxFQty == null ||*/ DDLMenuItems == null || DDLMenuItems.Items.Count == 0 || string.IsNullOrEmpty(DDLMenuItems.SelectedValue ) || DDLMenuItems.SelectedValue=="0")
                        {
                            if(validDDLMenuItems != null)                           
                                validDDLMenuItems.IsValid = false;
                            DDLMenuItems.Focus();
                            return;
                        }
                        else
                        {
                            if (validDDLMenuItems != null)
                                validDDLMenuItems.IsValid = true;
                        }

                        //if(Decimal.TryParse(TextBoxFQty.Text, out decimal decQty)==false)
                        //{ decQty = 0; }

                        sdsDayMenuEntry.InsertParameters["ItemID"].DefaultValue = DDLMenuItems.SelectedValue;
                        //sdsDayMenuEntry.InsertParameters["Quantity"].DefaultValue = decQty.ToString();
                        recordcount=sdsDayMenuEntry.Insert();

                        //if(recordcount>0)
                        //{
                        //    TextBoxFQty.Focus();
                        //}
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
            else if (e.CommandName == "Update" && Page.IsValid)
            {
                //if(DDLCategories.SelectedValue == "3" || DDLCategories.SelectedValue == "7")
                //    sdsDayMenuEntry.UpdateParameters["Quantity"].DefaultValue = "1";
            }
            else if (e.CommandName == "Delete" )
            {
                ((GridView)sender).Rows[0].Focus();
                
            }

        }

        /// <summary>
        /// Special scaled down version of the gvMenuDay_RowDataBound event for the GridViewStandards
        protected void gvMenuStandard_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ORD"].ToString() == "0")
                e.Row.Visible = false;

            GridView gv = (GridView)sender;
            Control gvParent = gv.Parent;
            if (e.Row.RowType == DataControlRowType.Header)
            {
                Label LabelHdrMenuItem = (Label)e.Row.FindControl("LabelHdrMenuItem");
                if (LabelHdrMenuItem != null)
                {
                    switch (gv.ID)
                    {
                        case "GridViewStandards01":
                            LabelHdrMenuItem.Text = "Hot Dish of the Day";
                            break;
                        case "GridViewStandards02":
                            LabelHdrMenuItem.Text = "Hot Fan Favorite";
                            break;
                        case "GridViewStandards10":
                            LabelHdrMenuItem.Text = "Pizzeria 1";
                            break;
                        case "GridViewStandards11":
                            LabelHdrMenuItem.Text = "Pizzeria 2";
                            break;
                        case "GridViewStandards12":
                            LabelHdrMenuItem.Text = "Pizzeria Specialty";
                            break;
                        case "GridViewStandards20":
                            LabelHdrMenuItem.Text = "Grill Line 1";
                            break;
                        case "GridViewStandards21":
                            LabelHdrMenuItem.Text = "Grill Line 2";
                            break;
                        case "GridViewStandards22":
                            LabelHdrMenuItem.Text = "Grill Line Specialty";
                            break;
                        case "GridViewStandards30":
                            LabelHdrMenuItem.Text = "Salad or Parfait Rotation";
                            break;
                        case "GridViewStandards31":
                            LabelHdrMenuItem.Text = "Salad 1";
                            break;
                        case "GridViewStandards32":
                            LabelHdrMenuItem.Text = "Salad 2";
                            break;
                        case "GridViewStandards33":
                            LabelHdrMenuItem.Text = "Salad Specialty";
                            break;
                        case "GridViewStandards40":
                            LabelHdrMenuItem.Text = "Yogurt Parfait";
                            break;
                        case "GridViewStandards50":
                            LabelHdrMenuItem.Text = "Deli Selection";
                            break;
                        case "GridViewStandards51":
                            LabelHdrMenuItem.Text = "Deli Line 1";
                            break;
                        case "GridViewStandards52":
                            LabelHdrMenuItem.Text = "Deli Line 2";
                            break;
                        case "GridViewStandards53":
                            LabelHdrMenuItem.Text = "Deli Specialty";
                            break;
                        case "GridViewStandards60":
                            LabelHdrMenuItem.Text = "Munchable";
                            break;
                        case "GridViewStandards61":
                            LabelHdrMenuItem.Text = "PB&J";
                            break;
                        default:
                            break;
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the delete button in the current row and set its CommandArgument.
                LinkButton deleteButton = (LinkButton)e.Row.FindControl("IDelete");
                if (deleteButton != null)
                {
                    deleteButton.CommandArgument = e.Row.RowIndex.ToString();
                }
            }

        }

        protected void gvMenuDay_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gv = (GridView)sender;
            Control gvParent = gv.Parent;
            if(gvParent == null)
                return;

            HiddenField HFTotalQty = (HiddenField)gvParent.FindControl("HFTotalQty");
            HiddenField HFTotalCost = (HiddenField)gvParent.FindControl("HFTotalCost");
            HiddenField HFTotalStandardsQty = (HiddenField)gvParent.FindControl("HFTotalStandardsQty");
            HiddenField HFTotalStandardsCost = (HiddenField)gvParent.FindControl("HFTotalStandardsCost");




            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ORD"].ToString() == "0")
            {
                e.Row.Visible = false;
            }


            if(e.Row.RowType==DataControlRowType.Header)
            {
                //reset the HiddenField values
                if ( HFTotalQty != null && HFTotalCost != null)
                {
                    HFTotalQty.Value = HFTotalCost.Value = "";
                }

                Label LabelHdrMenuItem = (Label)e.Row.FindControl("LabelHdrMenuItem");
                Label LabelHdrCost = (Label)e.Row.FindControl("LabelHdrCost");


                if (LabelHdrMenuItem != null && LabelHdrCost!=null)
                {
                    //Show/Hide header fields
                    switch (DDLCategories.SelectedValue)
                    {
                        case "1"://Brk Entree
                        case "2"://Brk Fruit
                        case "4"://Lunch Entree
                        case "5"://Lunch Fruit
                        case "6"://Lunch Veg
                            gv.ShowHeader = true;
                            LabelHdrMenuItem.Visible = true;
                            LabelHdrCost.Visible = true;
                            break;
                        case "3"://Brk Summary/Averages
                        case "7"://Lunch Summary/Averages
                            gv.ShowHeader = false;
                            break;
                        default:
                            LabelHdrMenuItem.Visible = true;
                            LabelHdrCost.Visible = true;
                            break;
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Needed to add hidden field since we removed the Qty column
                HiddenField HFLineQty = (HiddenField)FindControlRecursive(e.Row, "HFLineQty");
                if (HFLineQty != null && HFTotalQty != null)
                {
                    if (decimal.TryParse(HFLineQty.Value, out decimal outLineQty) == false)
                        outLineQty = 0M;
                    if (decimal.TryParse(HFTotalQty.Value, out decimal outTotalQty) == false)
                        outTotalQty = 0M;
                    HFTotalQty.Value = (outLineQty + outTotalQty).ToString("F0");
                }

                Label LabelMenuLineTotal = (Label)FindControlRecursive(e.Row, "LabelMenuLineTotal");
                if(LabelMenuLineTotal != null && HFTotalCost != null)
                {
                    if (decimal.TryParse(LabelMenuLineTotal.Text, out decimal outLineCost) == false)
                        outLineCost = 0M;
                    if (decimal.TryParse(HFTotalCost.Value, out decimal outTotalCost) == false)
                        outTotalCost = 0M;
                    HFTotalCost.Value = (outLineCost + outTotalCost).ToString("F2");
                }

                //Color coding
                if (DDLCategories.SelectedValue != "3" && DDLCategories.SelectedValue != "7")
                {
                    Label LabelMenuLineCost = (Label)FindControlRecursive(e.Row, "LabelMenuLineCost");
                    if (LabelMenuLineCost != null && !string.IsNullOrEmpty(HFCatMidLower.Value) && !string.IsNullOrEmpty(HFCatMidUpper.Value))
                    {
                        decimal decCost = 0M;
                        decimal decLower = decimal.MaxValue;
                        decimal decUpper = decimal.MaxValue;
                        if (decimal.TryParse(LabelMenuLineCost.Text, out decCost) == false)
                            decCost = 0M;
                        decimal.TryParse(HFCatMidLower.Value, out decLower);
                        decimal.TryParse(HFCatMidUpper.Value, out decUpper);

                        if (decCost < decLower)
                            LabelMenuLineCost.ForeColor = System.Drawing.Color.LimeGreen;
                        else if (decCost >= decLower && decCost <= decUpper)
                            LabelMenuLineCost.ForeColor = System.Drawing.Color.Goldenrod;
                        else if (decCost > decUpper)
                            LabelMenuLineCost.ForeColor = System.Drawing.Color.Red;
                        else
                            LabelMenuLineCost.ForeColor = System.Drawing.Color.Black;
                    }
                }
                
            }

            if(e.Row.RowType == DataControlRowType.Footer && gv.ID == "gvMenuDay")
            {
                TextBox TBBar1 = (TextBox)e.Row.FindControl("TBBar1");
                TextBox TBBar2 = (TextBox)e.Row.FindControl("TBBar2");
                //Literal Literal1 = (Literal)e.Row.FindControl("Literal1");
                Literal Literal2 = (Literal)e.Row.FindControl("Literal2");
                Literal Literal3 = (Literal)e.Row.FindControl("Literal3");
                Literal Literal4 = (Literal)e.Row.FindControl("Literal4");
                Literal Literal5 = (Literal)e.Row.FindControl("Literal5");

                //TextBox TextBoxMenuQtyTotal = (TextBox)e.Row.FindControl("TextBoxMenuQtyTotal");
                //if(TextBoxMenuQtyTotal != null && HFTotalQty != null)
                //{
                //    TextBoxMenuQtyTotal.Text = HFTotalQty.Value;
                //}

                TextBox TextBoxMenuAverageCost = (TextBox)e.Row.FindControl("TextBoxMenuAverageCost");
                if(TextBoxMenuAverageCost!=null && HFTotalQty != null && HFTotalCost != null)
                {
                    if (decimal.TryParse(HFTotalQty.Value, out decimal outTotalQty) == false)
                        outTotalQty = 0M;
                    if (decimal.TryParse(HFTotalCost.Value, out decimal outTotalCost) == false)
                        outTotalCost = 0M;
                    TextBoxMenuAverageCost.Text = (outTotalQty == 0 ? 0M : (outTotalCost/ outTotalQty)).ToString("F3");

                    //Color coding
                    if (!string.IsNullOrEmpty(HFCatMidLower.Value) && !string.IsNullOrEmpty(HFCatMidUpper.Value))
                    {
                        decimal decCost = 0M;
                        decimal decLower = decimal.MaxValue;
                        decimal decUpper = decimal.MaxValue;
                        if (decimal.TryParse(TextBoxMenuAverageCost.Text, out decCost) == false)
                            decCost = 0M;
                        decimal.TryParse(HFCatMidLower.Value, out decLower);
                        decimal.TryParse(HFCatMidUpper.Value, out decUpper);

                        if (decCost < decLower)
                            TextBoxMenuAverageCost.ForeColor = System.Drawing.Color.LimeGreen;
                        else if (decCost >= decLower && decCost <= decUpper)
                            TextBoxMenuAverageCost.ForeColor = System.Drawing.Color.Goldenrod;
                        else if (decCost > decUpper)
                            TextBoxMenuAverageCost.ForeColor = System.Drawing.Color.Red;
                        else
                            TextBoxMenuAverageCost.ForeColor = System.Drawing.Color.Black;
                    }
                }

                TextBox TextBoxMenuTotalCost = (TextBox)e.Row.FindControl("TextBoxMenuTotalCost");
                if(TextBoxMenuTotalCost!= null && HFTotalCost != null)
                {
                    TextBoxMenuTotalCost.Text = HFTotalCost.Value;
                }

                //Show/Hide header fields
                switch (DDLCategories.SelectedValue)
                {
                    case "1"://Brk Entree
                    case "2"://Brk Fruit
                    case "4"://Lunch Entree
                    case "5"://Lunch Fruit
                    case "6"://Lunch Veg

                        //TextBoxMenuQtyTotal.Visible = true;
                        TBBar1.Visible = true;
                        TBBar2.Visible = true;
                        TextBoxMenuTotalCost.Visible = true;
                        TextBoxMenuAverageCost.Visible = true;
                        //Literal1.Visible = true;
                        Literal2.Visible = true;
                        Literal3.Visible = true;
                        Literal4.Visible = true;
                        Literal5.Visible = true;
                        break;
                    case "3"://Brk Summary/Averages
                    case "7"://Lunch Summary/Averages
                        //TextBoxMenuQtyTotal.Visible = false;
                        TBBar1.Visible = false;
                        TBBar2.Visible = false;
                        TextBoxMenuTotalCost.Visible = false;
                        TextBoxMenuAverageCost.Visible = false;
                        //Literal1.Visible = false;
                        Literal2.Visible = false;
                        Literal3.Visible = false;
                        Literal4.Visible = false;
                        Literal5.Visible = false;
                        break;
                    default:
                        //TextBoxMenuQtyTotal.Visible = true;
                        TBBar1.Visible = true;
                        TBBar2.Visible = true;
                        TextBoxMenuTotalCost.Visible = true;
                        TextBoxMenuAverageCost.Visible = true;
                        //Literal1.Visible = true;
                        Literal2.Visible = true;
                        Literal3.Visible = true;
                        Literal4.Visible = true;
                        Literal5.Visible = true;
                        break;
                }
            }

            if(DDLCategories.SelectedValue=="3" || DDLCategories.SelectedValue=="7")
                UpdateMenuDayTotal(gvParent);

        }

        protected void SqlDataSourceDayMenuEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ListViewMenuDays.DataBind();
                ShowMessageIcon("Added", MessageType.Success);
                //UpdatePanel1.Update();
            }
            else if (e.Exception != null)
            {
                //ShowMessageIcon("Error", MessageType.Error);
                ShowDialog("Error", MessageType.Error, "Menu Item was not inserted.", e.Exception.Message);
            }
        }

        protected void SqlDataSourceDayMenuEntry_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                //ListViewMenuDays.DataBind();
                ShowMessageIcon("Deleted", MessageType.Success);
                //UpdatePanel1.Update();

            }
        }

        /// <summary>
        /// This procedure updates the yellow menu day total box for Brk Average and Lun Average
        /// </summary>
        /// <param name="ContainingControl"></param>
        protected void UpdateMenuDayTotal(Control ContainingControl)
        {

            DropDownList DDLVeg1Outside = (DropDownList)ContainingControl.FindControl("DDLVeg1Outside");
            DropDownList DDLVeg2Outside = (DropDownList)ContainingControl.FindControl("DDLVeg2Outside");
            DropDownList DDLFrt1Outside = (DropDownList)ContainingControl.FindControl("DDLFrt1Outside");
            DropDownList DDLFrt2Outside = (DropDownList)ContainingControl.FindControl("DDLFrt2Outside");
            TextBox TextBoxMenuDayTotal = (TextBox)ContainingControl.FindControl("TextBoxMenuDayTotal");

            HiddenField HFAverageEntreeBrk = (HiddenField)ContainingControl.FindControl("HFAverageEntreeBrk");
            HiddenField HFAverageEntreeLun = (HiddenField)ContainingControl.FindControl("HFAverageEntreeLun");
            HiddenField HFAverageVegLun = (HiddenField)ContainingControl.FindControl("HFAverageVegLun");
            HiddenField HFAverageFruitBrk = (HiddenField)ContainingControl.FindControl("HFAverageFruitBrk");
            HiddenField HFAverageFruitLun = (HiddenField)ContainingControl.FindControl("HFAverageFruitLun");
            HiddenField HFTotalCost = (HiddenField)ContainingControl.FindControl("HFTotalCost");


            if ( DDLVeg1Outside == null || DDLVeg2Outside == null || DDLFrt1Outside == null || DDLFrt2Outside == null 
                 || TextBoxMenuDayTotal == null || HFAverageEntreeBrk==null || HFAverageEntreeLun == null || HFAverageVegLun == null
                 || HFAverageFruitBrk == null || HFAverageFruitLun == null)
            {
                return;
            }

            if (decimal.TryParse(HFAverageEntreeBrk.Value, out decimal decBrkEntree) == false)
                decBrkEntree = 0M;
            if (decimal.TryParse(HFAverageEntreeLun.Value, out decimal decLunEntree) == false)
                decLunEntree = 0M;
            if (decimal.TryParse(HFAverageVegLun.Value, out decimal decVegLun) == false)
                decVegLun = 0M;
            if (decimal.TryParse(HFAverageFruitBrk.Value, out decimal decFrtBrk) == false)
                decFrtBrk = 0M;
            if (decimal.TryParse(HFAverageFruitLun.Value, out decimal decFrtLun) == false)
                decFrtLun = 0M;
            if (decimal.TryParse(HFTotalCost.Value, out decimal decGridViewCost) == false)
                decGridViewCost = 0M;

            decimal menuTotal = 0M;

            
            menuTotal += decGridViewCost;

            if (DDLVeg1Outside.SelectedValue == "1") menuTotal+= decVegLun;
            if (DDLVeg2Outside.SelectedValue == "1") menuTotal += decVegLun;

            if (DDLCategories.SelectedValue == "3")
            {
                if (DDLFrt1Outside.SelectedValue == "1") menuTotal += decFrtBrk;
                if (DDLFrt2Outside.SelectedValue == "1") menuTotal += decFrtBrk;
                menuTotal += decBrkEntree;

                //if menuEntree is zero, zero out the total
                if (decBrkEntree == 0M)
                    menuTotal = 0M;
            }
            else if (DDLCategories.SelectedValue == "7")
            {
                if (DDLFrt1Outside.SelectedValue == "1") menuTotal += decFrtLun;
                if (DDLFrt2Outside.SelectedValue == "1") menuTotal += decFrtLun;
                menuTotal += decLunEntree;

                //if menuEntree is zero, zero out the total
                if (decLunEntree == 0M)
                    menuTotal = 0M;
            }

            TextBoxMenuDayTotal.Text = menuTotal.ToString("F2");
        }

        protected void ListViewMenuDays_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)e.Item;
            TextBox TextBoxAverageEntreeOutside = (TextBox)lvi.FindControl("TextBoxAverageEntreeOutside");
            DropDownList DDLVeg1Outside = (DropDownList)lvi.FindControl("DDLVeg1Outside");
            DropDownList DDLVeg2Outside = (DropDownList)lvi.FindControl("DDLVeg2Outside");
            DropDownList DDLFrt1Outside = (DropDownList)lvi.FindControl("DDLFrt1Outside");
            DropDownList DDLFrt2Outside = (DropDownList)lvi.FindControl("DDLFrt2Outside");
            TextBox TextBoxVeg1AvgOutside = (TextBox)lvi.FindControl("TextBoxVeg1AvgOutside");
            TextBox TextBoxVeg2AvgOutside = (TextBox)lvi.FindControl("TextBoxVeg2AvgOutside");
            TextBox TextBoxFrt1AvgOutside = (TextBox)lvi.FindControl("TextBoxFrt1AvgOutside");
            TextBox TextBoxFrt2AvgOutside = (TextBox)lvi.FindControl("TextBoxFrt2AvgOutside");
            Control AveragesTable = (Control)lvi.FindControl("AveragesTable");
            TextBox TextBoxMenuDayTotal = (TextBox)lvi.FindControl("TextBoxMenuDayTotal");

            Label LabelDayNumber = (Label)lvi.FindControl("LabelDayNumber");
            int dayNumberInt = 0;
            if (LabelDayNumber == null) return;
            else int.TryParse(LabelDayNumber.Text, out dayNumberInt);

            if (dayNumberInt < 1 || dayNumberInt > 30) return;

            GridView gvMenuDay = (GridView)lvi.FindControl("gvMenuDay");
            if (gvMenuDay == null || AveragesTable == null || DDLVeg1Outside==null || DDLVeg2Outside==null || DDLFrt1Outside==null
                || DDLFrt2Outside==null || TextBoxVeg1AvgOutside==null || TextBoxVeg2AvgOutside==null || TextBoxFrt1AvgOutside==null || TextBoxFrt2AvgOutside==null || TextBoxMenuDayTotal==null)
            {
                return;
            }
          

            switch (DDLCategories.SelectedValue)
            {
                case "1"://Brk Entree
                case "2"://Brk Fruit
                case "4"://Lunch Entree
                case "5"://Lunch Fruit
                case "6"://Lunch Veg
                    AveragesTable.Visible = false;
                    TextBoxMenuDayTotal.Visible = false;
                    break;
                case "3"://Brk Summary/Averages
                case "7"://Lunch Summary/Averages
                    HiddenField HFAverageEntreeBrk = (HiddenField)lvi.FindControl("HFAverageEntreeBrk");
                    HiddenField HFAverageEntreeLun = (HiddenField)lvi.FindControl("HFAverageEntreeLun");
                    HiddenField HFAverageVegLun = (HiddenField)lvi.FindControl("HFAverageVegLun");
                    HiddenField HFAverageFruitBrk = (HiddenField)lvi.FindControl("HFAverageFruitBrk");
                    HiddenField HFAverageFruitLun = (HiddenField)lvi.FindControl("HFAverageFruitLun");

                    //Convert HF values to decimals
                    if (decimal.TryParse(HFAverageEntreeBrk.Value, out decimal decBrkEntree) == false)
                        decBrkEntree = 0M;
                    if (decimal.TryParse(HFAverageEntreeLun.Value, out decimal decLunEntree) == false)
                        decLunEntree = 0M;
                    if (decimal.TryParse(HFAverageVegLun.Value, out decimal decVegLun) == false)
                        decVegLun = 0M;
                    if (decimal.TryParse(HFAverageFruitBrk.Value, out decimal decFrtBrk) == false)
                        decFrtBrk = 0M;
                    if (decimal.TryParse(HFAverageFruitLun.Value, out decimal decFrtLun) == false)
                        decFrtLun = 0M;

                    AveragesTable.Visible = true;
                    TextBoxMenuDayTotal.Visible = true;

                    if(DDLCategories.SelectedValue=="3")
                        TextBoxAverageEntreeOutside.Text = (decBrkEntree).ToString("F2");
                    else if(DDLCategories.SelectedValue=="7")
                        TextBoxAverageEntreeOutside.Text = (decLunEntree).ToString("F2");

                    decimal decCost = 0M;
                    decimal decLower = decimal.MaxValue;
                    decimal decUpper = decimal.MaxValue;

                    if (DDLCategories.SelectedValue == "3")
                        decCost = decBrkEntree;
                    else if (DDLCategories.SelectedValue == "7")
                        decCost = decLunEntree;

                    decimal.TryParse(HFCatMidLower.Value, out decLower);
                    decimal.TryParse(HFCatMidUpper.Value, out decUpper);

                    if (decCost < decLower)
                        TextBoxAverageEntreeOutside.ForeColor = System.Drawing.Color.LimeGreen;
                    else if (decCost >= decLower && decCost <= decUpper)
                        TextBoxAverageEntreeOutside.ForeColor = System.Drawing.Color.Goldenrod;
                    else if (decCost > decUpper)
                        TextBoxAverageEntreeOutside.ForeColor = System.Drawing.Color.Red;
                    else
                        TextBoxAverageEntreeOutside.ForeColor = System.Drawing.Color.Black;


                    //Show-Hide the Veg fields
                    if (DDLCategories.SelectedValue=="3")
                    {
                        DDLVeg1Outside.Visible = false;
                        DDLVeg2Outside.Visible = false;
                        TextBoxVeg1AvgOutside.Visible = false;
                        TextBoxVeg2AvgOutside.Visible = false;
                    }
                    else if (DDLCategories.SelectedValue == "7")
                    {
                        DDLVeg1Outside.Visible = true;
                        DDLVeg2Outside.Visible = true;
                        TextBoxVeg1AvgOutside.Visible = true;
                        TextBoxVeg2AvgOutside.Visible = true;
                    }

                    //** set the average dropdowns state here**
                    DDLVeg1Outside.SelectedValue = loadDDLAvgState(dayNumberInt, ((int)ItemCats.Vegetable) * 10, 1) == 1M ? "1" : "0";
                    DDLVeg2Outside.SelectedValue = loadDDLAvgState(dayNumberInt, ((int)ItemCats.Vegetable) * 10, 2) == 1M ? "1" : "0";
                    DDLFrt1Outside.SelectedValue = loadDDLAvgState(dayNumberInt, ((int)ItemCats.Fruit) * 10, 1) == 1M ? "1" : "0";
                    DDLFrt2Outside.SelectedValue = loadDDLAvgState(dayNumberInt, ((int)ItemCats.Fruit) * 10, 2) == 1M ? "1" : "0";

                    if (DDLVeg1Outside.SelectedValue == "1") TextBoxVeg1AvgOutside.Text = decVegLun.ToString("F2");
                    else TextBoxVeg1AvgOutside.Text = "0.00";

                    if (DDLVeg2Outside.SelectedValue == "1") TextBoxVeg2AvgOutside.Text = decVegLun.ToString("F2");
                    else TextBoxVeg2AvgOutside.Text = "0.00";

                    if (DDLCategories.SelectedValue == "3")
                    {
                        if (DDLFrt1Outside.SelectedValue == "1") TextBoxFrt1AvgOutside.Text = decFrtBrk.ToString("F2");
                        else TextBoxFrt1AvgOutside.Text = "0.00";
                        if (DDLFrt2Outside.SelectedValue == "1") TextBoxFrt2AvgOutside.Text = decFrtBrk.ToString("F2");
                        else TextBoxFrt2AvgOutside.Text = "0.00";
                    }
                    else if (DDLCategories.SelectedValue == "7")
                    {
                        if (DDLFrt1Outside.SelectedValue == "1") TextBoxFrt1AvgOutside.Text = decFrtLun.ToString("F2");
                        else TextBoxFrt1AvgOutside.Text = "0.00";
                        if (DDLFrt2Outside.SelectedValue == "1") TextBoxFrt2AvgOutside.Text = decFrtLun.ToString("F2");
                        else TextBoxFrt2AvgOutside.Text = "0.00";
                    }

                    UpdateMenuDayTotal(lvi);


                    break;
                default:

                    break;
            }

            if (DDLCategories.SelectedValue == "4")
            {
                for (int i = 0; i < 100; i++)
                {
                    GridView gv = (GridView)lvi.FindControl("GridViewStandards" + i.ToString("00"));
                    HiddenField HFStandardsIndex = (HiddenField)lvi.FindControl("HFStandardsIndex");
                    SqlDataSource SqlDataSourceStandards = (SqlDataSource)lvi.FindControl("SqlDataSourceStandards");
                    if (gv != null && HFStandardsIndex != null && SqlDataSourceStandards!=null)
                    {
                        HFStandardsIndex.Value = i.ToString();
                        gv.DataSource = SqlDataSourceStandards;
                        gv.DataBind();
                    }
                }
            }
        }

        protected void gvMenuDay_PreRender(object sender, EventArgs e)
        {
            GridView gvMenuDay = (GridView)sender;
            if(gvMenuDay != null)
            {
                switch (DDLCategories.SelectedValue)
                {
                    case "1"://Brk Entree
                    case "2"://Brk Fruit
                    case "4"://Lunch Entree
                    case "5"://Lunch Fruit
                        gvMenuDay.Columns[0].Visible = false;
                        gvMenuDay.Columns[3].Visible = true;
                        break;
                    case "6"://Lunch Veg
                        gvMenuDay.Columns[0].Visible = true;
                        gvMenuDay.Columns[3].Visible = true;
                        break;
                    case "3"://Brk Summary/Averages
                    case "7"://Lunch Summary/Averages
                        gvMenuDay.Columns[0].Visible = false;
                        gvMenuDay.Columns[3].Visible = false;
                        break;
                    default:
                        gvMenuDay.Columns[0].Visible = false;
                        gvMenuDay.Columns[3].Visible = true;
                        break;
                }


            }
        }

        protected void SqlDataSourceDayMenuEntry_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e.Command.Parameters["@ItemID"].Value.ToString()=="0")
            {
                e.Cancel = true;
                return;
            }
        }

        protected void gvMenuDay_DataBinding(object sender, EventArgs e)
        {
            GridView gvMenuDay = (GridView)sender;
            Control gvParent = gvMenuDay.Parent;

            //the following only needs to be done for Menu Averages
            if (DDLCategories.SelectedValue != "3" && DDLCategories.SelectedValue != "7") { return; }
            
            if (gvParent == null)
                return;

            SqlDataSource SqlDataSourceDayAverages = (SqlDataSource)gvParent.FindControl("SqlDataSourceDayAverages");
            SqlDataSourceDayAverages.DataBind();
            using (SqlDataReader dr = (SqlDataReader)SqlDataSourceDayAverages.Select(DataSourceSelectArguments.Empty))
            {
                while (dr.Read())
                {
                    int menuCatInt = int.Parse(dr["MenuCategory"].ToString());
                    switch (menuCatInt)
                    {
                        case 1:
                            HiddenField HFAverageEntreeBrk = (HiddenField)gvParent.FindControl("HFAverageEntreeBrk");
                            if (HFAverageEntreeBrk == null)
                                continue;
                            HFAverageEntreeBrk.Value = dr["AverageCost"].ToString();
                            break;
                        case 2:
                            HiddenField HFAverageFruitBrk = (HiddenField)gvParent.FindControl("HFAverageFruitBrk");
                            if (HFAverageFruitBrk == null)
                                continue;
                            HFAverageFruitBrk.Value = dr["AverageCost"].ToString();
                            break;
                        case 4:
                            HiddenField HFAverageEntreeLun = (HiddenField)gvParent.FindControl("HFAverageEntreeLun");
                            if (HFAverageEntreeLun == null)
                                continue;
                            HFAverageEntreeLun.Value = dr["AverageCost"].ToString();
                            break;
                        case 5:
                            HiddenField HFAverageFruitLun = (HiddenField)gvParent.FindControl("HFAverageFruitLun");
                            if (HFAverageFruitLun == null)
                                continue;
                            HFAverageFruitLun.Value = dr["AverageCost"].ToString();
                            break;
                        case 6:
                            HiddenField HFAverageVegLun = (HiddenField)gvParent.FindControl("HFAverageVegLun");
                            if (HFAverageVegLun == null)
                                continue;
                            HFAverageVegLun.Value = dr["AverageCost"].ToString();
                            break;
                        default:
                            break;
                    }
                }
            }


            //DataView dv = (DataView)SqlDataSourceDayAverages.Select(DataSourceSelectArguments.Empty);
            //foreach (DataRow dr in dv.Table.Rows)
            //{
            //    int menuCatInt = int.Parse(dr["MenuCategory"].ToString());
            //    switch (menuCatInt)
            //    {
            //        case 1:
            //            HiddenField HFAverageEntreeBrk = (HiddenField)gvParent.FindControl("HFAverageEntreeBrk");
            //            if (HFAverageEntreeBrk == null)
            //                continue;
            //            HFAverageEntreeBrk.Value = dr["AverageCost"].ToString();
            //            break;
            //        case 2:
            //            HiddenField HFAverageFruitBrk = (HiddenField)gvParent.FindControl("HFAverageFruitBrk");
            //            if (HFAverageFruitBrk == null)
            //                continue;
            //            HFAverageFruitBrk.Value = dr["AverageCost"].ToString();
            //            break;
            //        case 4:
            //            HiddenField HFAverageEntreeLun = (HiddenField)gvParent.FindControl("HFAverageEntreeLun");
            //            if (HFAverageEntreeLun == null)
            //                continue;
            //            HFAverageEntreeLun.Value = dr["AverageCost"].ToString();
            //            break;
            //        case 5:
            //            HiddenField HFAverageFruitLun = (HiddenField)gvParent.FindControl("HFAverageFruitLun");
            //            if (HFAverageFruitLun == null)
            //                continue;
            //            HFAverageFruitLun.Value = dr["AverageCost"].ToString();
            //            break;
            //        case 6:
            //            HiddenField HFAverageVegLun = (HiddenField)gvParent.FindControl("HFAverageVegLun");
            //            if (HFAverageVegLun == null)
            //                continue;
            //            HFAverageVegLun.Value = dr["AverageCost"].ToString();
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        protected enum ItemCats { Empty, BreakfastEntree, LunchEntree, Vegetable, Fruit, Other }

        protected void DDLFrtVeg_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DDLFrtVeg = (DropDownList)sender; 
            Control AveragesTable = (Control)FindParentControlOfType(DDLFrtVeg, "HtmlTable");//DDLFrtVeg.Parent.Parent.Parent;//td - tr - table
            if (AveragesTable == null) return;

            Control TableParent = (Control)AveragesTable.Parent;

            if(TableParent == null) return;

            Label LabelDayNumber = (Label)TableParent.FindControl("LabelDayNumber");
            int dayNumberInt=0;
            if(LabelDayNumber == null) return;
            else int.TryParse(LabelDayNumber.Text, out dayNumberInt);

            if(dayNumberInt < 1 || dayNumberInt > 30) return;


            bool fruitVegSelected = false;

            switch (DDLFrtVeg.ID)
            {
                case "DDLVeg1Outside":
                    TextBox TextBoxVeg1AvgOutside = (TextBox)AveragesTable.FindControl("TextBoxVeg1AvgOutside");
                    HiddenField HFAverageVegLun1 = (HiddenField)AveragesTable.FindControl("HFAverageVegLun");
                    if (TextBoxVeg1AvgOutside == null || HFAverageVegLun1==null)
                        return;
                    if (Decimal.TryParse(HFAverageVegLun1.Value, out Decimal decOutVeg1) == false)
                        decOutVeg1 = 0M;
                    if (string.IsNullOrEmpty(DDLFrtVeg.SelectedValue) || DDLFrtVeg.SelectedValue == "0")
                    {
                        TextBoxVeg1AvgOutside.Text = "0.00";
                        fruitVegSelected = false;
                    }
                    else
                    {
                        TextBoxVeg1AvgOutside.Text = decOutVeg1.ToString("F2");
                        fruitVegSelected = true;
                    }

                    saveDDLAvgState(dayNumberInt, ((int)ItemCats.Vegetable)*10, 1, fruitVegSelected);

                    break;
                case "DDLVeg2Outside":
                    TextBox TextBoxVeg2AvgOutside = (TextBox)AveragesTable.FindControl("TextBoxVeg2AvgOutside");
                    HiddenField HFAverageVegLun2 = (HiddenField)AveragesTable.FindControl("HFAverageVegLun");
                    if (TextBoxVeg2AvgOutside == null || HFAverageVegLun2 == null)
                        return;
                    if (Decimal.TryParse(HFAverageVegLun2.Value, out Decimal decOutVeg2) == false)
                        decOutVeg2 = 0M;
                    if (string.IsNullOrEmpty(DDLFrtVeg.SelectedValue) || DDLFrtVeg.SelectedValue == "0")
                    {
                        TextBoxVeg2AvgOutside.Text = "0.00";
                        fruitVegSelected = false;
                    }
                    else
                    {
                        TextBoxVeg2AvgOutside.Text = decOutVeg2.ToString("F2");
                        fruitVegSelected = true;
                    }

                    saveDDLAvgState(dayNumberInt, ((int)ItemCats.Vegetable)*10, 2, fruitVegSelected);

                    break;
                case "DDLFrt1Outside":
                    TextBox TextBoxFrt1AvgOutside = (TextBox)AveragesTable.FindControl("TextBoxFrt1AvgOutside");
                    HiddenField HFAverageFrtLun1;
                    if (DDLCategories.SelectedValue=="3")
                        HFAverageFrtLun1 = (HiddenField)AveragesTable.FindControl("HFAverageFruitBrk");
                    else
                        HFAverageFrtLun1 = (HiddenField)AveragesTable.FindControl("HFAverageFruitLun");
                    if (TextBoxFrt1AvgOutside == null || HFAverageFrtLun1 == null)
                        return;
                    if (Decimal.TryParse(HFAverageFrtLun1.Value, out Decimal decOutFrt1) == false)
                        decOutFrt1 = 0M;
                    if (string.IsNullOrEmpty(DDLFrtVeg.SelectedValue) || DDLFrtVeg.SelectedValue == "0")
                    {
                        TextBoxFrt1AvgOutside.Text = "0.00";
                        fruitVegSelected = false;
                    }
                    else
                    {
                        TextBoxFrt1AvgOutside.Text = decOutFrt1.ToString("F2");
                        fruitVegSelected = true;
                    }

                    saveDDLAvgState(dayNumberInt, ((int)ItemCats.Fruit)*10, 1, fruitVegSelected);

                    break;
                case "DDLFrt2Outside":
                    TextBox TextBoxFrt2AvgOutside = (TextBox)AveragesTable.FindControl("TextBoxFrt2AvgOutside");
                    HiddenField HFAverageFrtLun2;
                    if (DDLCategories.SelectedValue == "3")
                        HFAverageFrtLun2 = (HiddenField)AveragesTable.FindControl("HFAverageFruitBrk");
                    else
                        HFAverageFrtLun2 = (HiddenField)AveragesTable.FindControl("HFAverageFruitLun");
                    if (TextBoxFrt2AvgOutside == null || HFAverageFrtLun2 == null)
                        return;
                    if (Decimal.TryParse(HFAverageFrtLun2.Value, out Decimal decOutFrt2) == false)
                        decOutFrt2 = 0M;
                    if (string.IsNullOrEmpty(DDLFrtVeg.SelectedValue) || DDLFrtVeg.SelectedValue == "0")
                    {
                        TextBoxFrt2AvgOutside.Text = "0.00";
                        fruitVegSelected = false;
                    }
                    else
                    {
                        TextBoxFrt2AvgOutside.Text = decOutFrt2.ToString("F2");
                        fruitVegSelected = true;
                    }

                    saveDDLAvgState(dayNumberInt, ((int)ItemCats.Fruit)*10, 2, fruitVegSelected);

                    break;
                default:
                    break;
            }


            UpdateMenuDayTotal(TableParent);
            /**** Also need to update the overall averages - Average Food Cost Per Day, Total Food Cost ****/
            getOverallSummary();
        }

        #region DDLAvgStateQueries
        protected void saveDDLAvgState(int dayNumber, int fruitVegCatIDInt, int ordinal, bool selected)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString;
            if(doesDDLAvgStateExist(dayNumber, fruitVegCatIDInt, ordinal))
                queryString = "UPDATE [MenuAnalysisEntry] SET [Quantity]=@Quantity, [ItemID]=@ItemID, [ItemCategoryID]=@ItemCategoryID  WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [MenuCategory]=@MenuCategory AND [DayNumber]=@DayNumber AND [ORD]=@ORD";
            else
                queryString = "INSERT INTO [MenuAnalysisEntry] ([ProgramYear],[DistrictID],[SchoolNum],[MenuCategory],[DayNumber],[ORD],[ItemID],[ItemCategoryID],[Quantity]) SELECT @ProgramYear,@DistrictID,@SchoolNum,@MenuCategory,@DayNumber,@ORD,@ItemID,@ItemCategoryID,@Quantity";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = DropDownListSchools.SelectedValue;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_menuCat = new SqlParameter();
                dbParam_menuCat.ParameterName = "@MenuCategory";
                dbParam_menuCat.Value = DDLCategories.SelectedValue;
                dbParam_menuCat.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_menuCat);

                SqlParameter dbParam_dayNum = new SqlParameter();
                dbParam_dayNum.ParameterName = "@DayNumber";
                dbParam_dayNum.Value = dayNumber;
                dbParam_dayNum.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNum);

                SqlParameter dbParam_ordinal = new SqlParameter();
                dbParam_ordinal.ParameterName = "@ORD";
                dbParam_ordinal.Value = (fruitVegCatIDInt*10) + ordinal;/* This will always be 301,302,401,402 depending on cat and ordinal  */
                dbParam_ordinal.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_ordinal);

                SqlParameter dbParam_itemID = new SqlParameter();
                dbParam_itemID.ParameterName = "@ItemID";
                dbParam_itemID.Value = 1;//Special Item IDs are always 1
                dbParam_itemID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemID);

                SqlParameter dbParam_quantity = new SqlParameter();
                dbParam_quantity.ParameterName = "@Quantity";
                dbParam_quantity.Value = selected ? 1M : 0M;
                dbParam_quantity.DbType = DbType.Decimal;
                dbCommand.Parameters.Add(dbParam_quantity);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = fruitVegCatIDInt;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);

                try
                {
                    dbConnection.Open();

                    dbCommand.ExecuteNonQuery();
                    
                }
                catch(SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while saving the average selection state.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while saving the average selection state.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


        }

        protected decimal loadDDLAvgState(int dayNumber, int fruitVegCatIDInt, int ordinal) 
        {
            decimal returnFruitVegValue = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT [Quantity] FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [MenuCategory]=@MenuCategory AND [DayNumber]=@DayNumber AND [ORD]=@ORD AND [ItemID]=@ItemID AND [ItemCategoryID]=@ItemCategoryID";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = DropDownListSchools.SelectedValue;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_menuCat = new SqlParameter();
                dbParam_menuCat.ParameterName = "@MenuCategory";
                dbParam_menuCat.Value = DDLCategories.SelectedValue;
                dbParam_menuCat.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_menuCat);

                SqlParameter dbParam_dayNum = new SqlParameter();
                dbParam_dayNum.ParameterName = "@DayNumber";
                dbParam_dayNum.Value = dayNumber;
                dbParam_dayNum.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNum);

                SqlParameter dbParam_ordinal = new SqlParameter();
                dbParam_ordinal.ParameterName = "@ORD";
                dbParam_ordinal.Value = (fruitVegCatIDInt*10) + ordinal;
                dbParam_ordinal.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_ordinal);

                SqlParameter dbParam_itemID = new SqlParameter();
                dbParam_itemID.ParameterName = "@ItemID";
                dbParam_itemID.Value = 1;
                dbParam_itemID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemID);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = fruitVegCatIDInt;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);


                try
                {
                    dbConnection.Open();

                    object objRetQty = dbCommand.ExecuteScalar();

                    if (objRetQty == null)
                        returnFruitVegValue = 0;
                    else
                    {
                        if (decimal.TryParse(objRetQty.ToString(), out returnFruitVegValue) == false)
                            returnFruitVegValue = 0;
                    }

                }
                catch (Exception)
                {
                    returnFruitVegValue = 0;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return returnFruitVegValue;
        }

        protected void getOverallSummary()
        {
            //the following only needs to be done for Menu Averages
            if (DDLCategories.SelectedValue != "3" && DDLCategories.SelectedValue != "7") { return; }

            int totalDays = 0;
            decimal avgCostPerDay = 0M;
            decimal avgEntreeCost = 0M;
            decimal totalFoodCost = 0M;
            decimal totalEntreeCost = 0M;

            SqlDataSourceAveragesOverall.DataBind();
            using (SqlDataReader dr = (SqlDataReader)SqlDataSourceAveragesOverall.Select(DataSourceSelectArguments.Empty))
            {
                while (dr.Read())
                {
                    int menuCatInt = int.Parse(dr["MenuCategory"].ToString());

                    //Ensure Breakfast averages are used with breakfast and lunch averages with lunch
                    if (DDLCategories.SelectedValue == "3" && menuCatInt > 3)
                        continue;
                    if (DDLCategories.SelectedValue == "7" && menuCatInt <= 3)
                        continue;

                    switch (menuCatInt)
                    {
                        case 1://Breakfast Entree
                            totalDays = (int)dr["NumDays"];
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            totalEntreeCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 2://Breakfast Fruit
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 3://Breakfast Other
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 4://Lunch Entree
                            totalDays = (int)dr["NumDays"];
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            totalEntreeCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 5://Lunch Fruit
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 6://Lunch Veg
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            break;
                        case 7://Lunch Other
                            totalFoodCost += (decimal)dr["SumAvgCost"];
                            break;
                        default:
                            break;
                    }
                }
            }

            LabelNumDays.Text = totalDays.ToString();
            LabelTotalFoodCost.Text = totalFoodCost.ToString("F2");
            decimal decAvgEntree = totalDays == 0M ? 0M : (totalEntreeCost / totalDays);
            decimal decAvgMeal = totalDays == 0M ? 0M : (totalFoodCost / totalDays);
            LabelAvgEntreeCost.Text = decAvgEntree.ToString("F3");
            LabelAvgFoodCostPerDay.Text = decAvgMeal.ToString("F3");

            decimal decLowerEntree = decimal.MaxValue;
            decimal decUpperEntree = decimal.MaxValue;
            decimal decLowerMeal = decimal.MaxValue;
            decimal decUpperMeal = decimal.MaxValue;

            decimal.TryParse(HFEntreeMidLower.Value, out decLowerEntree);
            decimal.TryParse(HFEntreeMidUpper.Value, out decUpperEntree);
            decimal.TryParse(HFTotalMealMidLower.Value, out decLowerMeal);
            decimal.TryParse(HFTotalMealMidUpper.Value, out decUpperMeal);

            //Entree
            if (decAvgEntree < decLowerEntree)
                LabelAvgEntreeCost.ForeColor = System.Drawing.Color.LimeGreen;
            else if (decAvgEntree >= decLowerEntree && decAvgEntree <= decUpperEntree)
                LabelAvgEntreeCost.ForeColor = System.Drawing.Color.Goldenrod;
            else if (decAvgEntree > decUpperEntree)
                LabelAvgEntreeCost.ForeColor = System.Drawing.Color.Red;
            else
                LabelAvgEntreeCost.ForeColor = System.Drawing.Color.Black;

            //Total Meal
            if (decAvgMeal < decLowerMeal)
                LabelAvgFoodCostPerDay.ForeColor = System.Drawing.Color.LimeGreen;
            else if (decAvgMeal >= decLowerMeal && decAvgMeal <= decUpperMeal)
                LabelAvgFoodCostPerDay.ForeColor = System.Drawing.Color.Goldenrod;
            else if (decAvgMeal > decUpperMeal)
                LabelAvgFoodCostPerDay.ForeColor = System.Drawing.Color.Red;
            else
                LabelAvgFoodCostPerDay.ForeColor = System.Drawing.Color.Black;

        }

        protected bool doesDDLAvgStateExist(int dayNumber, int fruitVegCatIDInt, int ordinal)
        {
            bool recordExists = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT COUNT(*) FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [MenuCategory]=@MenuCategory AND [DayNumber]=@DayNumber AND [ORD]=@ORD ";//AND [ItemID]=@ItemID AND [ItemCategoryID]=@ItemCategoryID

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = DropDownListSchools.SelectedValue;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_menuCat = new SqlParameter();
                dbParam_menuCat.ParameterName = "@MenuCategory";
                dbParam_menuCat.Value = DDLCategories.SelectedValue;
                dbParam_menuCat.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_menuCat);

                SqlParameter dbParam_dayNum = new SqlParameter();
                dbParam_dayNum.ParameterName = "@DayNumber";
                dbParam_dayNum.Value = dayNumber;
                dbParam_dayNum.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNum);

                SqlParameter dbParam_ordinal = new SqlParameter();
                dbParam_ordinal.ParameterName = "@ORD";
                dbParam_ordinal.Value = (fruitVegCatIDInt * 10) + ordinal;
                dbParam_ordinal.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_ordinal);

                SqlParameter dbParam_itemID = new SqlParameter();
                dbParam_itemID.ParameterName = "@ItemID";
                dbParam_itemID.Value = 1;
                dbParam_itemID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemID);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = fruitVegCatIDInt;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);


                try
                {
                    dbConnection.Open();

                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object objRetQty = dbCommand.ExecuteScalar();

                    if (objRetQty == null)
                        recordExists = false;
                    else if(int.Parse(objRetQty.ToString())>0)
                        recordExists = true;

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while checking for the average selection state.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while checking for the average selection state.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return recordExists;
        }
        #endregion

        protected void SqlDataSourceDayMenuEntry_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e.Command.Parameters["@ItemID"].Value.ToString() == "0")
            {
                e.Cancel = true;
                return;
            }
        }

        protected void SqlDataSourceDayMenuEntry_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Updated", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowDialog("Error", MessageType.Error, "Menu Item was not updated.", e.Exception.Message);
            }

        }

        /// <summary>
        /// Used to add or remove a fruit or vegetable item from the menu for all days
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplyFruitVegAllChanges(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int ordinal = 0;
            int fruitVegInt = 0;
            bool addAverage = false;

            if (DDLCategories.SelectedValue!="3" && DDLCategories.SelectedValue!="7" || btn==null) { return; }


            switch (btn.ID)
            {
                case "IButtonAddVeg1":
                    ordinal = 1;
                    fruitVegInt = ((int)ItemCats.Vegetable) * 10;
                    addAverage = true;
                    break;
                case "IButtonAddVeg2":
                    ordinal = 2;
                    fruitVegInt = ((int)ItemCats.Vegetable) * 10;
                    addAverage = true;
                    break;
                case "IButtonAddFrt1":
                    ordinal = 1;
                    fruitVegInt = ((int)ItemCats.Fruit) * 10;
                    addAverage = true;
                    break;
                case "IButtonAddFrt2":
                    ordinal = 2;
                    fruitVegInt = ((int)ItemCats.Fruit) * 10;
                    addAverage = true;
                    break;
                case "IButtonRemoveVeg1":
                    ordinal = 1;
                    fruitVegInt = ((int)ItemCats.Vegetable) * 10;
                    addAverage = false;
                    break;
                case "IButtonRemoveVeg2":
                    ordinal = 2;
                    fruitVegInt = ((int)ItemCats.Vegetable) * 10;
                    addAverage = false;
                    break;
                case "IButtonRemoveFrt1":
                    ordinal = 1;
                    fruitVegInt = ((int)ItemCats.Fruit) * 10;
                    addAverage = false;
                    break;
                case "IButtonRemoveFrt2":
                    ordinal = 2;
                    fruitVegInt = ((int)ItemCats.Fruit) * 10;
                    addAverage = false;
                    break;
                default:
                    break;
            }

            if (ordinal == 0 || fruitVegInt == 0 )
                return;

            for(int i = 1; i <= 30; i++) 
            {
                saveDDLAvgState(i, fruitVegInt, ordinal, addAverage);
            }

            ListViewMenuDays.DataBind();

        }

        /// <summary>
        /// Used to add or remove a menu item from the menu for all days
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplyMenuItemAllChanges(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            bool addItem = false;

            if (DDLCategories.SelectedValue != "3" && DDLCategories.SelectedValue != "7" || btn == null) { return; }

            if (int.TryParse(DDLMenuItemsAll.SelectedValue, out int itemIDInt) == false || itemIDInt==0)
                return;

            switch (btn.ID)
            {
                case "IButtonAddItem":
                    addItem = true;
                    break;
                case "iButtonRemoveItem":
                    addItem = false;
                    break;
                default:
                    break;
            }


            for (int i = 1; i <= 30; i++)
            {
                AddRemoveMenuItem(i, itemIDInt, addItem);
            }

            ListViewMenuDays.DataBind();

        }

        protected void AddRemoveMenuItem(int dayNumber, int itemID, bool addItem)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString;

            if (addItem)
                queryString = "INSERT INTO [MenuAnalysisEntry] ([ProgramYear],[DistrictID],[SchoolNum],[MenuCategory],[DayNumber],[ORD],[ItemID],[ItemCategoryID],[Quantity]) "
                            + " VALUES(@ProgramYear, @DistrictID, @SchoolNum, @MenuCategory, @DayNumber, "
                            + "(SELECT ISNULL(MAX(ORD), 99) + 1 FROM [MenuAnalysisEntry] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND SchoolNum = @SchoolNum AND MenuCategory = @MenuCategory AND DayNumber = @DayNumber AND ORD>=100 AND ORD<300), "
                            + "@ItemID,@ItemCategoryID,@Quantity) ";
            else
                queryString = "DELETE FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory AND DayNumber=@DayNumber AND ItemID=@ItemID" +
                    " AND ORD =ISNULL((SELECT MAX(ORD) FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory AND DayNumber=@DayNumber AND ItemID=@ItemID AND ItemCategoryID=@ItemCategoryID ),0) ";


            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = DropDownListSchools.SelectedValue;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_menuCat = new SqlParameter();
                dbParam_menuCat.ParameterName = "@MenuCategory";
                dbParam_menuCat.Value = DDLCategories.SelectedValue;
                dbParam_menuCat.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_menuCat);

                SqlParameter dbParam_dayNum = new SqlParameter();
                dbParam_dayNum.ParameterName = "@DayNumber";
                dbParam_dayNum.Value = dayNumber;
                dbParam_dayNum.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNum);

                SqlParameter dbParam_itemID = new SqlParameter();
                dbParam_itemID.ParameterName = "@ItemID";
                dbParam_itemID.Value = itemID;
                dbParam_itemID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemID);

                SqlParameter dbParam_quantity = new SqlParameter();
                dbParam_quantity.ParameterName = "@Quantity";
                dbParam_quantity.Value = 1M;
                dbParam_quantity.DbType = DbType.Decimal;
                dbCommand.Parameters.Add(dbParam_quantity);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = 5M;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);

                try
                {
                    dbConnection.Open();

                    dbCommand.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while saving the average selection state.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while saving the average selection state.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


        }

        protected void SqlDataSourceRanges_DataBinding(object sender, EventArgs e)
        {
            //for the averages - we want this to use the breakfast or lunch ranges instead of pulling the other
            if (DDLCategories.SelectedValue == "3")//breakfast average
                ((SqlDataSource)sender).SelectParameters["ItemCategoryID"].DefaultValue = "1";
            else if (DDLCategories.SelectedValue == "7")//Lunch Average
                ((SqlDataSource)sender).SelectParameters["ItemCategoryID"].DefaultValue = "2";
            else
                ((SqlDataSource)sender).SelectParameters["ItemCategoryID"].DefaultValue = HFItemCategoryID.Value;
        }

        protected enum SchoolType
        {
            Unknown,
            Elementary,
            Middle,
            Secondary
        }

        /// <summary>
        /// button that opens the modal box for copying the menu analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void iCopy_Click(object sender, EventArgs e)
        {
            LabelMenuCopyTitle.Text = "Copy " + DDLCategories.SelectedItem.Text + " Menus from " + DropDownListSchools.SelectedItem.Text;
            if (DDLCategories.SelectedValue == "4")
            {
                if (HFSchoolType.Value == "1")
                {
                    LabelLunchEntreeMessageElem.Visible = true;
                    LabelLunchEntreeMessageNonElem.Visible = false;
                }
                else 
                {
                    LabelLunchEntreeMessageElem.Visible = false;
                    LabelLunchEntreeMessageNonElem.Visible = true;
                }

                LabelLunchEntreeMessage2.Visible = true;
                int.TryParse(HFSchoolType.Value, out int schoolInt);
                LabelLunchEntreeMessage2.InnerText = "Current School Type: " + (SchoolType)schoolInt;

            }
            else
            {
                LabelLunchEntreeMessageElem.Visible = false;
                LabelLunchEntreeMessageNonElem.Visible = false;
                LabelLunchEntreeMessage2.Visible = false;
            }

            RegisterDOMReadyScript("modalBox", "$('#myModal').modal('show');");
        }

        protected void btnCopy_Click(object sender, EventArgs e)
        {
            bool copyComplete=false;
            int.TryParse(HFSchoolType.Value, out int schoolTypeInt);//1=Elementary, 2=Middle, 3=Secondary
            copyComplete=CopyMenuAnalysis(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLStartingDay.SelectedValue), int.Parse(DDLEndingDay.SelectedValue), int.Parse(DDLDestinationSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue), schoolTypeInt);
            if (copyComplete)
            {
                RegisterDOMReadyScript("pop", "$('#myModal').modal('hide');$('.modal-backdrop').remove();ShowPopup('Success','Records were copied from " + DropDownListSchools.SelectedItem.Text + " to " + DDLDestinationSchools.SelectedItem.Text + ".','" + MessageType.Success.ToString() + "');");
            }
        }

        private bool CopyMenuAnalysis(string progYear, string distID, int srcSchool, int startingDay, int endingDay, int destSchool, int menuCat, int schoolType)
        {
            bool success = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string sqlCopyMenuAnalysis="";

            sqlCopyMenuAnalysis = "DELETE [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND ((@destSchool=-1 AND [SchoolNum]<>@srcSchool) OR [SchoolNum]=@destSchool) AND [DayNumber] BETWEEN @StartingDay AND @EndingDay AND [MenuCategory]=@MenuCategory";
            if (menuCat == 4)
            {
                if (schoolType == 1)
                    sqlCopyMenuAnalysis += " AND [SchoolNum] IN (SELECT SchoolNum FROM DistrictSchools WHERE SchoolType=1 AND DistrictID=@DistrictID);";
                else
                    sqlCopyMenuAnalysis += " AND [SchoolNum] IN (SELECT SchoolNum FROM DistrictSchools WHERE SchoolType>1 AND DistrictID=@DistrictID);";
            }
            else
                sqlCopyMenuAnalysis += ";";

            sqlCopyMenuAnalysis+="INSERT INTO [MenuAnalysisEntry]([ProgramYear],[DistrictID],[SchoolNum],[MenuCategory],[DayNumber],[ORD],[ItemID],[ItemCategoryID],[Quantity],[WellnessWed]) " +
                "SELECT MAE.[ProgramYear],MAE.[DistrictID],DS.[SchoolNum],[MenuCategory],[DayNumber],[ORD],[ItemID],[ItemCategoryID],[Quantity],[WellnessWed] " +
                "FROM [MenuAnalysisEntry] MAE " +
                "CROSS APPLY DistrictSchools DS " +
                "WHERE DS.Inactive=0 AND MAE.[ProgramYear]=@ProgramYear AND MAE.[DistrictID]=@DistrictID AND MAE.[DistrictID]=DS.DistrictID AND MAE.[SchoolNum]=@srcSchool " +
                "AND [DayNumber] BETWEEN @StartingDay AND @EndingDay AND ((@destSchool=-1 AND DS.[SchoolNum]<>@srcSchool) OR DS.[SchoolNum]=@destSchool) AND MAE.[MenuCategory]=@MenuCategory ";

            if(menuCat == 4)
            {
                if (schoolType == 1)
                    sqlCopyMenuAnalysis += " AND [SchoolType]=1;";
                else
                    sqlCopyMenuAnalysis += " AND [SchoolType]>1;";
            }
            else
                sqlCopyMenuAnalysis += ";";


            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = sqlCopyMenuAnalysis;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = progYear;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = distID;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@srcSchool";
                dbParam_schoolNum.Value = srcSchool;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_destSchoolNum = new SqlParameter();
                dbParam_destSchoolNum.ParameterName = "@destSchool";
                dbParam_destSchoolNum.Value = destSchool;
                dbParam_destSchoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_destSchoolNum);

                SqlParameter dbParam_dayNumStart = new SqlParameter();
                dbParam_dayNumStart.ParameterName = "@StartingDay";
                dbParam_dayNumStart.Value = startingDay;
                dbParam_dayNumStart.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNumStart);

                SqlParameter dbParam_dayNumEnd = new SqlParameter();
                dbParam_dayNumEnd.ParameterName = "@EndingDay";
                dbParam_dayNumEnd.Value = endingDay;
                dbParam_dayNumEnd.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_dayNumEnd);

                SqlParameter dbParam_menuCat = new SqlParameter();
                dbParam_menuCat.ParameterName = "@MenuCategory";
                dbParam_menuCat.Value = menuCat;
                dbParam_menuCat.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_menuCat);

                try
                {
                    dbConnection.Open();

                    int recordCount = dbCommand.ExecuteNonQuery();
                    if (recordCount > 0)
                        success = true;

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while copying menus.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while copying menus.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return success;
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool deleteComplete = false;
            deleteComplete = DeleteCurrentMenuCat(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, int.Parse(DropDownListSchools.SelectedValue), int.Parse(DDLCategories.SelectedValue));
            if (deleteComplete)
            {
                RegisterDOMReadyScript("pop", "ShowPopup('Success','"+ DDLCategories.SelectedItem.Text +" records were deleted for " + DropDownListSchools.SelectedItem.Text + ".','" + MessageType.Success.ToString() + "');");
                ListViewMenuDays.DataBind();
            }
        }

        private bool DeleteCurrentMenuCat(string progYear, string distID, int srcSchool, int menuCat)
        {
            bool success = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string sqlDeleteMenuAnalysis = "DELETE [MenuAnalysisEntry] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory";
            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = sqlDeleteMenuAnalysis;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = progYear;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = distID;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = srcSchool;
                dbParam_schoolNum.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_MenuCat = new SqlParameter();
                dbParam_MenuCat.ParameterName = "@MenuCategory";
                dbParam_MenuCat.Value = menuCat;
                dbParam_MenuCat.DbType = DbType.Int16;
                dbCommand.Parameters.Add(dbParam_MenuCat);

               try
                {
                    dbConnection.Open();

                    int recordCount = dbCommand.ExecuteNonQuery();
                    if (recordCount > 0)
                        success = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while clearing out the current menu category.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while clearing out the current menu category.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return success;
        }




        protected void GridViewStandards_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Program was throwing a JavaScript error when deleting a standard stating that there wasn't a RowDeleting handler.  This is a dummy handler to prevent the error.
        }

        protected void IButtonPrintAvgs_Click(object sender, EventArgs e)
        {
            Session["PrintProgramYear"] = DDLProgramYears.SelectedValue;
            Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue;
            Session["PrintMenuCategory"] = DDLCategories.SelectedValue;
            Response.Redirect("~/Print/PrintMenuAverages.aspx");
        }

        protected void IButtonPrintMenus_Click(object sender, EventArgs e)
        {
            Session["PrintProgramYear"] = DDLProgramYears.SelectedValue;
            Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue;
            Session["PrintMenuCategory"] = DDLCategories.SelectedValue;
            Session["PrintSchoolNum"] = DropDownListSchools.SelectedValue;
            Response.Redirect("~/Print/PrintMADailyMenus.aspx");
        }

        protected void gvMenuDay_RowCRUD(object sender, GridViewRowEventArgs e)
        {
            /**** Also need to update the overall averages - Average Food Cost Per Day, Total Food Cost ****/
            /**** This only updates for Brk or Lun overall ****/
            getOverallSummary();
        }

        protected void gvMenuDay_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            /**** Also need to update the overall averages - Average Food Cost Per Day, Total Food Cost ****/
            /**** This only updates for Brk or Lun overall ****/
            getOverallSummary();
        }

        protected void gvMenuDay_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            /**** Also need to update the overall averages - Average Food Cost Per Day, Total Food Cost ****/
            /**** This only updates for Brk or Lun overall ****/
            getOverallSummary();
        }
    }
}