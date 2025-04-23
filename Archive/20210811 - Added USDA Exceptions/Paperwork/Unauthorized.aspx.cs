using System;

namespace Paperwork
{
    public partial class Unauthorized : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();
        }
    }
}