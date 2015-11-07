using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SoniReports.Tools
{
    [Obsolete]
    public static class HtmlHelperExt
    {
        //NOT IN USE
        //public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        //{
        //    string checkBoxWithHidden = htmlHelper.CheckBox(name, htmlAttributes).ToHtmlString().Trim();
        //    string pureCheckBox = RemoveHiddenCheckBox(checkBoxWithHidden);
        //    return new MvcHtmlString(pureCheckBox);
        //}

        //private static string RemoveHiddenCheckBox(string checkBoxWithHidden)
        //{
        //    string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
        //    return pureCheckBox;
        //}

        //public static MvcHtmlString CheckBoxSimpleFor<TModel>(this HtmlHelper<TModel> htmlHelper, 
        //    Expression<Func<TModel, bool>> expression, object htmlAttributes)
        //{
        //    string checkBoxWithHidden = htmlHelper.CheckBoxFor(expression, htmlAttributes).ToHtmlString().Trim();
        //    string pureCheckBox = RemoveHiddenCheckBox(checkBoxWithHidden);
        //    return new MvcHtmlString(pureCheckBox);
        //}
    }
}