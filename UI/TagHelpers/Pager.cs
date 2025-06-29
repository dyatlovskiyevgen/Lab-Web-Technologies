using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace OSS.UI.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class Pager : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Pager(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        // номер текущей страницы 
        public int PageCurrent { get; set; }
        // общее количество страниц 
        public int PageTotal { get; set; }
        // имя категории объектов 
        public string? Category { get; set; }
        // имя действия 
        public string Action { get; set; } = "Index";
        // имя контроллера 
        public string Controller { get; set; } = "Product";
        // признак страниц администратора 
        //public bool Admin { get; set; } = false;
        public bool ? Admin { get; set; } 
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "row");

            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "Page navigation example");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            // Кнопка "Назад"
            var prevLi = CreateListItem(PageCurrent - 1, "<span aria-hidden=\"true\">&laquo;</span>");
            prevLi.AddCssClass(PageCurrent == 1 ? "disabled" : "");
            ul.InnerHtml.AppendHtml(prevLi);

            // Нумерация страниц
            for (var i = 1; i <= PageTotal; i++)
            {
                var pageLi = CreateListItem(i, i.ToString());
                pageLi.AddCssClass(i == PageCurrent ? "active" : "");
                ul.InnerHtml.AppendHtml(pageLi);
            }

            // Кнопка "Вперед"
            var nextLi = CreateListItem(PageCurrent + 1, "<span aria-hidden=\"true\">&raquo;</span>");
            nextLi.AddCssClass(PageCurrent == PageTotal ? "disabled" : "");
            ul.InnerHtml.AppendHtml(nextLi);

            nav.InnerHtml.AppendHtml(ul);
            output.Content.AppendHtml(nav);
        }

        private TagBuilder CreateListItem(int pageNo, string innerHtml)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            var routeData = new { pageno = pageNo, category = Category };
            string url;

            if (Admin == true)
            {
                url = _linkGenerator.GetPathByPage(_httpContextAccessor.HttpContext, page: "./Index", values: routeData);
            }
            else
            {
                url = _linkGenerator.GetPathByAction(Action, Controller, routeData);
            }

            a.Attributes.Add("href", url);
            a.InnerHtml.AppendHtml(innerHtml);

            // Добавляем aria-атрибуты для кнопок навигации
            if (innerHtml.Contains("&laquo;"))
            {
                a.Attributes.Add("aria-label", "Previous");
            }
            else if (innerHtml.Contains("&raquo;"))
            {
                a.Attributes.Add("aria-label", "Next");
            }

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}