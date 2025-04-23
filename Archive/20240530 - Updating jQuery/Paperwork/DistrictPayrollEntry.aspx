<%@ Page Title="District Payroll Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DistrictPayrollEntry.aspx.cs" Inherits="Paperwork.DistrictPayrollEntry" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
          .custom-combobox {
            position: relative;
            display: inline-block;
          }
          .custom-combobox-toggle {
            position: absolute;
            top: 0;
            bottom: 0;
            margin-left: -1px;
            padding: 0;
          }
          .custom-combobox-inputEmp {
            margin: 0;
            padding: 5px 10px;
            width:175px;
          }
          .custom-combobox-inputSchool {
            margin: 0;
            padding: 5px 10px;
            width:100px;
          }
          .custom-combobox-inputCategory {
            margin: 0;
            padding: 5px 10px;
            width:100px;
          }
    </style>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);

        var focusedElementId = "";

        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                CreateAutoCompleteEmps();
                CreateAutoCompleteSchool();
                //CreateAutoCompleteCategory();
                BindEvents();
            };

            if (focusedElementId != "") {
                $("#" + focusedElementId).focus();
            }
        };

        function BeginRequest(sender, args) {
            var fe = document.activeElement;
            if (fe != null) {
                focusedElementId = fe.id;
            } else {
                focusedElementId = "";
            }
        };


        // For gridview resizing
        var h = $(window).height();
        var topAndBottom = 180;
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the height variable
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#GridViewHeightController").height(h - topAndBottom);
            }, 200);

        });

        $(function () {
            CreateAutoCompleteEmps();
            CreateAutoCompleteSchool();
            //CreateAutoCompleteCategory();
            BindEvents();
            
        });

        function BindEvents() {
            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            //my JQuery code to maintain scroll position
            //When GridViewHeightController div is scrolled - store the scroll bar top pixel value in hidden field
            $("#GridViewHeightController").scroll(function () {
                $("[id*=hdnScrollPos]").val($("#GridViewHeightController").scrollTop());
            });

            //After postback - restore the scroll position
            $('#GridViewHeightController').scrollTop($("[id*=hdnScrollPos]").val());

            //Program Year limits
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            $(function () {
                $("[id*=TextBoxPayrollStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightDays,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxPayrollEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightDays,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxFPayDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            updateDatesArray();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {

                    //This code moves between focusable elements. 
                    var $canfocus = $(':focusable').not("a[title|='Show All Items']");//"title", "Show All Items"
                    //var index = $canfocus.index(document.activeElement) + 1;
                    var index = $canfocus.index(this) + 1;

                    if (index >= $canfocus.length) index = 0;
                    $canfocus.eq(index).focus();//Handles Buttons
                    $canfocus.eq(index).select();//helps with dropdowns

                }
            });

            


        }


        //Date Functions for colorized calendar
        var dateArray;
        var dateArrayLocked;

        function highlightDays(date) {
            //if no dates, return immediately
            //if (dateArray.length === 0 && dateArrayLocked.length===0)
            //    return [true, '', ''];
            if (dateArray[0] != "") {
                for (var i = 0; i < dateArray.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArray[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarHighlighted', 'Payroll Entered'];
                    }
                }
            }

            if (dateArrayLocked[0] != "") {
                for (var i = 0; i < dateArrayLocked.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArrayLocked[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarLocked', 'Payroll Entered (Locked)'];
                    }
                }
            }

            return [true, '', ''];
        };

        function updateDatesArray() {
            dateArray = $('[id*=HFEnteredPayrollDates]').val().split(','); 
            dateArrayLocked = $('[id*=HFLockedPayrollDates]').val().split(',');
        };

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });



        function CreateAutoCompleteEmps () {
            $.widget("custom.combobox", {
                _create: function () {
                    this.wrapper = $("<span>")
                        .addClass("custom-combobox")
                        .insertAfter(this.element);

                    this.element.hide();
                    this._createAutocomplete();
                    this._createShowAllButton();
                },

                _createAutocomplete: function () {
                    var selected = this.element.children(":selected"),
                        value = selected.val() ? selected.text() : "0";

                    this.input = $("<input>")
                        .appendTo(this.wrapper)
                        .val(value)
                        .attr("title", "")
                        .attr("id", "ComboBoxEmps")
                        .addClass("custom-combobox-inputEmp ui-widget ui-widget-content ui-state-default ui-corner-left")
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: $.proxy(this, "_source")
                        })
                        .tooltip({
                            classes: {
                                "ui-tooltip": "ui-state-highlight"
                            }
                        });
                        

                    this._on(this.input, {
                        autocompleteselect: function (event, ui) {
                            ui.item.option.selected = true;
                            this._trigger("select", event, {
                                item: ui.item.option
                            });
                        },

                        autocompletechange: "_removeIfInvalid"
                    });
                },

                _createShowAllButton: function () {
                    var input = this.input,
                        wasOpen = false;

                    $("<a>")
                        .attr("tabIndex", -1)
                        .attr("title", "Show All Items")
                        .tooltip()
                        .appendTo(this.wrapper)
                        .button({
                            icons: {
                                primary: "ui-icon-triangle-1-s"
                            },
                            text: false
                        })
                        .removeClass("ui-corner-all")
                        .addClass("custom-combobox-toggle ui-corner-right")
                        .on("mousedown", function () {
                            wasOpen = input.autocomplete("widget").is(":visible");
                        })
                        .on("click", function () {
                            input.trigger("focus");

                            // Close if already visible
                            if (wasOpen) {
                                return;
                            }

                            // Pass empty string as value to search for, displaying all results
                            input.autocomplete("search", "");
                        });
                },

                _source: function (request, response) {
                    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                    response(this.element.children("option").map(function () {
                        var text = $(this).text();
                        if (this.value && (!request.term || matcher.test(text)))
                            return {
                                label: text,
                                value: text,
                                option: this
                            };
                    }));
                },

                _removeIfInvalid: function (event, ui) {

                    // Selected an item, nothing to do
                    if (ui.item) {
                        //Added 11/22/2022 to force postback which wasn't happening
                        __doPostBack(this.id, '');
                        return;
                    }

                    // Search for a match (case-insensitive)
                    var value = this.input.val(),
                        valueLowerCase = value.toLowerCase(),
                        valid = false;
                    this.element.children("option").each(function () {
                        if ($(this).text().toLowerCase() === valueLowerCase) {
                            this.selected = valid = true;
                            //Added 11/22/2022 to force postback which wasn't happening after match was found
                            __doPostBack(this.id, '');
                            return false;
                        }
                    });

                    // Found a match, nothing to do
                    if (valid) {
                        return;
                    }

                    // Remove invalid value
                    this.input
                        .val("")
                        .attr("title", value + " didn't match any item")
                        .tooltip("open");
                    this.element.val("");
                    this._delay(function () {
                        this.input.tooltip("close").attr("title", "");
                    }, 2500);
                    this.input.autocomplete("instance").term = "";
                },

                _destroy: function () {
                    this.wrapper.remove();
                    this.element.show();
                }
            });

            //activate the autocomplete combobox
            $("[id*=DropDownListFEmps]").combobox();

        }

        function CreateAutoCompleteSchool() {
            $.widget("custom.combobox", {
                _create: function () {
                    this.wrapper = $("<span>")
                        .addClass("custom-combobox")
                        .insertAfter(this.element);

                    this.element.hide();
                    this._createAutocomplete();
                    this._createShowAllButton();
                },

                _createAutocomplete: function () {
                    var selected = this.element.children(":selected"),
                        value = selected.val() ? selected.text() : "0";

                    this.input = $("<input>")
                        .appendTo(this.wrapper)
                        .val(value)
                        .attr("title", "")
                        .attr("id", "ComboBoxSchools")
                        .addClass("custom-combobox-inputSchool ui-widget ui-widget-content ui-state-default ui-corner-left")
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: $.proxy(this, "_source")
                        })
                        .tooltip({
                            classes: {
                                "ui-tooltip": "ui-state-highlight"
                            }
                        });


                    this._on(this.input, {
                        autocompleteselect: function (event, ui) {
                            ui.item.option.selected = true;
                            this._trigger("select", event, {
                                item: ui.item.option
                            });
                        },

                        autocompletechange: "_removeIfInvalid"
                    });
                },

                _createShowAllButton: function () {
                    var input = this.input,
                        wasOpen = false;

                    $("<a>")
                        .attr("tabIndex", -1)
                        .attr("title", "Show All Items")
                        .tooltip()
                        .appendTo(this.wrapper)
                        .button({
                            icons: {
                                primary: "ui-icon-triangle-1-s"
                            },
                            text: false
                        })
                        .removeClass("ui-corner-all")
                        .addClass("custom-combobox-toggle ui-corner-right")
                        .on("mousedown", function () {
                            wasOpen = input.autocomplete("widget").is(":visible");
                        })
                        .on("click", function () {
                            input.trigger("focus");

                            // Close if already visible
                            if (wasOpen) {
                                return;
                            }

                            // Pass empty string as value to search for, displaying all results
                            input.autocomplete("search", "");
                        });
                },

                _source: function (request, response) {
                    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                    response(this.element.children("option").map(function () {
                        var text = $(this).text();
                        if (this.value && (!request.term || matcher.test(text)))
                            return {
                                label: text,
                                value: text,
                                option: this
                            };
                    }));
                },

                _removeIfInvalid: function (event, ui) {

                    // Selected an item, nothing to do
                    if (ui.item) {
                        return;
                    }

                    // Search for a match (case-insensitive)
                    var value = this.input.val(),
                        valueLowerCase = value.toLowerCase(),
                        valid = false;
                    this.element.children("option").each(function () {
                        if ($(this).text().toLowerCase() === valueLowerCase) {
                            this.selected = valid = true;
                            return false;
                        }
                    });

                    // Found a match, nothing to do
                    if (valid) {
                        return;
                    }

                    // Remove invalid value
                    this.input
                        .val("")
                        .attr("title", value + " didn't match any item")
                        .tooltip("open");
                    this.element.val("");
                    this._delay(function () {
                        this.input.tooltip("close").attr("title", "");
                    }, 2500);
                    this.input.autocomplete("instance").term = "";
                },

                _destroy: function () {
                    this.wrapper.remove();
                    this.element.show();
                }
            });

            //activate the autocomplete combobox
            $("[id*=DropDownListFSchools]").combobox();

        }

        function CreateAutoCompleteCategory() {
            $.widget("custom.combobox", {
                _create: function () {
                    this.wrapper = $("<span>")
                        .addClass("custom-combobox")
                        .insertAfter(this.element);

                    this.element.hide();
                    this._createAutocomplete();
                    this._createShowAllButton();
                },

                _createAutocomplete: function () {
                    var selected = this.element.children(":selected"),
                        value = selected.val() ? selected.text() : "0";

                    this.input = $("<input>")
                        .appendTo(this.wrapper)
                        .val(value)
                        .attr("title", "")
                        .attr("id", "ComboBoxCategories")

                        .attr("AutoPostBack", "true")
                        .addClass("custom-combobox-inputCategory ui-widget ui-widget-content ui-state-default ui-corner-left")
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: $.proxy(this, "_source")
                        })
                        .tooltip({
                            classes: {
                                "ui-tooltip": "ui-state-highlight"
                            }
                        });


                    this._on(this.input, {
                        autocompleteselect: function (event, ui) {
                            ui.item.option.selected = true;
                            this._trigger("select", event, {
                                item: ui.item.option
                            });
                        },

                        autocompletechange: "_removeIfInvalid"
                    });
                },

                _createShowAllButton: function () {
                    var input = this.input,
                        wasOpen = false;

                    $("<a>")
                        .attr("tabIndex", -1)
                        .attr("title", "Show All Items")
                        .tooltip()
                        .appendTo(this.wrapper)
                        .button({
                            icons: {
                                primary: "ui-icon-triangle-1-s"
                            },
                            text: false
                        })
                        .removeClass("ui-corner-all")
                        .addClass("custom-combobox-toggle ui-corner-right")
                        .on("mousedown", function () {
                            wasOpen = input.autocomplete("widget").is(":visible");
                        })
                        .on("click", function () {
                            input.trigger("focus");

                            // Close if already visible
                            if (wasOpen) {
                                return;
                            }

                            // Pass empty string as value to search for, displaying all results
                            input.autocomplete("search", "");
                        });
                },

                _source: function (request, response) {
                    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                    response(this.element.children("option").map(function () {
                        var text = $(this).text();
                        if (this.value && (!request.term || matcher.test(text)))
                            return {
                                label: text,
                                value: text,
                                option: this
                            };
                    }));
                },

                _removeIfInvalid: function (event, ui) {

                    // Selected an item, nothing to do
                    if (ui.item) {
                        return;
                    }

                    // Search for a match (case-insensitive)
                    var value = this.input.val(),
                        valueLowerCase = value.toLowerCase(),
                        valid = false;
                    this.element.children("option").each(function () {
                        if ($(this).text().toLowerCase() === valueLowerCase) {
                            this.selected = valid = true;
                            return false;
                        }
                    });

                    // Found a match, nothing to do
                    if (valid) {
                        return;
                    }

                    // Remove invalid value
                    this.input
                        .val("")
                        .attr("title", value + " didn't match any item")
                        .tooltip("open");
                    this.element.val("");
                    this._delay(function () {
                        this.input.tooltip("close").attr("title", "");
                    }, 2500);
                    this.input.autocomplete("instance").term = "";
                },

                _destroy: function () {
                    this.wrapper.remove();
                    this.element.show();
                }
            });

            //activate the autocomplete combobox
            $("[id*=DropDownListFCodes]").combobox();

        }


    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <div id="topSection" style="position:relative;" class="row">
                    <div id="hdrDiv"  class="col-sm-4">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv" class="col-sm-8">
                        <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                        &nbsp;&nbsp; 
                        <asp:HiddenField ID="HFEnteredPayrollDates" runat="server" />
                        <asp:HiddenField ID="HFLockedPayrollDates" runat="server" />
                        <strong>Start Date:</strong>
                        <asp:TextBox ID="TextBoxPayrollStartDate" runat="server" Width="100" AutoPostBack="true" 
                            OnTextChanged="TextBoxDate_TextChanged"></asp:TextBox>
                        &nbsp;
                        <strong>End Date:</strong>
                        <asp:TextBox ID="TextBoxPayrollEndDate" runat="server" Width="100" AutoPostBack="true" 
                            OnTextChanged="TextBoxDate_TextChanged"></asp:TextBox>
                        <br />
                        <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="False" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                        <strong>Health Lookup?</strong>
                            <asp:DropDownList ID="DDLHealthLookup" runat="server" ToolTip="Automatically enter health amounts?">
                                <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                <asp:ListItem Text="No" Value="No" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                    </div>

                    <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                    <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                    <asp:HiddenField id="hdnScrollPos" runat="server"/>

                </div>
                <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                    <div class="col-sm-12">
                        <asp:GridView ID="GridViewPayroll" runat="server" AutoGenerateColumns="False" DataKeyNames="Counter,DistrictID,PayDate" DataSourceID="SqlDataSourcePayroll" CssClass="mGrid"
                        ShowFooter="true" OnRowCommand="GridViewPayroll_RowCommand" ShowHeaderWhenEmpty="true" OnRowDataBound="GridViewPayroll_RowDataBound" AllowSorting="true" 
                        OnPreRender="GridViewPayroll_PreRender">
                        <Columns>
                            <asp:TemplateField HeaderText="Pay Date" SortExpression="Paydate">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelEPayDate" runat="server" Text='<%# Eval("PayDate","{0:d}") %>' Width="100"></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelPayDate" runat="server" Text='<%# Eval("PayDate","{0:d}") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFPayDate" runat="server"  Width="100" AutoPostBack="true" OnTextChanged="TextBoxDate_TextChanged" AutoCompleteType="Disabled"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqPayDate" runat="server" ControlToValidate="TextBoxFPayDate" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Payroll Date is required." ValidationGroup="Insert">*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Employee" SortExpression="EmpName" HeaderStyle-Width="210">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListEmps" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceAllEmps" DataTextField="EmpName" 
                                        DataValueField="EmployeeNum" SelectedValue='<%# Bind("EmployeeNum") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListEmps_SelectedIndexChanged"
                                        >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLEEmps" runat="server" ValueToCompare="0" ControlToValidate="DropDownListEmps" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Employee is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelEmpName" runat="server" Text='<%# Eval("EmpName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="ui-widget">
                                        <asp:DropDownList ID="DropDownListFEmps" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceActiveEmps" DataTextField="EmpName" AutoPostBack="true" 
                                            DataValueField="EmployeeNum" SelectedValue='<%# Bind("EmployeeNum") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListEmps_SelectedIndexChanged">
                                            <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                        <asp:CompareValidator ID="CompareDDLFEmps" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFEmps" Operator="GreaterThan" Display="Dynamic" 
                                            CssClass="validators" ErrorMessage="Employee is required." ValidationGroup="Insert">*</asp:CompareValidator>

                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="School" SortExpression="SchoolNum" HeaderStyle-Width="140">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceAllSchools" DataTextField="SchoolName" 
                                        DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLESchool" runat="server" ValueToCompare="0" ControlToValidate="DropDownListSchools" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="School is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="ui-widget">
                                        <asp:DropDownList ID="DropDownListFSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceActiveSchools" DataTextField="SchoolName" 
                                            DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                            <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    
                                    <asp:CompareValidator ID="CompareDDLFSchool" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFSchools" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="School is required." ValidationGroup="Insert">*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Hours" SortExpression="Hours">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditHours" runat="server" Text='<%# Bind("Hours") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelEditHours" runat="server" Text='<%# Eval("Hours") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFHours" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Category" HeaderStyle-Width="140">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListECodes" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourcePaycodes" DataTextField="PayCodeDesc" 
                                        DataValueField="PayCode" SelectedValue='<%# Bind("PayCode") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListPayCodes_SelectedIndexChanged" >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLECodes" runat="server" ValueToCompare="0" ControlToValidate="DropDownListECodes" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Category is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:Label ID="TextBoxCategory" runat="server" Text='<%# Eval("PayCodeDesc") %>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="ui-widget">
                                        <asp:DropDownList ID="DropDownListFCodes" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourcePaycodes" DataTextField="PayCodeDesc" AutoPostBack="true"
                                            DataValueField="PayCode" SelectedValue='<%# Bind("PayCode") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListPayCodes_SelectedIndexChanged" >
                                            <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    
                                    <asp:CompareValidator ID="CompareDDLFCodes" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFCodes" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Category is required." ValidationGroup="Insert">*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditRate" runat="server" Text='<%# Bind("Rate") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelRate" runat="server" Text='<%# Eval("Rate") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFRate" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Other $" SortExpression="OtherPay">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditOther" runat="server" Text='<%# Bind("OtherPay") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelOther" runat="server" Text='<%# Eval("OtherPay") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFOther" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Health" SortExpression="Health">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditHealth" runat="server" Text='<%# Bind("Health") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelHealth" runat="server" Text='<%# Eval("Health") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFHealth" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="FFV" HeaderStyle-Width="50" SortExpression="FFV" Visible="false" >
                                <EditItemTemplate>
                                    <div style="width:50px;text-align:center;padding-left:15px;">
                                        <asp:CheckBox ID="cbEFFV" runat="server" Checked='<%# Bind("FFV") %>' Enabled="true" ToolTip="Fresh Fruit & Vegetable" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:50px;text-align:center;padding-left:15px;">
                                        <asp:CheckBox ID="cbFFV" runat="server" Checked='<%# Eval("FFV") %>' Enabled="false" ToolTip="Fresh Fruit & Vegetable" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="width:50px;text-align:center;padding-left:15px;">
                                        <asp:CheckBox ID="cbFFFV" runat="server" ToolTip="Fresh Fruit & Vegetable"  />
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Locked" HeaderStyle-Width="50">
                                <ItemTemplate>
                                    <div style="width:50px;text-align:center;padding-left:15px;">
                                        <asp:CheckBox ID="cbLocked" runat="server" Checked='<%# Eval("Locked") %>' Enabled="false" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" <%# (bool)Eval("Locked")==true?"hidden='hidden'":"" %> >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Update" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:60px;text-align:center;"<%# (bool)Eval("Locked")==true?"hidden='hidden'":"" %> >
                                        <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                            CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        
                                <ItemStyle Width="50" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                        <div class="messagealert" id="alert_container"></div>

                        
                    <asp:DetailsView ID="DetailsViewDistInfo" runat="server" Height="50px" Width="125px" AutoGenerateRows="False" DataSourceID="SqlDataSourceDistYearInfo" Visible="false">
                        <Fields>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CBFFVDist" runat="server" Text="FFVDistrict" Checked='<%# Eval("FFVDistrict")  %>' Enabled="false"  />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    
                </div>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAllSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceActiveSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistYearInfo" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT FFVDistrict FROM [DistrictYearInfo] WHERE DistrictID=@DistrictID AND ProgramYear=@ProgramYear">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePayroll" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            UpdateCommand="UPDATE [Payroll] SET [EmployeeNum]=@EmployeeNum, SchoolNum=@SchoolNum, PayCode=@PayCode, Hours=@Hours, Rate=@Rate, OtherPay=@OtherPay, Health=@Health, FFV=@FFV
            WHERE DistrictID=@DistrictID AND PayDate=@PayDate AND Counter=@Counter"

            InsertCommand="INSERT INTO [Payroll] ([DistrictID],[EmployeeNum],[ProgramYear],[PayDate],[SchoolNum],[PayCode],[Hours],[Rate],[OtherPay],[Health],[Locked],[FFV]) 
            VALUES(@DistrictID,@EmployeeNum,@ProgramYear,@PayDate,@SchoolNum,@PayCode,@Hours,@Rate,@OtherPay,@Health,0,@FFV)"

            OnInserting="SqlDataSourcePayroll_Inserting" OnInserted="SqlDataSourcePayroll_Inserted"
            OnDeleted="SqlDataSourcePayroll_Deleted"

            DeleteCommand="DELETE [Payroll] WHERE DistrictID=@DistrictID AND PayDate=@PayDate AND Counter=@Counter AND Locked=0"
            SelectCommand="SELECT PAY.[Counter],PAY.[DistrictID],PAY.[EmployeeNum],PAY.[ProgramYear],PAY.[PayDate],PAY.[SchoolNum],PAY.[PayCode]
            ,PAY.[Hours],PAY.[Rate],PAY.[OtherPay],PAY.[Health],PAY.[Locked]
            , [SchoolName], CASE WHEN EMP.[Inactive]=1 THEN 'X-' ELSE '' END + [LastName] + ', ' + [FirstName] EmpName 
            ,ISNULL(PayCodeDesc,  RTRIM(CAST(PAY.PayCode AS Varchar(3))) ) PayCodeDesc, PAY.[FFV]
            FROM [Payroll] PAY
            LEFT OUTER JOIN [DistrictSchools] DS ON PAY.[DistrictID]=DS.[DistrictID] AND PAY.[SchoolNum]=DS.[SchoolNum]
            LEFT OUTER JOIN [Employees] EMP ON PAY.[DistrictID]=EMP.[DistrictID] AND PAY.[EmployeeNum]=EMP.[EmployeeNum]
            LEFT OUTER JOIN [DistrictPaycodes] DPC ON PAY.[DistrictID]=DPC.[DistrictID] AND PAY.[ProgramYear]=DPC.[ProgramYear] AND PAY.[PayCode]=DPC.[PayCode]
            WHERE PAY.DistrictID=@DistrictID AND PAY.PayDate>=@PayDateStart AND PAY.PayDate<=@PayDateEnd AND PAY.ProgramYear=@ProgramYear
            UNION ALL --Needed for Insert Row
            SELECT 0,'',0,'','1/1/1900',0,0,0,0,0,0,CAST(0 AS bit) Locked
            ,'' SchoolName, '' EmpName, '' PayCodeDesc,CAST(0 AS bit) FFV
            ORDER BY [Counter]">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter Name="PayDateStart" ControlID="TextBoxPayrollStartDate" PropertyName="Text" Type="DateTime" />
                <asp:ControlParameter Name="PayDateEnd" ControlID="TextBoxPayrollEndDate" PropertyName="Text" Type="DateTime" />
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="PayDate" Type="DateTime" />
                <asp:Parameter Name="Counter" Type="Int32" />
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="PayCode" Type="Int16" />
                <asp:Parameter Name="Hours" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="OtherPay" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="FFV" Type="Byte" DefaultValue="0" />
            </UpdateParameters>
            <InsertParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                <asp:Parameter Name="PayDate" Type="DateTime" />
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="PayCode" Type="Int16" />
                <asp:Parameter Name="Hours" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="OtherPay" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="FFV" Type="Byte" DefaultValue="0" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAllEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT EmployeeNum, CASE WHEN Inactive=1 THEN 'X-' ELSE '' END + LastName + ', ' + FirstName EmpName FROM [Employees] WHERE DistrictID=@DistrictID ORDER BY LastName,FirstName ">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceActiveEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT EmployeeNum, LastName + ', ' + FirstName EmpName FROM [Employees] WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY LastName,FirstName ">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePaycodes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT [PayCode], [PayCodeDesc] FROM [DistrictPaycodes] WHERE DistrictID=@DistrictID AND ProgramYear=@ProgramYear ORDER BY PayCodeDesc ">
            <SelectParameters>
                <asp:ControlParameter Name="DistrictID" ControlID="DropDownListDistricts" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
