using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html; 

namespace Sample.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, Type enumType, object selected)
        {
            var items = Enum.GetValues(enumType);
            var listItems = new List<SelectListItem>();

            foreach (var item in items)
            {
                listItems.Add(new SelectListItem
                {
                    Selected = item.Equals(selected),
                    Text = Enum.GetName(enumType, item),
                    Value = item.ToString()
                });
            }

            return htmlHelper.DropDownList(name, listItems); 
        }
    }
}