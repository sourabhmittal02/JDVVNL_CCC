using System.Web.Mvc;

namespace ComplaintTracker.Areas.DirectComplaintRegister
{
    public class DirectComplaintRegisterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DirectComplaintRegister";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {


            context.MapRoute(
                "DirectComplaintRegister_default",
                "DirectComplaint/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );


            context.MapRoute("CloseComplaintA", "{controller}/*close", new { action = "closeSearch" });
            context.MapRoute("CloseComplaintB", "{controller}/searchComplaint", new { action = "closeSearch" });
            context.MapRoute("CloseComplaintC", "{controller}/ResolveComplaint", new { action = "closeSearch" });

            context.MapRoute(
                "CloseComplaint",
                "{controller}/{action}/{id}",
                new { action = "closeSearch", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "CloseComplaint1",
                "{controller}/close/{id}",
                new { action = "closeSearch", id = UrlParameter.Optional }
            );

           


        }
    }
}